using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record SyncStateDto(SnapshotId SnapshotId, RoomVersion? RoomVersion, StateVersion? StateVersion, EventSequence? LastCommittedEventSequence, bool ResumeRequiresAck);
