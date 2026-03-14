using NUnit.Framework;
using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Tests;

public sealed class MatchSnapshotSerializationTests
{
    [Test]
    public void MatchSnapshot_RoundTripSerialize_AllowsNullGeometryRef()
    {
        var snapshot = BuildSnapshot();
        var roundTrip = TestJson.RoundTrip(snapshot);
        Assert.That(roundTrip.Entities[0].GeometryRef, Is.Null);
    }

    [Test]
    public void MatchSnapshot_RoundTripSerialize_PreservesPendingRestartSemanticRefs()
    {
        var snapshot = BuildSnapshot();
        var roundTrip = TestJson.RoundTrip(snapshot);
        Assert.That(roundTrip.PendingRestart?.OriginRef, Is.EqualTo("north_penalty_region"));
        Assert.That(roundTrip.PendingRestart?.SourceBasis, Is.EqualTo(EvidenceStatus.Confirmed));
    }

    [Test]
    public void MatchSnapshot_RoundTripSerialize_PreservesTerminalOutcomeWhenPresent()
    {
        var snapshot = BuildSnapshot() with { TerminalOutcome = new MatchTerminalOutcomeDto("normalWin", new SeatId("north"), new SeatId("south"), new ScoreStateDto(7, 5), null) };
        var roundTrip = TestJson.RoundTrip(snapshot);
        Assert.That(roundTrip.TerminalOutcome?.TerminalReason, Is.EqualTo("normalWin"));
    }

    private static MatchSnapshotDto BuildSnapshot() => new(
        new MatchId("match_123"), new MatchInstance(1), MatchPhase.RestartPending, new StateVersion(19), new EventSequence(128),
        new ScoreStateDto(2, 3), new TurnStateDto(SideId.North, 7),
        new PendingRestartDto("penaltyRestart", SideId.North, "north_penalty_region", ResolutionState.Provisional, EvidenceStatus.Confirmed),
        [new SemanticEntityStateDto("north_piece_01", "player", SideId.North, EntityAvailabilityState.Active, "north_defensive_group_a", null, ResolutionState.Unresolved)],
        null,
        GeometryReadiness.TopologyReady);
}
