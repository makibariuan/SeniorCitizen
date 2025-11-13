using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineRegistration.Server.Services
{
    public class AfisQueueWorker : BackgroundService
    {
        private readonly AfisQueueService _queue;

        public AfisQueueWorker(AfisQueueService queue)
        {
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[AFIS] Background queue worker started...");

            try
            {
                await _queue.ProcessQueueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[AFIS] Worker cancellation requested. Stopping...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AFIS] ❌ Worker crashed: {ex.Message}");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[AFIS] Worker stopping, saving queue...");
            _queue.SaveQueueForShutdown();  // ensures queue is persisted
            return base.StopAsync(cancellationToken);
        }
    }
}
