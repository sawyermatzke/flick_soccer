using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record ReplayWindowDto(EventSequence AvailableFromSequence, EventSequence? AvailableToSequence, SnapshotId? SnapshotId);
