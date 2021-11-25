using Service.Providers;

namespace Tests.IntegrationTests.Providers;

[TestFixture]
public class Get_provider_tests
{
    [Test]
    public async Task Get_existing_azure_provider()
    {
        var entity = new AzureProvider { Name = "dev", ClientId = "id", ClientSecret = "secret", SubscriptionId = Guid.NewGuid().ToString(), TenantId = Guid.NewGuid().ToString() };
        await SUT.Database.AzureProviders.AddAsync(entity);
        await SUT.Database.SaveChangesAsync();

        var data = new { id = entity.Id };
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/provider/get");

        var (response, content) = await SUT.SendHttpRequest<GetProviderResponse>(request, data);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        Assert.Multiple(() =>
        {
            Assert.That(content?.Provider.Id, Is.EqualTo(entity.Id));
            Assert.That(content?.Provider.Name, Is.EqualTo(entity.Name));
            Assert.That(content?.Provider.AzureProviderExtentions?.ClientId, Is.EqualTo(entity.ClientId));
            Assert.That(content?.Provider.AzureProviderExtentions?.ClientSecret, Is.EqualTo(entity.ClientSecret));
            Assert.That(content?.Provider.AzureProviderExtentions?.SubscriptionId, Is.EqualTo(entity.SubscriptionId));
            Assert.That(content?.Provider.AzureProviderExtentions?.TenantId, Is.EqualTo(entity.TenantId));
        });
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("d982041a-3789-40ea-909d-479386010602")]
    public async Task Getting_non_existent_providers_results_in_not_found_response(string id)
    {
        var data = new { id = Guid.Parse(id) };
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/provider/get");

        var (response, _) = await SUT.SendHttpRequest<GetProviderResponse>(request, data);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
