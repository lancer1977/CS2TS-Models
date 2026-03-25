namespace CS2TS.Tests.Fixtures;

/// <summary>
/// Sample interfaces for testing TypeScript generation.
/// Covers: primitives, nullable types, collections, enums.
/// </summary>

// Simple interface with primitive types
public interface IPrimitiveModel
{
    int Id { get; set; }
    string Name { get; set; }
    bool IsActive { get; set; }
    double Value { get; set; }
    DateTime CreatedAt { get; set; }
}

// Interface with nullable types
public interface INullableModel
{
    string? OptionalName { get; set; }
    int? OptionalCount { get; set; }
    DateTime? OptionalDate { get; set; }
    Guid? OptionalId { get; set; }
}

// Interface with collections
public interface ICollectionModel
{
    List<int> Numbers { get; set; }
    List<string> Names { get; set; }
    IEnumerable<IPlayerModel> Players { get; set; }
    Dictionary<string, int> Scores { get; set; }
}

// Interface with nested object reference
public interface IPlayerModel
{
    string Name { get; set; }
    int Score { get; set; }
    PlayerStatus Status { get; set; }
}

// Enum for testing enum generation
public enum PlayerStatus
{
    Offline = 0,
    Online = 1,
    InGame = 2,
    Away = 3
}

// Complex interface combining multiple concepts
public interface IGameSession
{
    Guid SessionId { get; set; }
    string? DisplayName { get; set; }
    List<IPlayerModel> Participants { get; set; }
    PlayerStatus CurrentStatus { get; set; }
    DateTime StartTime { get; set; }
    DateTime? EndTime { get; set; }
    Dictionary<string, string> Metadata { get; set; }
}