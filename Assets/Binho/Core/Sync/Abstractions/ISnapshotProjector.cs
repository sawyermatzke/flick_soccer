using System.Collections.Generic;
using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Sync;

namespace Binho.Core.Sync.Abstractions;

public interface ISnapshotProjector
{
    RoomSnapshotDto ProjectRoom(RoomSnapshotDto seed, IEnumerable<DomainEventEnvelopeDto<object>> events);
    MatchSnapshotDto ProjectMatch(MatchSnapshotDto seed, IEnumerable<DomainEventEnvelopeDto<object>> events);
}
