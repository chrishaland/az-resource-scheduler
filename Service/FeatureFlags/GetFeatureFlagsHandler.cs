using Unleash;

namespace Service.FeatureFlags;

[Route("api/feature-flags/get")]
public class GetFeatureFlagsHandler : QueryHandlerBase<GetFeatureFlagsRequest, GetFeatureFlagsResponse>
{
    private readonly IUnleash _featureFlagHandler;

    public GetFeatureFlagsHandler(IUnleash featureFlagHandler)
    {
        _featureFlagHandler = featureFlagHandler;
    }

    public override async Task<ActionResult<GetFeatureFlagsResponse>> Execute([FromBody] GetFeatureFlagsRequest request, CancellationToken ct)
    {
        await Task.CompletedTask;

        var dto = new FeatureFlagsDto(
            FeatureFlags: GetActiveFeatureFlags()
        );

        return Ok(new GetFeatureFlagsResponse(dto));
    }

    private string[] GetActiveFeatureFlags()
    {
        var activeFeatureFlags = new List<string>();
        var featureFlags = Enum.GetValues<FeatureFlagsEnum>().Select(f => f.ToString());

        foreach (var featureFlag in featureFlags)
        {
            if (_featureFlagHandler.IsEnabled(featureFlag))
            {
                activeFeatureFlags.Add(featureFlag);
            }
        }

        return activeFeatureFlags.ToArray();
    }
}
