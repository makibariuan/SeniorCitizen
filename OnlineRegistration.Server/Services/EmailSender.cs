using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;

namespace OnlineRegistration.Server.Services
{
    public class EmailSender : BackgroundService
    {
        private readonly IEmailQueue _queue;
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IEmailQueue queue, IConfiguration config, ILogger<EmailSender> logger)
        {
            _queue = queue;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("📧 Email sender started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var email = await _queue.DequeueAsync(stoppingToken);
                if (email == null) continue;

                int retryCount = 0;
                const int maxRetries = 5;
                TimeSpan delay = TimeSpan.FromSeconds(2);

                while (retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(_config["Smtp:From"], _config["Smtp:From"]));
                        message.To.Add(new MailboxAddress(email.To, email.To));
                        message.Subject = email.Subject;
                        message.Body = new TextPart("html") { Text = email.Body };

                        using var client = new SmtpClient();

                        // Accept all SSL certs (optional, for debugging self-signed certs)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        var host = _config["Smtp:Host"];
                        var port = int.Parse(_config["Smtp:Port"]!);

                        try
                        {
                            // Try STARTTLS first if port is 587
                            if (port == 587)
                                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, stoppingToken);
                            else if (port == 465)
                                await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect, stoppingToken);
                            else
                                await client.ConnectAsync(host, port, SecureSocketOptions.Auto, stoppingToken);
                        }
                        catch
                        {
                            // Fallback: try SSL if STARTTLS fails
                            await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect, stoppingToken);
                        }

                        await client.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Pass"], stoppingToken);
                        await client.SendAsync(message, stoppingToken);
                        await client.DisconnectAsync(true, stoppingToken);

                        _logger.LogInformation("✅ Email sent to {Email}", email.To);
                        break; // ✅ Success, exit retry loop
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        _logger.LogWarning(ex,
                            "⚠️ Failed to send email to {Email} (Attempt {Retry}/{Max})",
                            email.To, retryCount, maxRetries);

                        if (retryCount >= maxRetries)
                        {
                            _logger.LogError("❌ Giving up on sending email to {Email} after {MaxRetries} retries",
                                email.To, maxRetries);
                            break;
                        }

                        await Task.Delay(delay, stoppingToken);
                        delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
                    }
                }
            }
        }
    }
}
