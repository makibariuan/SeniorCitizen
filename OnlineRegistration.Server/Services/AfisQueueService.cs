using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OnlineRegistration.Server.Models;

namespace OnlineRegistration.Server.Services
{
    public class AfisQueueService
    {
        private readonly ConcurrentQueue<AfisJob> _queue = new();
        private readonly HttpClient _http = new();
        private readonly string _afisUrl;
        private readonly string _queueFile;
        private readonly object _fileLock = new();

        public AfisQueueService(IConfiguration config)
        {
            _afisUrl = config["AFIS:ApiUrl"] ?? "https://localhost:3000/api/AFIS/enroll";
            _queueFile = config["AFIS:QueueFile"] ?? "afis_queue.json";
            LoadQueueFromFile();
        }

        public void Enqueue(AfisJob job)
        {
            _queue.Enqueue(job);
            SaveQueueToFile();
            Console.WriteLine($"[AFIS] 📥 Job enqueued for PersonID: {job.PersonId}");
        }

        public async Task ProcessQueueAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_queue.TryPeek(out var job))
                {
                    if (DateTime.UtcNow >= job.NextAttempt)
                    {
                        bool success = await SendToAfisApiAsync(job);

                        if (success)
                        {
                            _queue.TryDequeue(out _);
                            SaveQueueToFile();
                            Console.WriteLine($"[AFIS] ✅ Sent fingerprint for PersonID: {job.PersonId}");
                        }
                        else
                        {
                            _queue.TryDequeue(out _);
                            job.RetryCount++;
                            job.NextAttempt = DateTime.UtcNow + GetBackoffDelay(job.RetryCount);
                            _queue.Enqueue(job);
                            SaveQueueToFile();
                            Console.WriteLine($"[AFIS] ❌ Retry {job.RetryCount} scheduled at {job.NextAttempt}");
                        }
                    }
                }

                await Task.Delay(3000, token);
            }
        }

        private async Task<bool> SendToAfisApiAsync(AfisJob job)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(_afisUrl, new
                {
                    personId = job.PersonId,
                    fingerPosition = job.FingerPosition,
                    base64Image = job.Base64Image
                });

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AFIS] ❌ Failed to send: {ex.Message}");
                return false;
            }
        }

        private static TimeSpan GetBackoffDelay(int retryCount)
        {
            return retryCount switch
            {
                1 => TimeSpan.FromMinutes(1),
                2 => TimeSpan.FromMinutes(5),
                3 => TimeSpan.FromMinutes(30),
                4 => TimeSpan.FromHours(2),
                _ => TimeSpan.FromHours(6)
            };
        }

        private void SaveQueueToFile()
        {
            lock (_fileLock)
            {
                try
                {
                    var jobs = _queue.ToArray();
                    var json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(_queueFile, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AFIS] ⚠ Failed to save queue: {ex.Message}");
                }
            }
        }

        private void LoadQueueFromFile()
        {
            if (!File.Exists(_queueFile)) return;

            try
            {
                var json = File.ReadAllText(_queueFile);
                var jobs = JsonSerializer.Deserialize<List<AfisJob>>(json);
                if (jobs != null)
                {
                    foreach (var job in jobs)
                        _queue.Enqueue(job);
                }

                Console.WriteLine($"[AFIS] 🧾 Loaded {jobs?.Count ?? 0} jobs from queue file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AFIS] ⚠ Failed to load queue: {ex.Message}");
            }
        }

        public void SaveQueueForShutdown()
        {
            Console.WriteLine("[AFIS] 🧩 Saving queue to file before shutdown...");
            SaveQueueToFile();
        }
    }
}
