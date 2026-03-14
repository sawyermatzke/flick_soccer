using System;
using NUnit.Framework;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Tests;

public sealed class PersistenceRecordSerializationTests
{
    [Test]
    public void SeatResumeCheckpoint_RoundTripSerialize_PreservesAllWatermarkFields()
    {
        var checkpoint = new SeatResumeCheckpointDto(new RoomId("room_123"), new SeatId("north"), new PlayerId("player_a"), new RoomVersion(42), new MatchId("match_123"), new MatchInstance(1), new StateVersion(19), new EventSequence(128), new SnapshotId("snap_019"), false, DateTimeOffset.Parse("2026-03-14T03:36:05Z"));
        var roundTrip = TestJson.RoundTrip(checkpoint);
        Assert.That(roundTrip.LastAckedSnapshotId?.Value, Is.EqualTo("snap_019"));
        Assert.That(roundTrip.LastAckedStateVersion?.Value, Is.EqualTo(19));
    }

    [Test]
    public void TimerRecord_RoundTripSerialize_PreservesScopeAndNullableMatchFields()
    {
        var timer = new TimerRecordDto(new TimerId("timer_disconnect_north"), null, null, TimerRecordScope.Room, "disconnectGrace", new SeatId("north"), TimerStatus.Running, DateTimeOffset.Parse("2026-03-14T03:34:00Z"), DateTimeOffset.Parse("2026-03-14T03:39:00Z"), "commitAbandonmentForfeit", null);
        var roundTrip = TestJson.RoundTrip(timer);
        Assert.That(roundTrip.Scope, Is.EqualTo(TimerRecordScope.Room));
        Assert.That(roundTrip.MatchId, Is.Null);
    }
}
