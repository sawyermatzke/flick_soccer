using Binho.Core.Contracts.Shared;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Tests;

public sealed class TestIdGenerator : IIdGenerator
{
    private int _snapshot;
    private int _event;
    private int _command;
    private int _timer;

    public SnapshotId NextSnapshotId() => new($"snap_{++_snapshot:000}");
    public EventId NextEventId() => new($"evt_{++_event:000}");
    public CommandId NextCommandId() => new($"cmd_{++_command:000}");
    public TimerId NextTimerId() => new($"timer_{++_timer:000}");
}
