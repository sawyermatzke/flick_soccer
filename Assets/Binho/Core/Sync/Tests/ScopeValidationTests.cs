using System;
using NUnit.Framework;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;
using Binho.Core.Sync.Validation;

namespace Binho.Core.Sync.Tests;

public sealed class ScopeValidationTests
{
    [Test]
    public void DomainEventValidator_WhenRoomScopeAndMatchFieldsNull_Accepts()
    {
        var evt = new DomainEventEnvelopeDto<object>(new EventId("evt_room_001"), "disconnectPauseStarted", ScopeKind.Room, new RoomId("room_123"), null, null, null, new RoomVersion(42), null, null, new { }, DateTimeOffset.UtcNow);
        Assert.That(DomainEventScopeValidator.Validate(evt).IsSuccess, Is.True);
    }

    [Test]
    public void DomainEventValidator_WhenMatchScopeMissingMatchId_Rejects()
    {
        var evt = new DomainEventEnvelopeDto<object>(new EventId("evt_match_001"), "restartAwarded", ScopeKind.Match, new RoomId("room_123"), null, new MatchInstance(1), new EventSequence(128), new RoomVersion(42), new StateVersion(19), null, new { }, DateTimeOffset.UtcNow);
        Assert.That(DomainEventScopeValidator.Validate(evt).IsSuccess, Is.False);
    }

    [Test]
    public void DomainEventValidator_WhenMatchScopeMissingMatchInstance_Rejects()
    {
        var evt = new DomainEventEnvelopeDto<object>(new EventId("evt_match_001"), "restartAwarded", ScopeKind.Match, new RoomId("room_123"), new MatchId("match_123"), null, new EventSequence(128), new RoomVersion(42), new StateVersion(19), null, new { }, DateTimeOffset.UtcNow);
        Assert.That(DomainEventScopeValidator.Validate(evt).IsSuccess, Is.False);
    }

    [Test]
    public void DomainEventValidator_WhenMatchScopeMissingStateVersion_Rejects()
    {
        var evt = new DomainEventEnvelopeDto<object>(new EventId("evt_match_001"), "restartAwarded", ScopeKind.Match, new RoomId("room_123"), new MatchId("match_123"), new MatchInstance(1), new EventSequence(128), new RoomVersion(42), null, null, new { }, DateTimeOffset.UtcNow);
        Assert.That(DomainEventScopeValidator.Validate(evt).IsSuccess, Is.False);
    }

    [Test]
    public void CommandValidator_WhenRoomOnlyCommandOmitsMatchId_Accepts()
    {
        var command = new CommandEnvelopeDto<object>(new CommandId("cmd_001"), "acknowledgeSnapshot", new RoomId("room_123"), null, new SeatId("north"), null, new { }, DateTimeOffset.UtcNow);
        Assert.That(CommandScopeValidator.Validate(command).IsSuccess, Is.True);
    }

    [Test]
    public void CommandValidator_WhenGameplayCommandOmitsMatchId_Rejects()
    {
        var command = new CommandEnvelopeDto<object>(new CommandId("cmd_001"), "submitShotIntent", new RoomId("room_123"), null, new SeatId("north"), null, new { }, DateTimeOffset.UtcNow);
        Assert.That(CommandScopeValidator.Validate(command).IsSuccess, Is.False);
    }
}
