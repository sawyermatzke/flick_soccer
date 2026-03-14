using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record SnapshotReferenceDto(SnapshotId SnapshotId, RoomId RoomId, MatchId? MatchId, MatchInstance? MatchInstance, StateVersion? StateVersion, RoomVersion? RoomVersion);
