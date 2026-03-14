using System.Collections.Generic;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;

namespace Binho.Core.Sync.Validation;

public static class DomainEventScopeValidator
{
    public static Result Validate<TPayload>(DomainEventEnvelopeDto<TPayload> envelope)
    {
        var errors = new List<string>();
        if (envelope.RoomId == default) errors.Add("RoomId is required.");
        if (envelope.EventId == default) errors.Add("EventId is required.");

        if (envelope.Scope == ScopeKind.Room)
        {
            if (envelope.MatchId is not null) errors.Add("Room-scoped event must not have MatchId.");
            if (envelope.MatchInstance is not null) errors.Add("Room-scoped event must not have MatchInstance.");
            if (envelope.StateVersionAtEmit is not null) errors.Add("Room-scoped event must not have StateVersionAtEmit.");
        }
        else
        {
            if (envelope.MatchId is null) errors.Add("Match-scoped event requires MatchId.");
            if (envelope.MatchInstance is null) errors.Add("Match-scoped event requires MatchInstance.");
            if (envelope.StateVersionAtEmit is null) errors.Add("Match-scoped event requires StateVersionAtEmit.");
            if (envelope.Sequence is null) errors.Add("Match-scoped event requires Sequence.");
        }

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
    }
}
