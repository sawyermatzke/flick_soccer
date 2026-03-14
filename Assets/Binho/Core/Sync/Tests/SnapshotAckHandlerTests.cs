using System;
using NUnit.Framework;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;
using Binho.Core.Sync.Resume;

namespace Binho.Core.Sync.Tests;

public sealed class SnapshotAckHandlerTests
{
    [Test]
    public void HandleAck_WhenSameSnapshotAckedTwice_IsIdempotent()
    {
        var checkpoint = BuildCheckpoint();
        var clock = new TestClock(DateTimeOffset.Parse("2026-03-14T03:36:05Z"));
        var ack = new AcknowledgedWatermarkDto(new SnapshotId("snap_019"), new RoomVersion(42), new StateVersion(19), new EventSequence(128));
        var result = SnapshotAckHandler.Handle(checkpoint, ack, new MatchId("match_123"), new MatchInstance(1), clock);
        Assert.That(result.Value?.ResumeRequired, Is.False);
        Assert.That(result.Value?.LastAckedSnapshotId?.Value, Is.EqualTo("snap_019"));
    }

    [Test]
    public void HandleAck_WhenOlderSnapshotAckArrivesAfterNewer_KeepsResumeRequired()
    {
        var checkpoint = BuildCheckpoint() with { LastAckedSnapshotId = new SnapshotId("snap_020"), LastAckedStateVersion = new StateVersion(20), LastAckedEventSequence = new EventSequence(129) };
        var clock = new TestClock(DateTimeOffset.Parse("2026-03-14T03:36:05Z"));
        var ack = new AcknowledgedWatermarkDto(new SnapshotId("snap_019"), new RoomVersion(42), new StateVersion(19), new EventSequence(128));
        var result = SnapshotAckHandler.Handle(checkpoint, ack, new MatchId("match_123"), new MatchInstance(1), clock);
        Assert.That(result.Value?.ResumeRequired, Is.True);
        Assert.That(result.Value?.LastAckedSnapshotId?.Value, Is.EqualTo("snap_020"));
    }

    private static SeatResumeCheckpointDto BuildCheckpoint() => new(new RoomId("room_123"), new SeatId("north"), new PlayerId("player_a"), new RoomVersion(42), new MatchId("match_123"), new MatchInstance(1), new StateVersion(19), new EventSequence(128), new SnapshotId("snap_019"), true, DateTimeOffset.Parse("2026-03-14T03:35:00Z"));
}
