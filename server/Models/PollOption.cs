namespace LivePolling.Api.Models;

public class PollOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PollId { get; set; }
    public required string Text { get; set; }
    public int VoteCount { get; set; }

    public Poll? Poll { get; set; }
}
