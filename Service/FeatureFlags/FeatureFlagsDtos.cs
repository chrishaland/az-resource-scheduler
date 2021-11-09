namespace Service.FeatureFlags;

public record GetFeatureFlagsRequest;
public record GetFeatureFlagsResponse(FeatureFlagsDto FeatureFlags);

public record FeatureFlagsDto(string[] FeatureFlags);
