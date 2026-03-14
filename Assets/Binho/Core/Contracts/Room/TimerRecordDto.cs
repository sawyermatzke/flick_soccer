using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record TimerRecordDto(
    TimerId TimerId,
    MatchId? MatchId,
    MatchInstance? MatchInstance,
    TimerRecordScope Scope,
    string TimerType,
    SeatId? Seat,
    TimerStatus Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset DeadlineUtc,
    string? ExpiryAction,
    string? CancelReason);
