namespace UserProfiles.Api.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }

        public string UserSub { get; set; } = String.Empty;

        public string JobId { get; set; } = String.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool CleanupStarted { get; set; } = false;
    }
}
