namespace Tests.IntegrationTests.Providers;

[TestFixture]
public class Upsert_azure_provider_tests
{
    [Test]
    public async Task Create_new_azure_provider()
    {
        var data = new
        {
            name = "dev",
            azureProviderExtentions = new 
            {
                tenantId = Guid.NewGuid().ToString(),
                clientId = "id",
                clientSecret = "secret",
                subscriptionId = Guid.NewGuid().ToString()
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/provider/upsert");

        var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);

        if (content == null) throw new AssertionException("Http response did not return a object");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.Id, Is.Not.Null);
            Assert.That(content.Id, Is.Not.EqualTo(Guid.Empty));
        });

        var entity = await SUT.Database.AzureProviders
            .SingleAsync(e => e.Id.ToString() == content.Id);

        Assert.Multiple(() =>
        {
            Assert.That(entity.Id, Is.EqualTo(Guid.Parse(content.Id)));
            Assert.That(entity.Name, Is.EqualTo(data.name));
            Assert.That(entity.ClientId, Is.EqualTo(data.azureProviderExtentions.clientId));
            Assert.That(entity.ClientSecret, Is.EqualTo(data.azureProviderExtentions.clientSecret));
            Assert.That(entity.SubscriptionId, Is.EqualTo(data.azureProviderExtentions.subscriptionId));
            Assert.That(entity.TenantId, Is.EqualTo(data.azureProviderExtentions.tenantId));
        });
    }

    [Test]
    public async Task Update_existing_azure_provider()
    {
        var entity = new AzureProvider { Name = "dev1", ClientId = "id1", ClientSecret = "secret1", SubscriptionId = Guid.NewGuid().ToString(), TenantId = Guid.NewGuid().ToString() };
        await SUT.Database.AzureProviders.AddAsync(entity);
        await SUT.Database.SaveChangesAsync();
        SUT.Database.ChangeTracker.Clear();

        var data = new
        {
            id = entity.Id,
            name = "dev2",
            azureProviderExtentions = new
            {
                tenantId = Guid.NewGuid().ToString(),
                clientId = "id2",
                clientSecret = "secret2",
                subscriptionId = Guid.NewGuid().ToString()
            }
        };
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/provider/upsert");

        var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        entity = await SUT.Database.AzureProviders
            .SingleAsync(e => e.Id.Equals(entity.Id));

        Assert.Multiple(() =>
        {
            Assert.That(entity.Id, Is.EqualTo(data.id));
            Assert.That(entity.Name, Is.EqualTo(data.name));
            Assert.That(entity.ClientId, Is.EqualTo(data.azureProviderExtentions.clientId));
            Assert.That(entity.ClientSecret, Is.EqualTo(data.azureProviderExtentions.clientSecret));
            Assert.That(entity.SubscriptionId, Is.EqualTo(data.azureProviderExtentions.subscriptionId));
            Assert.That(entity.TenantId, Is.EqualTo(data.azureProviderExtentions.tenantId));
        });
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("d982041a-3789-40ea-909d-479386000602")]
    public async Task Invalid_id_should_result_in_bad_request_response(string id)
    {
        var data = new
        {
            id = Guid.Parse(id),
            name = " ",
            azureProviderExtentions = new
            {
                tenantId = " ",
                clientId = " ",
                clientSecret = " ",
                subscriptionId = " "
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/provider/upsert");

        var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task Invalid_name_should_result_in_bad_request_response(string name)
    {
        var data = new
        {
            name = name,
            azureProviderExtentions = new
            {
                tenantId = " ",
                clientId = " ",
                clientSecret = " ",
                subscriptionId = " "
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/provider/upsert");

        var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}
