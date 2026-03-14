using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Match;

public static class MatchEventSequencer
{
    public static EventSequence Next(EventSequence current) => new(current.Value + 1);
}
