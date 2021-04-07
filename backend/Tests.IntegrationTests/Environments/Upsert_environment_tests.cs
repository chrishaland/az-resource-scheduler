using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Service;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Environment = Repository.Models.Environment;

namespace Tests.IntegrationTests.Environments
{
    [TestFixture]
    public class Upsert_environment_tests
    {
        [Test]
        public async Task Create_new_environment()
        {
            var data = new 
            { 
                name = "qa", 
                description = "Test",
                scheduledStartup = "* * * * *",
                scheduledUptime = 6
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content?.Id, Is.Not.Null);
                Assert.That(content?.Id, Is.Not.EqualTo(Guid.Empty));
            });

            var entity = await SUT.Database.Environments
                .SingleOrDefaultAsync(e => e.Id.ToString() == content.Id);

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(Guid.Parse(content.Id)));
                Assert.That(entity.Name, Is.EqualTo(data.name));
                Assert.That(entity.Description, Is.EqualTo(data.description));
                Assert.That(entity.ScheduledStartup, Is.EqualTo(data.scheduledStartup));
                Assert.That(entity.ScheduledUptime, Is.EqualTo(data.scheduledUptime));
            });
        }

        [Test]
        public async Task Update_existing_environment()
        {
            var entity = new Environment { Name = "1", Description = "One", ScheduledStartup = "* 1 * * *", ScheduledUptime = 1 };
            await SUT.Database.Environments.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();
            SUT.Database.ChangeTracker.Clear();

            var data = new
            {
                id = entity.Id,
                name = "2",
                description = "Two",
                scheduledStartup = "* 2 * * *",
                scheduledUptime = 2
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            entity = await SUT.Database.Environments
                .SingleOrDefaultAsync(e => e.Id.Equals(entity.Id));

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(data.id));
                Assert.That(entity.Name, Is.EqualTo(data.name));
                Assert.That(entity.Description, Is.EqualTo(data.description));
                Assert.That(entity.ScheduledStartup, Is.EqualTo(data.scheduledStartup));
                Assert.That(entity.ScheduledUptime, Is.EqualTo(data.scheduledUptime));
            });
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Invalid_id_should_result_in_bad_request_response(string id)
        {
            var data = new
            {
                id = Guid.Parse(id),
                name = "2",
                scheduledStartup = "* * * * *"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");
            
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
                scheduledStartup = "* * * * *"
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");

            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase("* * * *")]
        [TestCase("this is not a cron expression")]
        public async Task Invalid_scheduled_startup_should_result_in_bad_request_response(string cronExpression)
        {
            var data = new
            {
                name = " ",
                scheduledStartup = cronExpression
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");

            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
