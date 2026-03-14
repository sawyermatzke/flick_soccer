using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Persistence;

public sealed record SeatResumeCheckpointDto(
    RoomId RoomId,
    SeatId Seat,
    PlayerId? PlayerId,
    RoomVersion? LastAckedRoomVersion,
    MatchId? LastAckedMatchId,
    MatchInstance? LastAckedMatchInstance,
    StateVersion? LastAckedStateVersion,
    EventSequence? LastAckedEventSequence,
    SnapshotId? LastAckedSnapshotId,
    bool ResumeRequired,
    DateTimeOffset UpdatedAtUtc);
