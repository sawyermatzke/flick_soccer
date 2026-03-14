using System;
using Binho.Core.Contracts.Shared;

namespace Binho.Core.Contracts.Sync;

public sealed record CommandEnvelopeDto<TPayload>(
    CommandId CommandId,
    string CommandType,
    RoomId RoomId,
    MatchId? MatchId,
    SeatId Seat,
    ClientWatermarkDto? ClientWatermark,
    TPayload Payload,
    DateTimeOffset SentAtUtc);
