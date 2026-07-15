namespace LivePolling.Api.Models;

public class Poll
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Question { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsClosed { get; set; }

    public List<PollOption> Options { get; set; } = [];
}
