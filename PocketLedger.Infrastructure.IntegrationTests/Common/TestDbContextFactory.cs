using Microsoft.EntityFrameworkCore;
using PocketLedger.Infrastructure.Persistence;

namespace PocketLedger.Infrastructure.IntegrationTests.Common;

public static class TestDbContextFactory
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=pocketledger_tests;Username=postgres;Password=postgres";

    public static PocketLedgerDbContext Create()
    {
        var options = new DbContextOptionsBuilder<PocketLedgerDbContext>()
            .UseNpgsql(ConnectionString)
            .EnableSensitiveDataLogging()
            .Options;

        var dbContext = new PocketLedgerDbContext(options);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}