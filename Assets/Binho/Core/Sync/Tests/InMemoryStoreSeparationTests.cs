using System;
using NUnit.Framework;
using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Persistence.InMemory;

namespace Binho.Core.Sync.Tests;

public sealed class InMemoryStoreSeparationTests
{
    [Test]
    public void RoomSnapshotStore_Save_DoesNotOverwriteMatchSnapshot()
    {
        var roomStore = new InMemoryRoomSnapshotStore();
        var matchStore = new InMemoryMatchSnapshotStore();
        roomStore.Save(new RoomSnapshotRecordDto(new RoomId("room_123"), new RoomVersion(42), RoomPhase.PausedForDisconnect, new MatchId("match_123"), new MatchInstance(1), new RoomSnapshotDto(new RoomId("room_123"), new RoomCode("ABCD12"), RoomPhase.PausedForDisconnect, new RoomVersion(42), new MatchId("match_123"), new MatchInstance(1), [], null, null, null, []), DateTimeOffset.UtcNow));
        matchStore.Save(new MatchSnapshotRecordDto(new RoomId("room_123"), new MatchId("match_123"), new MatchInstance(1), new StateVersion(19), new EventSequence(128), new MatchSnapshotDto(new MatchId("match_123"), new MatchInstance(1), MatchPhase.RestartPending, new StateVersion(19), new EventSequence(128), new ScoreStateDto(2, 3), new TurnStateDto(SideId.North, 7), null, [], null, GeometryReadiness.TopologyReady), DateTimeOffset.UtcNow));
        Assert.That(roomStore.Get(new RoomId("room_123"))?.RoomVersion.Value, Is.EqualTo(42));
        Assert.That(matchStore.Get(new RoomId("room_123"), new MatchId("match_123"), new MatchInstance(1))?.StateVersion.Value, Is.EqualTo(19));
    }

    [Test]
    public void MatchSnapshotStore_Save_DoesNotMutateRoomRetentionState()
    {
        var roomStore = new InMemoryRoomSnapshotStore();
        var roomRecord = new RoomSnapshotRecordDto(new RoomId("room_123"), new RoomVersion(42), RoomPhase.MatchEnded, new MatchId("match_123"), new MatchInstance(1), new RoomSnapshotDto(new RoomId("room_123"), new RoomCode("ABCD12"), RoomPhase.MatchEnded, new RoomVersion(42), new MatchId("match_123"), new MatchInstance(1), [], new RoomRetentionDto(RetentionStatus.Retained, DateTimeOffset.Parse("2026-03-14T03:52:00Z")), null, null, []), DateTimeOffset.UtcNow);
        roomStore.Save(roomRecord);
        var matchStore = new InMemoryMatchSnapshotStore();
        matchStore.Save(new MatchSnapshotRecordDto(new RoomId("room_123"), new MatchId("match_124"), new MatchInstance(2), new StateVersion(1), new EventSequence(1), new MatchSnapshotDto(new MatchId("match_124"), new MatchInstance(2), MatchPhase.ActiveTurnPendingInput, new StateVersion(1), new EventSequence(1), new ScoreStateDto(0, 0), new TurnStateDto(SideId.North, 1), null, [], null, GeometryReadiness.TopologyReady), DateTimeOffset.UtcNow));
        Assert.That(roomStore.Get(new RoomId("room_123"))?.Snapshot.Retention?.Status, Is.EqualTo(RetentionStatus.Retained));
    }

    [Test]
    public void EventLogStore_AppendRoomScope_PreservesNullMatchColumns()
    {
        var store = new InMemoryEventLogStore();
        store.Append(new EventLogRecordDto(new RoomId("room_123"), null, null, new EventId("evt_room_001"), null, new RoomVersion(42), null, ScopeKind.Room, "disconnectPauseStarted", null, "{}", DateTimeOffset.UtcNow));
        Assert.That(store.GetRoomEvents(new RoomId("room_123"))[0].MatchId, Is.Null);
    }

    [Test]
    public void EventLogStore_AppendMatchScope_RequiresMatchAttribution()
    {
        var store = new InMemoryEventLogStore();
        Assert.Throws<InvalidOperationException>(() => store.Append(new EventLogRecordDto(new RoomId("room_123"), null, null, new EventId("evt_match_001"), new EventSequence(1), new RoomVersion(42), new StateVersion(1), ScopeKind.Match, "matchStarted", null, "{}", DateTimeOffset.UtcNow)));
    }

    [Test]
    public void TimerStore_Save_PreservesDistinctRoomAndMatchScopeRecords()
    {
        var store = new InMemoryTimerStore();
        store.Save(new RoomId("room_123"), new TimerRecordDto(new TimerId("timer_room"), null, null, TimerRecordScope.Room, "disconnectGrace", new SeatId("north"), TimerStatus.Running, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(1), "commitAbandonmentForfeit", null));
        store.Save(new RoomId("room_123"), new TimerRecordDto(new TimerId("timer_match"), new MatchId("match_123"), new MatchInstance(1), TimerRecordScope.Match, "restartWindow", new SeatId("north"), TimerStatus.Running, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(1), null, null));
        Assert.That(store.GetByRoom(new RoomId("room_123")).Count, Is.EqualTo(2));
    }
}
