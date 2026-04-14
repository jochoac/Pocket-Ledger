using Microsoft.EntityFrameworkCore;
using PocketLedger.Infrastructure.Persistence.Models;

namespace PocketLedger.Infrastructure.Persistence;

public sealed class PocketLedgerDbContext(DbContextOptions<PocketLedgerDbContext> options) : DbContext(options)
{
    public DbSet<SourceEventRecord> SourceEvents => Set<SourceEventRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PocketLedgerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}