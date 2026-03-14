using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record DomainEventEnvelopeDto<TPayload>(
    EventId EventId,
    string EventType,
    ScopeKind Scope,
    RoomId RoomId,
    MatchId? MatchId,
    MatchInstance? MatchInstance,
    EventSequence? Sequence,
    RoomVersion RoomVersionAtEmit,
    StateVersion? StateVersionAtEmit,
    CommandId? CausationId,
    TPayload Payload,
    DateTimeOffset EmittedAtUtc);
