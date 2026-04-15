using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PocketLedger.Infrastructure.Persistence;

public sealed class PocketLedgerDesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<PocketLedgerDbContext>
{
    private const string ConnectionStringEnvironmentVariable = "POCKETLEDGER_CONNECTION_STRING";

    public PocketLedgerDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable)
                               ?? throw new InvalidOperationException(
                                   $"Environment variable '{ConnectionStringEnvironmentVariable}' was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<PocketLedgerDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new PocketLedgerDbContext(optionsBuilder.Options);
    }
}