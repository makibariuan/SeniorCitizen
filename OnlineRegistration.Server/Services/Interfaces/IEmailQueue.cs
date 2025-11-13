using OnlineRegistration.Server.Models;

namespace OnlineRegistration.Server.Services.Interfaces
{
    public interface IEmailQueue
    {
        void QueueEmail(EmailMessage message);
        Task<EmailMessage?> DequeueAsync(CancellationToken cancellationToken);
    }
}
