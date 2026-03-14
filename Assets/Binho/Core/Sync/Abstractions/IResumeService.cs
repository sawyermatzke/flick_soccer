using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Sync;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IResumeService
{
    Result<RoomSnapshotDto> Resume<TPayload>(CommandEnvelopeDto<TPayload> command);
    Result Acknowledge(AcknowledgedWatermarkDto watermark, string? resumeToken);
}
