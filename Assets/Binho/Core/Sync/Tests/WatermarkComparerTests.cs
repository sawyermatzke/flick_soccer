using NUnit.Framework;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;
using Binho.Core.Sync.Resume;

namespace Binho.Core.Sync.Tests;

public sealed class WatermarkComparerTests
{
    [Test] public void Compare_WhenStateVersionAdvances_ReturnsNewer() => Assert.That(WatermarkComparer.Compare(new ClientWatermarkDto(new StateVersion(20), new EventSequence(128), new SnapshotId("snap_020")), new AcknowledgedWatermarkDto(new SnapshotId("snap_019"), null, new StateVersion(19), new EventSequence(128))), Is.EqualTo(WatermarkComparison.Newer));
    [Test] public void Compare_WhenStateVersionEqualAndEventSequenceAdvances_ReturnsNewer() => Assert.That(WatermarkComparer.Compare(new ClientWatermarkDto(new StateVersion(19), new EventSequence(129), new SnapshotId("snap_020")), new AcknowledgedWatermarkDto(new SnapshotId("snap_019"), null, new StateVersion(19), new EventSequence(128))), Is.EqualTo(WatermarkComparison.Newer));
    [Test] public void Compare_WhenWatermarksEqual_ReturnsEqual() => Assert.That(WatermarkComparer.Compare(new ClientWatermarkDto(new StateVersion(19), new EventSequence(128), new SnapshotId("snap_019")), new AcknowledgedWatermarkDto(new SnapshotId("snap_019"), null, new StateVersion(19), new EventSequence(128))), Is.EqualTo(WatermarkComparison.Equal));
    [Test] public void Compare_WhenOnlySnapshotReferenceDiffersAtSameVersions_ReturnsEquivalentWatermark() => Assert.That(WatermarkComparer.Compare(new ClientWatermarkDto(new StateVersion(19), new EventSequence(128), new SnapshotId("snap_alt")), new AcknowledgedWatermarkDto(new SnapshotId("snap_019"), null, new StateVersion(19), new EventSequence(128))), Is.EqualTo(WatermarkComparison.Equivalent));
}
