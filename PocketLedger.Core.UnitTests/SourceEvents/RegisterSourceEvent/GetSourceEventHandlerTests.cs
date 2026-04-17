using FluentAssertions;
using LanguageExt.UnitTesting;
using NSubstitute;
using PocketLedger.Core.SourceEvents.Models;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Core.SourceEvents.UseCases.GetSourceEvent;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Entities;
using static LanguageExt.Prelude;
using static PocketLedger.Domain.Common.Prelude;
using SourceEventId = PocketLedger.Domain.Common.Primitives.GuidTypes.SourceEventId;

namespace PocketLedger.Core.UnitTests.SourceEvents.RegisterSourceEvent;

public sealed class GetSourceEventHandlerTests
{
    private readonly ISourceEventRepository _sourceEventRepository = Substitute.For<ISourceEventRepository>();
    private readonly GetSourceEventHandler _handler;

    public GetSourceEventHandlerTests()
    {
        _handler = new GetSourceEventHandler(_sourceEventRepository);
    }

    [Fact]
    public async Task GetSourceEventByIdAsync_GivenExistingSourceEvent_ReturnsSuccess()
    {
        // Arrange
        var sourceEventId = SourceEventId.New();
        var sourceEvent = SourceEvent.Create(
            SourceEventType.Manual,
            RawPayload("""{ "description": "Lunch", "amount": 25000 }"""),
            new DateTimeOffset(2026, 4, 15, 15, 0, 0, TimeSpan.Zero),
            externalId: ExternalId("manual-001"));

        _sourceEventRepository
            .GetById(sourceEventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Success<Error, SourceEvent>(sourceEvent)));

        // Act
        var vSourceEvent = await _handler.GetSourceEventByIdAsync(sourceEventId, CancellationToken.None);

        // Assert
        vSourceEvent.ShouldBeSuccess(found => found.Should().Be(sourceEvent));
    }

    [Fact]
    public async Task GetSourceEventByIdAsync_GivenMissingSourceEvent_ReturnsNotFound()
    {
        // Arrange
        var sourceEventId = SourceEventId.New();

        _sourceEventRepository
            .GetById(sourceEventId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Errors.NotFound<SourceEvent>("SourceEvent", sourceEventId.ToString())));

        // Act
        var vSourceEvent = await _handler.GetSourceEventByIdAsync(sourceEventId, CancellationToken.None);

        // Assert
        vSourceEvent.ShouldBeFail(errors => errors.Head().Should().BeOfType<EntityNotFoundError>());
    }

    [Fact]
    public async Task Handle_GivenFilters_ReturnsSourceEventsFromRepository()
    {
        // Arrange
        var filters = new SourceEventFilters(
            SourceType: SourceEventType.Wallet,
            ExternalId: None);

        var sourceEvents = Seq(
            SourceEventHelpers.BuildSourceEvent(),
            SourceEventHelpers.BuildSourceEvent()
        );

        _sourceEventRepository
            .List(filters, Arg.Any<CancellationToken>())
            .Returns(sourceEvents);

        // Act
        var vResult = await _handler.ListSourceEvents(filters, CancellationToken.None);

        // Assert
        vResult.ShouldBeSuccess(foundEvents => foundEvents.Should().HaveCount(2));
    }
}