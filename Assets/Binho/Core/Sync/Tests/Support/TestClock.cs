using System;
using Binho.Core.Sync.Abstractions;

namespace Binho.Core.Sync.Tests;

public sealed class TestClock : IClock
{
    public TestClock(DateTimeOffset utcNow) => UtcNow = utcNow;
    public DateTimeOffset UtcNow { get; set; }
}
