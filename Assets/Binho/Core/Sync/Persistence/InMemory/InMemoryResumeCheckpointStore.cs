using System.Collections.Generic;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Persistence.InMemory;

public sealed class InMemoryResumeCheckpointStore : IResumeCheckpointStore
{
    private readonly Dictionary<string, SeatResumeCheckpointDto> _records = new();
    public void Save(SeatResumeCheckpointDto checkpoint) => _records[Key(checkpoint.RoomId, checkpoint.Seat)] = checkpoint;
    public SeatResumeCheckpointDto? Get(RoomId roomId, SeatId seat) => _records.TryGetValue(Key(roomId, seat), out var checkpoint) ? checkpoint : null;
    private static string Key(RoomId roomId, SeatId seat) => $"{roomId.Value}:{seat.Value}";
}
