namespace Binho.Core.Contracts.Room;

public enum RoomPhase
{
    RoomCreated,
    WaitingForOpponent,
    BothPresentNotReady,
    ReadyToStart,
    StartingMatch,
    ActiveTurnPendingInput,
    ActiveTurnResolving,
    RestartPending,
    PausedForDisconnect,
    AbandonedPendingForfeit,
    MatchEnded,
    RematchPending,
    RoomClosed
}
