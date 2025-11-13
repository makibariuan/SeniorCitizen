namespace OnlineRegistration.Server.Models
{
    public class AfisJob
    {
        public int PersonId { get; set; }
        public int FingerPosition { get; set; }
        public string Base64Image { get; set; } = string.Empty;

        public int RetryCount { get; set; } = 0;
        public DateTime NextAttempt { get; set; } = DateTime.UtcNow;
    }
}
