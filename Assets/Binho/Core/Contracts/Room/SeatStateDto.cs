using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record SeatStateDto(
    SeatId Seat,
    PlayerId? PlayerId,
    OccupancyState OccupancyState,
    SeatPresenceState PresenceState,
    SeatResumeStateDto? ResumeState);
