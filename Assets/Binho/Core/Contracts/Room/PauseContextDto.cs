using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record PauseContextDto(string Reason, SeatId? RequiredSeat, DateTimeOffset? GraceDeadlineUtc, string? AbandonmentOutcome);
