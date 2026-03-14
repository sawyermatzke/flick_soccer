using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IMatchSnapshotStore
{
    void Save(MatchSnapshotRecordDto snapshot);
    MatchSnapshotRecordDto? Get(RoomId roomId, MatchId matchId, MatchInstance matchInstance);
}
