namespace BuggyAnimalDetailsApi.Api;

public record FaultyApiFlag(bool Enabled);

public record RateLimitedApiFlag(bool Enabled);

public record CoolingDownApiFlag(bool Enabled);
