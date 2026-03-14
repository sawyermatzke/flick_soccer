using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record ClientWatermarkDto(StateVersion? LastKnownStateVersion, EventSequence? LastKnownEventSequence, SnapshotId? LastSnapshotId);
