using FluentAssertions;
using LanguageExt;
using NSubstitute;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Core.SourceEvents.UseCases.RegisterSourceEvent;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Common.Primitives.EnumTypes;
using PocketLedger.Domain.Common.Primitives.StringTypes;
using PocketLedger.Domain.Entities;
using static LanguageExt.Prelude;
using static PocketLedger.Domain.Common.Prelude;

namespace PocketLedger.Core.UnitTests.SourceEvents.RegisterSourceEvent;

public sealed class RegisterSourceEventHandlerTests
{
    [Fact]
    public async Task Handle_GivenValidCommand_AddsSourceEventAndReturnsCreatedId()
    {
        //Arrange
        var repository = Substitute.For<ISourceEventRepository>();
        var handler = new RegisterSourceEventHandler(repository);

        var command = BuildCommand();

        var persistedSourceEvent = SourceEvent.Create(
            command.SourceType,
            command.RawPayload,
            command.ReceivedAt,
            None,
            command.ExternalId);

        repository
            .Add(Arg.Any<SourceEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(
                Success<Error, SourceEvent>(persistedSourceEvent)));

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        await repository.Received(1)
            .Add(Arg.Any<SourceEvent>(), Arg.Any<CancellationToken>());

        result.Match(
            success => success.SourceEventId.Should().Be(persistedSourceEvent.Id),
            error => error.Should().BeNull("the handler should return a successful validation"));
    }

    [Fact]
    public async Task Handle_GivenRepositoryFailure_ReturnsTheError()
    {
        //Arrange
        var repository = Substitute.For<ISourceEventRepository>();
        var handler = new RegisterSourceEventHandler(repository);

        var command = BuildCommand();
        var expectedError = new UnexpectedError("Could not persist source event.");

        repository
            .Add(Arg.Any<SourceEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(
                Fail<Error, SourceEvent>(expectedError)));

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        await repository.Received(1)
            .Add(Arg.Any<SourceEvent>(), Arg.Any<CancellationToken>());

        result.Match(
            _ => throw new Exception("Expected failure but got success."),
            error => error.Should().Contain(expectedError));
    }

    private static RegisterSourceEventCommand BuildCommand() =>
        new(
            SourceType: BuildSourceType(),
            RawPayload: BuildRawPayload(),
            ExternalId: BuildExternalId(),
            ReceivedAt: DateTimeOffset.UtcNow);

    private static SourceEventType BuildSourceType() => SourceEventType.Wallet;
    private static RawPayload BuildRawPayload() => RawPayload("test payload");
    private static Option<ExternalId> BuildExternalId() => None;
}