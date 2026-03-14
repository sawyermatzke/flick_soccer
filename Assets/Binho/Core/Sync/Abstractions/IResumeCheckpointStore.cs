using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IResumeCheckpointStore
{
    void Save(SeatResumeCheckpointDto checkpoint);
    SeatResumeCheckpointDto? Get(RoomId roomId, SeatId seat);
}
