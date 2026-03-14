using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Persistence;

public sealed record EventLogRecordDto(
    RoomId RoomId,
    MatchId? MatchId,
    MatchInstance? MatchInstance,
    EventId EventId,
    EventSequence? Sequence,
    RoomVersion RoomVersionAtEmit,
    StateVersion? StateVersionAtEmit,
    ScopeKind Scope,
    string EventType,
    CommandId? CausationId,
    string PayloadJson,
    DateTimeOffset EmittedAtUtc);
