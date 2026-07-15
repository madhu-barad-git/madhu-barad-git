namespace LivePolling.Api.Models;

public record CreatePollRequest(string Question, List<string> Options);

public record VoteRequest(Guid OptionId, string VoterKey);

public record PollOptionResponse(Guid Id, string Text, int VoteCount);

public record PollResponse(
    Guid Id,
    string Question,
    DateTime CreatedAt,
    bool IsClosed,
    List<PollOptionResponse> Options);
