using Microsoft.EntityFrameworkCore;
using PocketLedger.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<PocketLedgerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Pocket Ledger API");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
