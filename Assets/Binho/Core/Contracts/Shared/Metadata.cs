namespace Binho.Core.Contracts.Shared;

public enum ResolutionState { Unresolved, Provisional, Final }
public enum GeometryReadiness { TopologyReady, ZoneReady, CoordinateReady }
public enum EvidenceStatus { Confirmed, Inferred, DesignedForDigital }
public enum ScopeKind { Room, Match }
public enum TimerRecordScope { Room, Match, RoomMatchBridge }
public enum SideId { North, South }
public enum OccupancyState { Empty, Joined, LeftRoom, Forfeited }
public enum RetentionStatus { NotRetained, Retained, Expired }
public enum RematchStatus { NotAvailable, Pending, Accepted, Declined }
public enum TimerStatus { Running, Cancelled, Expired }
