using System.Collections.Generic;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Persistence.InMemory;

public sealed class InMemoryRoomSnapshotStore : IRoomSnapshotStore
{
    private readonly Dictionary<RoomId, RoomSnapshotRecordDto> _records = new();
    public void Save(RoomSnapshotRecordDto snapshot) => _records[snapshot.RoomId] = snapshot;
    public RoomSnapshotRecordDto? Get(RoomId roomId) => _records.TryGetValue(roomId, out var snapshot) ? snapshot : null;
}
