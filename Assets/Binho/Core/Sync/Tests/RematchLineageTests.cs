using NUnit.Framework;
using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Match;

namespace Binho.Core.Sync.Tests;

public sealed class RematchLineageTests
{
    [Test]
    public void RematchTransition_PreservesRoomIdentityAndSeatOwnership()
    {
        Assert.That(new RoomId("room_123").Value, Is.EqualTo("room_123"));
        Assert.That(new SeatId("north").Value, Is.EqualTo("north"));
    }

    [Test]
    public void RematchTransition_CreatesFreshMatchInstanceBoundary()
    {
        Assert.That(new MatchInstance(2).Value, Is.GreaterThan(new MatchInstance(1).Value));
    }

    [Test]
    public void RematchTransition_ResetsMatchScopedWatermarks()
    {
        var next = new MatchSnapshotDto(new MatchId("match_124"), new MatchInstance(2), MatchPhase.ActiveTurnPendingInput, new StateVersion(1), new EventSequence(1), new ScoreStateDto(0, 0), new TurnStateDto(SideId.North, 1), null, [], null, GeometryReadiness.TopologyReady);
        Assert.That(next.StateVersion.Value, Is.EqualTo(1));
        Assert.That(next.LastCommittedEventSequence.Value, Is.EqualTo(1));
    }

    [Test]
    public void RematchTransition_DoesNotReusePriorMatchEventStreamAsActiveHistory()
    {
        var prior = new MatchSnapshotDto(new MatchId("match_123"), new MatchInstance(1), MatchPhase.MatchEnded, new StateVersion(19), new EventSequence(128), new ScoreStateDto(7, 5), new TurnStateDto(SideId.North, 9), null, [], null, GeometryReadiness.TopologyReady);
        var next = new MatchSnapshotDto(new MatchId("match_124"), new MatchInstance(2), MatchPhase.ActiveTurnPendingInput, new StateVersion(1), new EventSequence(1), new ScoreStateDto(0, 0), new TurnStateDto(SideId.South, 1), null, [], null, GeometryReadiness.TopologyReady);
        Assert.That(next.MatchId, Is.Not.EqualTo(prior.MatchId));
        Assert.That(next.LastCommittedEventSequence.Value, Is.LessThan(prior.LastCommittedEventSequence.Value));
    }
}
