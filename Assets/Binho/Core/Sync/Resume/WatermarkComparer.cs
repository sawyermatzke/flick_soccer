using Binho.Core.Contracts.Sync;

namespace Binho.Core.Sync.Resume;

public enum WatermarkComparison
{
    Older,
    Equal,
    Newer,
    Equivalent
}

public static class WatermarkComparer
{
    public static WatermarkComparison Compare(ClientWatermarkDto? candidate, AcknowledgedWatermarkDto? baseline)
    {
        if (candidate is null && baseline is null) return WatermarkComparison.Equal;
        if (candidate is null) return WatermarkComparison.Older;
        if (baseline is null) return WatermarkComparison.Newer;

        var state = NullableCompare(candidate.LastKnownStateVersion?.Value, baseline.StateVersion?.Value);
        if (state != 0) return state < 0 ? WatermarkComparison.Older : WatermarkComparison.Newer;

        var seq = NullableCompare(candidate.LastKnownEventSequence?.Value, baseline.EventSequence?.Value);
        if (seq != 0) return seq < 0 ? WatermarkComparison.Older : WatermarkComparison.Newer;

        if (candidate.LastSnapshotId is not null && baseline.SnapshotId != default)
        {
            return candidate.LastSnapshotId.Value == baseline.SnapshotId.Value
                ? WatermarkComparison.Equal
                : WatermarkComparison.Equivalent;
        }

        return WatermarkComparison.Equal;
    }

    private static int NullableCompare(int? left, int? right)
    {
        if (left.HasValue && right.HasValue) return left.Value.CompareTo(right.Value);
        if (left.HasValue) return 1;
        if (right.HasValue) return -1;
        return 0;
    }
}
