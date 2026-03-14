using System;

namespace Binho.Core.Sync.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
