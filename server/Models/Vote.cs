namespace LivePolling.Api.Models;

public class Vote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PollId { get; set; }
    public Guid OptionId { get; set; }
    public required string VoterKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
