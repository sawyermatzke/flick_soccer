using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Sync;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IMatchSyncService
{
    Result<MatchSnapshotDto> HandleCommand<TPayload>(CommandEnvelopeDto<TPayload> command);
}
