using System;
using Binho.Core.Contracts.Room;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Persistence;

public sealed record RoomSnapshotRecordDto(RoomId RoomId, RoomVersion RoomVersion, RoomPhase RoomPhase, MatchId? ActiveMatchId, MatchInstance? ActiveMatchInstance, RoomSnapshotDto Snapshot, DateTimeOffset LastUpdatedAtUtc);
