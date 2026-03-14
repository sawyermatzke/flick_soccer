using System;
using NUnit.Framework;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;
using Binho.Core.Sync.Resume;

namespace Binho.Core.Sync.Tests;

public sealed class StaleCommandPolicyTests
{
    [Test]
    public void Evaluate_WhenGameplayCommandWatermarkOlderThanCheckpoint_RejectsAsStale()
    {
        var command = new CommandEnvelopeDto<object>(new CommandId("cmd_001"), "submitShotIntent", new RoomId("room_123"), new MatchId("match_123"), new SeatId("north"), new ClientWatermarkDto(new StateVersion(18), new EventSequence(124), new SnapshotId("snap_018")), new { }, DateTimeOffset.UtcNow);
        Assert.That(StaleCommandPolicy.Evaluate(command, BuildCheckpoint(resumeRequired: false)), Is.EqualTo(StaleCommandDecision.RejectStale));
    }

    [Test]
    public void Evaluate_WhenGameplayCommandMissingWatermarkAndCheckpointExists_Rejects()
    {
        var command = new CommandEnvelopeDto<object>(new CommandId("cmd_001"), "submitShotIntent", new RoomId("room_123"), new MatchId("match_123"), new SeatId("north"), null, new { }, DateTimeOffset.UtcNow);
        Assert.That(StaleCommandPolicy.Evaluate(command, BuildCheckpoint(resumeRequired: false)), Is.EqualTo(StaleCommandDecision.RejectMissingWatermark));
    }

    [Test]
    public void Evaluate_WhenResumeSessionCommandMissingWatermark_AllowsResumeFlow()
    {
        var command = new CommandEnvelopeDto<object>(new CommandId("cmd_001"), "resumeSession", new RoomId("room_123"), null, new SeatId("north"), null, new { }, DateTimeOffset.UtcNow);
        Assert.That(StaleCommandPolicy.Evaluate(command, BuildCheckpoint(resumeRequired: true)), Is.EqualTo(StaleCommandDecision.Accept));
    }

    private static SeatResumeCheckpointDto BuildCheckpoint(bool resumeRequired) => new(new RoomId("room_123"), new SeatId("north"), new PlayerId("player_a"), new RoomVersion(42), new MatchId("match_123"), new MatchInstance(1), new StateVersion(19), new EventSequence(128), new SnapshotId("snap_019"), resumeRequired, DateTimeOffset.UtcNow);
}
