using System.Collections.Concurrent;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;

namespace OnlineRegistration.Server.Services
{
    public class EmailBackgroundQueue : IEmailQueue
    {
        private readonly ConcurrentQueue<EmailMessage> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void QueueEmail(EmailMessage message)
        {
            _queue.Enqueue(message);
            _signal.Release();
        }

        public async Task<EmailMessage?> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var message);
            return message;
        }
    }
}
