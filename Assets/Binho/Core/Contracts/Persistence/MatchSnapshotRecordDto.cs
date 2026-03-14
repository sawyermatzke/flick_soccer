using System;
using Binho.Core.Contracts.Match;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Persistence;

public sealed record MatchSnapshotRecordDto(RoomId RoomId, MatchId MatchId, MatchInstance MatchInstance, StateVersion StateVersion, EventSequence LastCommittedEventSequence, MatchSnapshotDto Snapshot, DateTimeOffset CapturedAtUtc);
