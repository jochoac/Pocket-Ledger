using Microsoft.EntityFrameworkCore;
using PocketLedger.Api.SourceEvents.GetSourceEvents;
using PocketLedger.Api.SourceEvents.RegisterSourceEvent;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Core.SourceEvents.UseCases.GetSourceEvent;
using PocketLedger.Core.SourceEvents.UseCases.RegisterSourceEvent;
using PocketLedger.Infrastructure.Persistence;
using PocketLedger.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<PocketLedgerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

//DI
builder.Services.AddScoped<RegisterSourceEventHandler>();
builder.Services.AddScoped<GetSourceEventHandler>();
builder.Services.AddScoped<ISourceEventRepository, SourceEventRepository>();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Pocket Ledger API");
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
//ENDPOINTS
//Source Events
app.MapRegisterSourceEventEndpoint();
app.MapGetSourceEventByIdEndpoint();
app.MapListSourceEventsEndpoint();

app.Run();