using System.Collections.Generic;
using System.Linq;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Abstractions;
using Binho.Core.Sync.Validation;

namespace Binho.Core.Sync.Persistence.InMemory;

public sealed class InMemoryEventLogStore : IEventLogStore
{
    private readonly List<EventLogRecordDto> _records = new();

    public void Append(EventLogRecordDto record)
    {
        if (record.Scope == ScopeKind.Match && (record.MatchId is null || record.MatchInstance is null))
            throw new System.InvalidOperationException("Match-scoped event records require match attribution.");
        _records.Add(record);
    }

    public IReadOnlyList<EventLogRecordDto> GetRoomEvents(RoomId roomId) => _records.Where(x => x.RoomId.Equals(roomId)).ToList();
    public IReadOnlyList<EventLogRecordDto> GetMatchEvents(RoomId roomId, MatchId matchId, MatchInstance matchInstance) =>
        _records.Where(x => x.RoomId.Equals(roomId) && x.MatchId?.Equals(matchId) == true && x.MatchInstance?.Equals(matchInstance) == true).ToList();
}
