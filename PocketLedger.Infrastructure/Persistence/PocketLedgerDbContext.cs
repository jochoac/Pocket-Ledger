using Microsoft.EntityFrameworkCore;

namespace PocketLedger.Infrastructure.Persistence;

public sealed class PocketLedgerDbContext(DbContextOptions<PocketLedgerDbContext> options) : DbContext(options)
{
}