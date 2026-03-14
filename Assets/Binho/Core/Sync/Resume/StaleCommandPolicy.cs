using System;
using System.Collections.Generic;
using Binho.Core.Contracts.Persistence;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;

namespace Binho.Core.Sync.Resume;

public enum StaleCommandDecision
{
    Accept,
    RejectMissingWatermark,
    RejectResumeRequired,
    RejectStale
}

public static class StaleCommandPolicy
{
    private static readonly HashSet<string> ExemptCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "resumeSession", "acknowledgeSnapshot", "requestRematch", "declineRematch", "leaveRoom", "startMatch"
    };

    public static StaleCommandDecision Evaluate<TPayload>(CommandEnvelopeDto<TPayload> command, SeatResumeCheckpointDto? checkpoint)
    {
        if (checkpoint is null) return StaleCommandDecision.Accept;
        if (ExemptCommands.Contains(command.CommandType)) return StaleCommandDecision.Accept;
        if (checkpoint.ResumeRequired) return StaleCommandDecision.RejectResumeRequired;
        if (command.ClientWatermark is null) return StaleCommandDecision.RejectMissingWatermark;

        var baseline = new AcknowledgedWatermarkDto(
            checkpoint.LastAckedSnapshotId ?? default,
            checkpoint.LastAckedRoomVersion,
            checkpoint.LastAckedStateVersion,
            checkpoint.LastAckedEventSequence);

        var comparison = WatermarkComparer.Compare(command.ClientWatermark, baseline);
        return comparison == WatermarkComparison.Older ? StaleCommandDecision.RejectStale : StaleCommandDecision.Accept;
    }
}
