using LivePolling.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LivePolling.Api.Data;

public class PollDbContext(DbContextOptions<PollDbContext> options) : DbContext(options)
{
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<PollOption> PollOptions => Set<PollOption>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Options)
            .WithOne(o => o.Poll)
            .HasForeignKey(o => o.PollId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.PollId, v.VoterKey })
            .IsUnique();
    }
}
