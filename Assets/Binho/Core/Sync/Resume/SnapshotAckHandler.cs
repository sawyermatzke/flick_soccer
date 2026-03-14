using System;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Resume;

public static class SnapshotAckHandler
{
    public static Result<SeatResumeCheckpointDto> Handle(SeatResumeCheckpointDto checkpoint, AcknowledgedWatermarkDto acknowledgedWatermark, MatchId? matchId, MatchInstance? matchInstance, IClock clock)
    {
        var current = new AcknowledgedWatermarkDto(
            checkpoint.LastAckedSnapshotId ?? default,
            checkpoint.LastAckedRoomVersion,
            checkpoint.LastAckedStateVersion,
            checkpoint.LastAckedEventSequence);

        var comparison = WatermarkComparer.Compare(
            new ClientWatermarkDto(acknowledgedWatermark.StateVersion, acknowledgedWatermark.EventSequence, acknowledgedWatermark.SnapshotId),
            current);

        if (comparison == WatermarkComparison.Older)
        {
            return Result<SeatResumeCheckpointDto>.Success(checkpoint with { ResumeRequired = true });
        }

        if (comparison == WatermarkComparison.Equal)
        {
            return Result<SeatResumeCheckpointDto>.Success(checkpoint with { ResumeRequired = false });
        }

        return Result<SeatResumeCheckpointDto>.Success(checkpoint with
        {
            LastAckedRoomVersion = acknowledgedWatermark.RoomVersion,
            LastAckedMatchId = matchId,
            LastAckedMatchInstance = matchInstance,
            LastAckedStateVersion = acknowledgedWatermark.StateVersion,
            LastAckedEventSequence = acknowledgedWatermark.EventSequence,
            LastAckedSnapshotId = acknowledgedWatermark.SnapshotId,
            ResumeRequired = false,
            UpdatedAtUtc = clock.UtcNow
        });
    }
}
