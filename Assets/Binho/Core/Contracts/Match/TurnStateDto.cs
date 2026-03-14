using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Match;

public sealed record TurnStateDto(SideId ActiveSide, int ShotIndex);
