using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record SeatResumeStateDto(
    RoomVersion? LastAckedRoomVersion,
    MatchId? LastAckedMatchId,
    MatchInstance? LastAckedMatchInstance,
    StateVersion? LastAckedStateVersion,
    EventSequence? LastAckedEventSequence,
    SnapshotId? LastAckedSnapshotId,
    bool ResumeRequired,
    string? ResumeToken);
