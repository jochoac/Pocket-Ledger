using FluentAssertions;
using LanguageExt;
using LanguageExt.UnitTesting;
using Microsoft.EntityFrameworkCore;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.GuidTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using PocketLedger.Domain.Entities;
using PocketLedger.Infrastructure.IntegrationTests.Common;
using PocketLedger.Infrastructure.Persistence;
using PocketLedger.Infrastructure.Persistence.Mappers;
using PocketLedger.Infrastructure.Persistence.Repositories;
using static LanguageExt.Prelude;

namespace PocketLedger.Infrastructure.IntegrationTests.SourceEvents;

public sealed class SourceEventRepositoryTests : IAsyncLifetime
{
    private PocketLedgerDbContext _dbContext;
    private SourceEventRepository _repository;

    public Task InitializeAsync()
    {
        _dbContext = TestDbContextFactory.Create();
        _repository = new SourceEventRepository(_dbContext);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task Add_GivenValidSourceEvent_PersistsItAndReturnsSuccess()
    {
        var sourceEvent = BuildSourceEvent();

        var result = await _repository.Add(sourceEvent, CancellationToken.None);

        result.Match(
            persisted => persisted.Id.Should().Be(sourceEvent.Id),
            error => throw new Exception($"Expected success but got error: {error}")
        );

        var persistedInDb = await _dbContext.SourceEvents
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == (Guid)sourceEvent.Id, CancellationToken.None);

        if (persistedInDb is null)
            throw new Exception("Expected success but got error: record not saved into the DB");
        
        var persistedEntity =  SourceEventMapper.ToDomain(persistedInDb);
        
        persistedEntity.Should().NotBeNull();
        persistedEntity.Id.Should().Be(sourceEvent.Id);
        persistedEntity.Type.Should().Be(sourceEvent.Type);
        persistedEntity.RawPayload.Value.Should().NotBeNullOrWhiteSpace();
        persistedEntity.RawPayload.Value.Should().Contain("Wallet");
        persistedEntity.RawPayload.Value.Should().Contain("OXXO");
        persistedEntity.RawPayload.Value.Should().Contain("23500");
        persistedEntity.ReceivedAt.Should().BeCloseTo(sourceEvent.ReceivedAt, TimeSpan.FromMilliseconds(1));
        persistedEntity.OccurredAt.ShouldBeNone();
        persistedEntity.ExternalId.ShouldBeNone();
    }
    
    [Fact]
    public async Task GetById_GivenExistingSourceEvent_ReturnsIt()
    {
        // Arrange
        var sourceEvent = BuildSourceEvent();

        await _repository.Add(sourceEvent, CancellationToken.None);

        // Act
        var vSourceEvent = await _repository.GetById(sourceEvent.Id, CancellationToken.None);

        // Assert
        vSourceEvent.ShouldBeSuccess(found =>
        {
            found.Id.Should().Be(sourceEvent.Id);
            found.Type.Should().Be(sourceEvent.Type);
            found.RawPayload.Value.Should().NotBeNullOrWhiteSpace();
            found.RawPayload.Value.Should().Contain("Wallet");
            found.RawPayload.Value.Should().Contain("OXXO");
            found.RawPayload.Value.Should().Contain("23500");
            found.ReceivedAt.Should().BeCloseTo(sourceEvent.ReceivedAt, TimeSpan.FromMilliseconds(1));
            found.ExternalId.ShouldBeNone();
        });
    }
    
    [Fact]
    public async Task GetById_GivenMissingSourceEvent_ReturnsNotFound()
    {
        // Arrange
        var sourceEventId = SourceEventId.New();

        // Act
        var vSourceEvent = await _repository.GetById(sourceEventId, CancellationToken.None);

        // Assert
        vSourceEvent.ShouldBeFail(errors => errors.Head.Should().BeOfType<EntityNotFoundError>());
    }

    private static SourceEvent BuildSourceEvent() =>
        SourceEvent.Create(
            BuildSourceType(),
            BuildRawPayload(),
            DateTimeOffset.UtcNow,
            None,
            BuildExternalId());

    private static SourceEventType BuildSourceType() => SourceEventType.Wallet;
    private static RawPayload BuildRawPayload() =>
        new("""
            {
              "source": "Wallet",
              "message": "Compra aprobada por 23500 COP en OXXO",
              "merchant": "OXXO",
              "amount": 23500,
              "currency": "COP"
            }
            """);
    private static Option<ExternalId> BuildExternalId() => None;
}