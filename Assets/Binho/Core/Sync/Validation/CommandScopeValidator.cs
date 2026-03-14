using System;
using System.Collections.Generic;
using Binho.Core.Contracts.Shared;
using Binho.Core.Contracts.Sync;

namespace Binho.Core.Sync.Validation;

public static class CommandScopeValidator
{
    private static readonly HashSet<string> RoomCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "resumeSession", "acknowledgeSnapshot", "requestRematch", "declineRematch", "leaveRoom", "startMatch"
    };

    private static readonly HashSet<string> MatchCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        "submitShotIntent", "concedeMatch"
    };

    public static Result Validate<TPayload>(CommandEnvelopeDto<TPayload> command)
    {
        var errors = new List<string>();
        if (command.CommandId == default) errors.Add("CommandId is required.");
        if (string.IsNullOrWhiteSpace(command.CommandType)) errors.Add("CommandType is required.");
        if (command.RoomId == default) errors.Add("RoomId is required.");
        if (command.Seat == default) errors.Add("Seat is required.");

        if (MatchCommands.Contains(command.CommandType) && command.MatchId is null)
            errors.Add("MatchId is required for match-scoped commands.");

        if (RoomCommands.Contains(command.CommandType) == false && MatchCommands.Contains(command.CommandType) == false)
            errors.Add("Unknown command type for first-pass scope validation.");

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
    }
}
