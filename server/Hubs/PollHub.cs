using Microsoft.AspNetCore.SignalR;

namespace LivePolling.Api.Hubs;

public class PollHub : Hub
{
    public Task JoinPoll(string pollId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, pollId);

    public Task LeavePoll(string pollId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, pollId);
}
