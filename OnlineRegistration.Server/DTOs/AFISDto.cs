namespace OnlineRegistration.Server.DTOs
{
    public class PersonWithHitDto
    {
        public int PersonA { get; set; }
        public int PersonB { get; set; }
        public float Score { get; set; }
        public DateTime DateDetected { get; set; }
    }

    public class PersonValidatedDto
    {
        public int PersonId { get; set; }
        public DateTime ValidatedAt { get; set; }
        public bool HasHit { get; set; }
    }
}
