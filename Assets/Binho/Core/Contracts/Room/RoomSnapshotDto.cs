using System.Collections.Generic;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record RoomSnapshotDto(
    RoomId RoomId,
    RoomCode RoomCode,
    RoomPhase RoomPhase,
    RoomVersion RoomVersion,
    MatchId? ActiveMatchId,
    MatchInstance? ActiveMatchInstance,
    IReadOnlyList<SeatStateDto> Seats,
    RoomRetentionDto? Retention,
    PauseContextDto? PauseContext,
    RematchStateDto? Rematch,
    IReadOnlyList<TimerRecordDto> Timers);
