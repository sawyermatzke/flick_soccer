using System.Collections.Generic;
using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Match;

public sealed record MatchSnapshotMutation(
    ScoreStateDto? Score = null,
    TurnStateDto? Turn = null,
    PendingRestartDto? PendingRestart = null,
    IReadOnlyList<SemanticEntityStateDto>? Entities = null,
    MatchTerminalOutcomeDto? TerminalOutcome = null,
    StateVersion? StateVersion = null,
    EventSequence? LastCommittedEventSequence = null,
    MatchPhase? Phase = null);

public static class MatchSnapshotBuilder
{
    public static MatchSnapshotDto Apply(MatchSnapshotDto snapshot, MatchSnapshotMutation mutation) => snapshot with
    {
        Score = mutation.Score ?? snapshot.Score,
        Turn = mutation.Turn ?? snapshot.Turn,
        PendingRestart = mutation.PendingRestart ?? snapshot.PendingRestart,
        Entities = mutation.Entities ?? snapshot.Entities,
        TerminalOutcome = mutation.TerminalOutcome ?? snapshot.TerminalOutcome,
        StateVersion = mutation.StateVersion ?? snapshot.StateVersion,
        LastCommittedEventSequence = mutation.LastCommittedEventSequence ?? snapshot.LastCommittedEventSequence,
        Phase = mutation.Phase ?? snapshot.Phase
    };
}
