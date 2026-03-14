using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Sync;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IRoomLifecycleService
{
    Result<RoomSnapshotDto> HandleCommand<TPayload>(CommandEnvelopeDto<TPayload> command);
}
