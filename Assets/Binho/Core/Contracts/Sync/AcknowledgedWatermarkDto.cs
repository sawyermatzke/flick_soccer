using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record AcknowledgedWatermarkDto(SnapshotId SnapshotId, RoomVersion? RoomVersion, StateVersion? StateVersion, EventSequence? EventSequence);
