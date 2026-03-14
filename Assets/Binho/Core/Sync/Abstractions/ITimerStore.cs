using System.Collections.Generic;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface ITimerStore
{
    void Save(RoomId roomId, TimerRecordDto timer);
    IReadOnlyList<TimerRecordDto> GetByRoom(RoomId roomId);
}
