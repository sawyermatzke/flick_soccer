using System;
using NUnit.Framework;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;

namespace Binho.Core.Contracts.Tests;

public sealed class SyncEnvelopeSerializationTests
{
    [Test]
    public void CommandEnvelope_RoundTripSerialize_PreservesNullableMatchIdForRoomOnlyCommand()
    {
        var command = new CommandEnvelopeDto<object>(new CommandId("cmd_001"), "resumeSession", new RoomId("room_123"), null, new SeatId("north"), null, new { resumeToken = (string?)null }, DateTimeOffset.Parse("2026-03-14T03:36:03Z"));
        var roundTrip = TestJson.RoundTrip(command);
        Assert.That(roundTrip.MatchId, Is.Null);
    }

    [Test]
    public void DomainEventEnvelope_RoundTripSerialize_PreservesRoomScopeNullMatchAttribution()
    {
        var evt = new DomainEventEnvelopeDto<object>(new EventId("evt_room_001"), "disconnectPauseStarted", ScopeKind.Room, new RoomId("room_123"), null, null, null, new RoomVersion(42), null, null, new { requiredSeat = "north" }, DateTimeOffset.Parse("2026-03-14T03:34:00Z"));
        var roundTrip = TestJson.RoundTrip(evt);
        Assert.That(roundTrip.MatchId, Is.Null);
        Assert.That(roundTrip.Scope, Is.EqualTo(ScopeKind.Room));
    }

    [Test]
    public void DomainEventEnvelope_RoundTripSerialize_PreservesMatchScopeAttribution()
    {
        var evt = new DomainEventEnvelopeDto<object>(new EventId("evt_match_128"), "restartAwarded", ScopeKind.Match, new RoomId("room_123"), new MatchId("match_123"), new MatchInstance(1), new EventSequence(128), new RoomVersion(42), new StateVersion(19), new CommandId("cmd_000057"), new { restartType = "penaltyRestart" }, DateTimeOffset.Parse("2026-03-14T03:24:00Z"));
        var roundTrip = TestJson.RoundTrip(evt);
        Assert.That(roundTrip.MatchId?.Value, Is.EqualTo("match_123"));
        Assert.That(roundTrip.MatchInstance?.Value, Is.EqualTo(1));
    }
}
