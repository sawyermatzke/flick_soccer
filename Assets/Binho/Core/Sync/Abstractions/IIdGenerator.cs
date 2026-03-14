using Binho.Core.Contracts.Shared;

namespace Binho.Core.Sync.Abstractions;

public interface IIdGenerator
{
    SnapshotId NextSnapshotId();
    EventId NextEventId();
    CommandId NextCommandId();
    TimerId NextTimerId();
}
