using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Match;

public sealed record PendingRestartDto(string RestartType, SideId BeneficiarySide, string OriginRef, ResolutionState ResolutionState, EvidenceStatus SourceBasis);
