using FluentAssertions;
using LanguageExt;
using LanguageExt.UnitTesting;
using Microsoft.EntityFrameworkCore;
using PocketLedger.Core.SourceEvents.Models;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using PocketLedger.Domain.Entities;
using PocketLedger.Infrastructure.IntegrationTests.Common;
using PocketLedger.Infrastructure.Persistence;
using PocketLedger.Infrastructure.Persistence.Mappers;
using PocketLedger.Infrastructure.Persistence.Repositories;
using static LanguageExt.Prelude;
using static PocketLedger.Domain.Common.Prelude;
using SourceEventId = PocketLedger.Domain.Common.Primitives.GuidTypes.SourceEventId;

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

        _ = result.Match(
            persisted => persisted.Id.Should().Be(sourceEvent.Id),
            error => throw new Exception($"Expected success but got error: {error}")
        );

        var persistedInDb = await _dbContext.SourceEvents
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == (Guid)sourceEvent.Id, CancellationToken.None);

        if (persistedInDb is null)
            throw new Exception("Expected success but got error: record not saved into the DB");

        var persistedEntity = SourceEventMapper.ToDomain(persistedInDb);

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

    [Fact]
    public async Task List_GivenSourceTypeFilter_ReturnsMatchingSourceEventsOrderedByReceivedAtDesc()
    {
        // Arrange
        var olderWalletEvent = BuildSourceEvent(
            oType: SourceEventType.Wallet,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-10));

        var smsEvent = BuildSourceEvent(
            oType: SourceEventType.Sms,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-5));

        var newerWalletEvent = BuildSourceEvent(
            oType: SourceEventType.Wallet,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        await _repository.Add(olderWalletEvent, CancellationToken.None);
        await _repository.Add(smsEvent, CancellationToken.None);
        await _repository.Add(newerWalletEvent, CancellationToken.None);

        var filters = new SourceEventFilters(
            SourceType: Some(SourceEventType.Wallet),
            ExternalId: None
        );

        // Act
        var vResult = await _repository.List(filters, CancellationToken.None);

        // Assert
        vResult.ShouldBeSuccess(events =>
        {
            events.Should().HaveCount(2);
            events.Should().OnlyContain(x => x.Type == SourceEventType.Wallet);
            events.Map(x => x.Id).Should().ContainInOrder(
                newerWalletEvent.Id,
                olderWalletEvent.Id
            );
        });
    }

    [Fact]
    public async Task List_GivenExternalIdFilter_ReturnsOnlyMatchingSourceEvents()
    {
        // Arrange
        var matchingExternalId = ExternalId("ext-123");
        var otherExternalId = ExternalId("ext-999");

        var olderMatchingEvent = BuildSourceEvent(
            oType: SourceEventType.Wallet,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-15),
            oExternalId: matchingExternalId);

        var nonMatchingEvent = BuildSourceEvent(
            oType: SourceEventType.Sms,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-10),
            oExternalId: otherExternalId);

        var newerMatchingEvent = BuildSourceEvent(
            oType: SourceEventType.Wallet,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-5),
            oExternalId: matchingExternalId);

        await _repository.Add(olderMatchingEvent, CancellationToken.None);
        await _repository.Add(nonMatchingEvent, CancellationToken.None);
        await _repository.Add(newerMatchingEvent, CancellationToken.None);

        var filters = new SourceEventFilters(
            SourceType: None,
            ExternalId: matchingExternalId);

        // Act
        var vResult = await _repository.List(filters, CancellationToken.None);

        // Assert
        vResult.ShouldBeSuccess(sourceEvents =>
        {
            sourceEvents.Should().HaveCount(2);
            sourceEvents.Select(x => x.Id).Should().ContainInOrder(
                newerMatchingEvent.Id,
                olderMatchingEvent.Id);
        });
    }

    [Fact]
    public async Task List_GivenEmptyFilters_ReturnsAllSourceEventsOrderedByReceivedAtDesc()
    {
        // Arrange
        var oldestEvent = BuildSourceEvent(
            oType: SourceEventType.Wallet,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-15));

        var middleEvent = BuildSourceEvent(
            oType: SourceEventType.Sms,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-10));

        var newestEvent = BuildSourceEvent(
            oType: SourceEventType.Wallet,
            oReceivedAt: DateTimeOffset.UtcNow.AddMinutes(-5));

        await _repository.Add(oldestEvent, CancellationToken.None);
        await _repository.Add(middleEvent, CancellationToken.None);
        await _repository.Add(newestEvent, CancellationToken.None);

        // Act
        var vResult = await _repository.List(SourceEventFilters.Empty, CancellationToken.None);

        // Assert
        vResult.ShouldBeSuccess(sourceEvents =>
        {
            sourceEvents.Should().HaveCount(3);
            sourceEvents.Select(x => x.Id).Should().ContainInOrder(
                newestEvent.Id,
                middleEvent.Id,
                oldestEvent.Id);
        });
    }

    private static SourceEvent BuildSourceEvent(
        Option<SourceEventType> oType = default,
        Option<RawPayload> oPayload = default,
        Option<DateTimeOffset> oReceivedAt = default,
        Option<DateTimeOffset> oOccurredAt = default,
        Option<ExternalId> oExternalId = default) =>
        SourceEvent.Create(
            oType.IfNone(BuildSourceType),
            oPayload.IfNone(BuildRawPayload),
            oReceivedAt.IfNone(() => DateTimeOffset.UtcNow),
            oOccurredAt,
            oExternalId);

    private static SourceEventType BuildSourceType() => SourceEventType.Wallet;

    private static RawPayload BuildRawPayload() =>
        RawPayload("""
                   {
                     "source": "Wallet",
                     "message": "Compra aprobada por 23500 COP en OXXO",
                     "merchant": "OXXO",
                     "amount": 23500,
                     "currency": "COP"
                   }
                   """);
}