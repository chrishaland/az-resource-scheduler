using Service.FeatureFlags;

namespace Tests.IntegrationTests.FeaureFlags;

[TestFixture]
public class Get_feature_flags_tests
{
    [Test]
    public async Task Get_feature_flags()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/feature-flags/get");

        var (response, content) = await SUT.SendHttpRequest<GetFeatureFlagsResponse>(request, new { });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        if (content == null) throw new AssertionException("Http response did not return a object");

        Assert.Multiple(() =>
        {
            foreach (var featureFlag in Enum.GetValues<FeatureFlagsEnum>().Select(f => f.ToString()))
            {
                CollectionAssert.Contains(content.FeatureFlags.FeatureFlags, featureFlag);
            }
        });
    }
}
