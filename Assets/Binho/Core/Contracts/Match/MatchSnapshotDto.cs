using System.Collections.Generic;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Match;

public sealed record MatchSnapshotDto(
    MatchId MatchId,
    MatchInstance MatchInstance,
    MatchPhase Phase,
    StateVersion StateVersion,
    EventSequence LastCommittedEventSequence,
    ScoreStateDto Score,
    TurnStateDto Turn,
    PendingRestartDto? PendingRestart,
    IReadOnlyList<SemanticEntityStateDto> Entities,
    MatchTerminalOutcomeDto? TerminalOutcome,
    GeometryReadiness GeometryReadiness);
