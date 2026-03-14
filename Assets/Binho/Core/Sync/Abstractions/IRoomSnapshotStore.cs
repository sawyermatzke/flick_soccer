using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IRoomSnapshotStore
{
    void Save(RoomSnapshotRecordDto snapshot);
    RoomSnapshotRecordDto? Get(RoomId roomId);
}
