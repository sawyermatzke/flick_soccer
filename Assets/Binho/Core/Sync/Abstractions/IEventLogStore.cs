using System.Collections.Generic;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IEventLogStore
{
    void Append(EventLogRecordDto record);
    IReadOnlyList<EventLogRecordDto> GetRoomEvents(RoomId roomId);
    IReadOnlyList<EventLogRecordDto> GetMatchEvents(RoomId roomId, MatchId matchId, MatchInstance matchInstance);
}
