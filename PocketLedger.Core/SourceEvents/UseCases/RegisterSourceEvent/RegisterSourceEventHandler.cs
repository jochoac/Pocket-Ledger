using LanguageExt;
using static LanguageExt.Prelude;
using PocketLedger.Core.SourceEvents.Ports;
using PocketLedger.Domain.Common.ErrorTypes;
using PocketLedger.Domain.Entities;

namespace PocketLedger.Core.SourceEvents.UseCases.RegisterSourceEvent;

public sealed class RegisterSourceEventHandler(ISourceEventRepository sourceEventRepository)
{
    public async Task<Validation<Error, RegisterSourceEventResult>> Handle(
        RegisterSourceEventCommand command,
        CancellationToken cancellationToken)
    {
        //TODO: SourceEvent.Create() might go wrong. Convert to Validation<Error, SourceEvent>
        Validation<Error, SourceEvent> sourceEventValidation =
            SourceEvent.Create(command.SourceType, command.RawPayload, command.ReceivedAt, None, command.ExternalId);

        return await sourceEventValidation.MatchAsync(
            async sourceEvent =>
            {
                var persistedValidation = await sourceEventRepository.Add(sourceEvent, cancellationToken);
                return persistedValidation.Map(persisted => new RegisterSourceEventResult(persisted.Id));
            },
            error => Task.FromResult(Fail<Error, RegisterSourceEventResult>(error))
        );
    }
}