using System;

namespace Binho.Core.Contracts.Shared;

public readonly record struct RoomId(string Value);
public readonly record struct RoomCode(string Value);
public readonly record struct MatchId(string Value);
public readonly record struct PlayerId(string Value);
public readonly record struct SeatId(string Value);
public readonly record struct CommandId(string Value);
public readonly record struct EventId(string Value);
public readonly record struct SnapshotId(string Value);
public readonly record struct TimerId(string Value);
