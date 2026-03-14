using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Match;

public sealed record SemanticEntityStateDto(
    string EntityId,
    string EntityClass,
    SideId? Team,
    EntityAvailabilityState Availability,
    string? TopologyRef,
    string? GeometryRef,
    ResolutionState ResolutionState);
