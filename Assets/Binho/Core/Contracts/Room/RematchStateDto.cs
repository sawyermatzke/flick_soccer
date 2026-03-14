using System.Collections.Generic;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record RematchStateDto(RematchStatus Status, IReadOnlyList<SeatId> RequestedBy);
