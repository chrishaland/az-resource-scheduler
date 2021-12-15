using Service.Schedules;

namespace Tests.IntegrationTests.Schedules;

[TestFixture]
public class Get_environment_uptimes_tests
{
    [Test]
    public async Task Return_list_of_environment_uptimes()
    {
        await SUT.Database.DeleteAllAsync<Resource>(); 
        await SUT.Database.DeleteAllAsync<Environment>();
        await SUT.Database.DeleteAllAsync<ResourceStopJob>();

        var scheduledStopTime = DateTime.Now.AddHours(2);
        var entity = await SUT.Database.Environments.AddAsync(new Environment { Name = "dev", Description = "Development", ScheduledStartup = "* 1 * * *", ScheduledUptime = 1, Resources = new List<Resource> { } });

        var provider = await SUT.Database.Providers.AddAsync(new Provider { Name = "dev" });
        var resource = await SUT.Database.Resources.AddAsync(new VirtualMachine { Name = "vm", ResourceGroup = "rg", Description = "VM", Provider = provider.Entity, Environments = new List<Environment> { entity.Entity } });
        
        await SUT.Database.ResourceStopJobs.AddAsync(new ResourceStopJob { JobId = Guid.NewGuid().ToString(), Resource = resource.Entity, StopAt = scheduledStopTime });
        await SUT.Database.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/schedule/environment-uptimes/get");

        var (response, content) = await SUT.SendHttpRequest<GetEnvironmentUptimesResponse>(request, new { });

        if (content == null) throw new AssertionException("Http response did not return a object");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.EnvironmentUptimes.Keys.Count, Is.EqualTo(1));

            Assert.That(content.EnvironmentUptimes[entity.Entity.Id], Is.Not.Null);
            Assert.That(content.EnvironmentUptimes[entity.Entity.Id].ResourcesCount, Is.EqualTo(1));
            Assert.That(content.EnvironmentUptimes[entity.Entity.Id].ScheduledStopTime, Is.EqualTo(scheduledStopTime));
        });
    }

    [Test]
    public async Task Earliest_scheduled_stop_time_wins()
    {
        await SUT.Database.DeleteAllAsync<Resource>();
        await SUT.Database.DeleteAllAsync<Environment>();
        await SUT.Database.DeleteAllAsync<ResourceStopJob>();

        var scheduledStopTime1 = DateTime.Now.AddHours(2);
        var scheduledStopTime2 = DateTime.Now.AddHours(3);
        var entity = await SUT.Database.Environments.AddAsync(new Environment { Name = "dev", Description = "Development", ScheduledStartup = "* 1 * * *", ScheduledUptime = 1, Resources = new List<Resource> { } });

        var provider = await SUT.Database.Providers.AddAsync(new Provider { Name = "dev" });
        var resource1 = await SUT.Database.Resources.AddAsync(new VirtualMachine { Name = "vm1", ResourceGroup = "rg", Description = "VM", Provider = provider.Entity, Environments = new List<Environment> { entity.Entity } });
        var resource2 = await SUT.Database.Resources.AddAsync(new VirtualMachine { Name = "vm2", ResourceGroup = "rg", Description = "VM", Provider = provider.Entity, Environments = new List<Environment> { entity.Entity } });
        
        await SUT.Database.ResourceStopJobs.AddAsync(new ResourceStopJob { JobId = Guid.NewGuid().ToString(), Resource = resource1.Entity, StopAt = scheduledStopTime1 });
        await SUT.Database.ResourceStopJobs.AddAsync(new ResourceStopJob { JobId = Guid.NewGuid().ToString(), Resource = resource2.Entity, StopAt = scheduledStopTime2 });

        await SUT.Database.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/schedule/environment-uptimes/get");

        var (response, content) = await SUT.SendHttpRequest<GetEnvironmentUptimesResponse>(request, new { });

        if (content == null) throw new AssertionException("Http response did not return a object");

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.EnvironmentUptimes.Keys.Count, Is.EqualTo(1));

            Assert.That(content.EnvironmentUptimes[entity.Entity.Id], Is.Not.Null);
            Assert.That(content.EnvironmentUptimes[entity.Entity.Id].ResourcesCount, Is.EqualTo(2));
            Assert.That(content.EnvironmentUptimes[entity.Entity.Id].ScheduledStopTime, Is.EqualTo(scheduledStopTime1));
        });
    }

    [Test]
    public async Task Return_empty_list_when_no_environment_uptimes()
    {
        await SUT.Database.DeleteAllAsync<Resource>();
        await SUT.Database.DeleteAllAsync<Environment>();
        await SUT.Database.DeleteAllAsync<ResourceStopJob>();
        
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/schedule/environment-uptimes/get");

        var (response, content) = await SUT.SendHttpRequest<GetEnvironmentUptimesResponse>(request, new { });

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content?.EnvironmentUptimes, Is.Not.Null);
            CollectionAssert.IsEmpty(content?.EnvironmentUptimes.Keys);
        });
    }
}
