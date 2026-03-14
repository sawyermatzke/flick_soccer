using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Match;

public sealed record MatchTerminalOutcomeDto(string TerminalReason, SeatId? WinningSeat, SeatId? LosingSeat, ScoreStateDto? FinalScore, TimerId? TriggerTimerId);
