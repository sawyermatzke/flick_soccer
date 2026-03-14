using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Room;

public sealed record RoomRetentionDto(RetentionStatus Status, DateTimeOffset? ExpiresAtUtc);
