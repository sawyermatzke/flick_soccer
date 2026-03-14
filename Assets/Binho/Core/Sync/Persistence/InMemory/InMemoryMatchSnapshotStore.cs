using System.Collections.Generic;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Persistence.InMemory;

public sealed class InMemoryMatchSnapshotStore : IMatchSnapshotStore
{
    private readonly Dictionary<string, MatchSnapshotRecordDto> _records = new();
    public void Save(MatchSnapshotRecordDto snapshot) => _records[Key(snapshot.RoomId, snapshot.MatchId, snapshot.MatchInstance)] = snapshot;
    public MatchSnapshotRecordDto? Get(RoomId roomId, MatchId matchId, MatchInstance matchInstance) => _records.TryGetValue(Key(roomId, matchId, matchInstance), out var snapshot) ? snapshot : null;
    private static string Key(RoomId roomId, MatchId matchId, MatchInstance matchInstance) => $"{roomId.Value}:{matchId.Value}:{matchInstance.Value}";
}
