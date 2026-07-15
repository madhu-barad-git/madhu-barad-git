using LivePolling.Api.Data;
using LivePolling.Api.Hubs;
using LivePolling.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LivePolling.Api.Endpoints;

public static class PollEndpoints
{
    private const int MinOptions = 2;
    private const int MaxOptions = 6;

    public static void MapPollEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/polls");

        group.MapPost("/", CreatePoll);
        group.MapGet("/{id:guid}", GetPoll);
        group.MapPost("/{id:guid}/vote", Vote);
        group.MapPost("/{id:guid}/close", ClosePoll);
    }

    private static async Task<IResult> CreatePoll(CreatePollRequest request, PollDbContext db)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return Results.BadRequest(new { message = "Question is required." });

        var options = request.Options
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => o.Trim())
            .ToList();

        if (options.Count is < MinOptions or > MaxOptions)
            return Results.BadRequest(new { message = $"A poll must have between {MinOptions} and {MaxOptions} options." });

        var poll = new Poll
        {
            Question = request.Question.Trim(),
            Options = options.Select(text => new PollOption { Text = text }).ToList()
        };

        db.Polls.Add(poll);
        await db.SaveChangesAsync();

        return Results.Created($"/api/polls/{poll.Id}", ToResponse(poll));
    }

    private static async Task<IResult> GetPoll(Guid id, PollDbContext db)
    {
        var poll = await db.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == id);

        return poll is null ? Results.NotFound() : Results.Ok(ToResponse(poll));
    }

    private static async Task<IResult> Vote(Guid id, VoteRequest request, PollDbContext db, IHubContext<PollHub> hubContext)
    {
        if (string.IsNullOrWhiteSpace(request.VoterKey))
            return Results.BadRequest(new { message = "voterKey is required." });

        var poll = await db.Polls.FindAsync(id);
        if (poll is null)
            return Results.NotFound();

        if (poll.IsClosed)
            return Results.BadRequest(new { message = "This poll is closed." });

        var optionBelongsToPoll = await db.PollOptions.AnyAsync(o => o.Id == request.OptionId && o.PollId == id);
        if (!optionBelongsToPoll)
            return Results.BadRequest(new { message = "Invalid option for this poll." });

        await using var transaction = await db.Database.BeginTransactionAsync();

        db.Votes.Add(new Vote
        {
            PollId = id,
            OptionId = request.OptionId,
            VoterKey = request.VoterKey.Trim()
        });

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return Results.Conflict(new { message = "You've already voted on this poll." });
        }

        // Atomic increment (single UPDATE statement) to avoid lost updates under concurrent votes.
        await db.PollOptions
            .Where(o => o.Id == request.OptionId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(o => o.VoteCount, o => o.VoteCount + 1));

        await transaction.CommitAsync();

        var updated = await db.Polls.Include(p => p.Options).FirstAsync(p => p.Id == id);
        var response = ToResponse(updated);

        await hubContext.Clients.Group(id.ToString()).SendAsync("PollUpdated", response);

        return Results.Ok(response);
    }

    private static async Task<IResult> ClosePoll(Guid id, PollDbContext db)
    {
        var poll = await db.Polls.Include(p => p.Options).FirstOrDefaultAsync(p => p.Id == id);
        if (poll is null)
            return Results.NotFound();

        poll.IsClosed = true;
        await db.SaveChangesAsync();

        return Results.Ok(ToResponse(poll));
    }

    private static PollResponse ToResponse(Poll poll) => new(
        poll.Id,
        poll.Question,
        poll.CreatedAt,
        poll.IsClosed,
        poll.Options.Select(o => new PollOptionResponse(o.Id, o.Text, o.VoteCount)).ToList());
}
