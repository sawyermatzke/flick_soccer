using System.Collections.Generic;
using System.Linq;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Persistence.InMemory;

public sealed class InMemoryTimerStore : ITimerStore
{
    private readonly Dictionary<RoomId, List<TimerRecordDto>> _records = new();
    public void Save(RoomId roomId, TimerRecordDto timer)
    {
        if (_records.TryGetValue(roomId, out var timers) == false)
        {
            timers = [];
            _records[roomId] = timers;
        }

        var index = timers.FindIndex(x => x.TimerId.Equals(timer.TimerId));
        if (index >= 0) timers[index] = timer; else timers.Add(timer);
    }

    public IReadOnlyList<TimerRecordDto> GetByRoom(RoomId roomId) => _records.TryGetValue(roomId, out var timers) ? timers.ToList() : [];
}
