using System;
using NUnit.Framework;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Tests;

public sealed class RoomSnapshotSerializationTests
{
    [Test]
    public void RoomSnapshot_RoundTripSerialize_AllowsNullActiveMatchForPreMatchRoom()
    {
        var snapshot = BuildSnapshot(includeActiveMatch: false);
        var roundTrip = TestJson.RoundTrip(snapshot);
        Assert.That(roundTrip.ActiveMatchId, Is.Null);
        Assert.That(roundTrip.ActiveMatchInstance, Is.Null);
    }

    [Test]
    public void RoomSnapshot_RoundTripSerialize_PreservesSeatResumeWatermarks()
    {
        var snapshot = BuildSnapshot();
        var roundTrip = TestJson.RoundTrip(snapshot);
        Assert.That(roundTrip.Seats[0].ResumeState?.LastAckedEventSequence?.Value, Is.EqualTo(124));
        Assert.That(roundTrip.Seats[0].ResumeState?.LastAckedSnapshotId?.Value, Is.EqualTo("snap_018"));
    }

    [Test]
    public void RoomSnapshot_RoundTripSerialize_PreservesPauseAndTimerData()
    {
        var snapshot = BuildSnapshot();
        var roundTrip = TestJson.RoundTrip(snapshot);
        Assert.That(roundTrip.PauseContext?.Reason, Is.EqualTo("seatDisconnected"));
        Assert.That(roundTrip.Timers[0].Scope, Is.EqualTo(TimerRecordScope.Room));
    }

    private static RoomSnapshotDto BuildSnapshot(bool includeActiveMatch = true) => new(
        new RoomId("room_123"),
        new RoomCode("ABCD12"),
        RoomPhase.PausedForDisconnect,
        new RoomVersion(42),
        includeActiveMatch ? new MatchId("match_123") : null,
        includeActiveMatch ? new MatchInstance(1) : null,
        [
            new SeatStateDto(new SeatId("north"), new PlayerId("player_a"), OccupancyState.Joined, SeatPresenceState.Disconnected,
                new SeatResumeStateDto(new RoomVersion(41), new MatchId("match_123"), new MatchInstance(1), new StateVersion(18), new EventSequence(124), new SnapshotId("snap_018"), true, "resume_tok_001"))
        ],
        new RoomRetentionDto(RetentionStatus.Retained, DateTimeOffset.Parse("2026-03-14T03:52:00Z")),
        new PauseContextDto("seatDisconnected", new SeatId("north"), DateTimeOffset.Parse("2026-03-14T03:39:00Z"), "forfeitLoss"),
        new RematchStateDto(RematchStatus.NotAvailable, []),
        [new TimerRecordDto(new TimerId("timer_disconnect_north"), null, null, TimerRecordScope.Room, "disconnectGrace", new SeatId("north"), TimerStatus.Running, DateTimeOffset.Parse("2026-03-14T03:34:00Z"), DateTimeOffset.Parse("2026-03-14T03:39:00Z"), "commitAbandonmentForfeit", null)]);
}
