using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Service.Environments;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Environment = Repository.Models.Environment;

namespace Tests.IntegrationTests.Environments
{
    [TestFixture]
    public class List_environments_tests
    {
        [Test]
        public async Task Return_list_of_environments()
        {
            await SUT.Database.DeleteAllAsync<Environment>();
            var entity1 = new Environment { Name = "dev", Description = "Development", ScheduledStartup = "* 1 * * *", ScheduledUptime = 1 };
            var entity2 = new Environment { Name = "qa", Description = "Test", ScheduledStartup = "* 2 * * *", ScheduledUptime = 2 };
            await SUT.Database.Environments.AddRangeAsync(entity1, entity2);
            await SUT.Database.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/list");

            var (response, content) = await SUT.SendHttpRequest<ListEnvironmentsResponse>(request, new ListEnvironmentsRequest());

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Environments.Length, Is.EqualTo(2));

                var dto1 = content.Environments.Single(e => e.Id.Equals(entity1.Id));
                Assert.That(dto1.Id, Is.EqualTo(entity1.Id));
                Assert.That(dto1.Name, Is.EqualTo(entity1.Name));
                Assert.That(dto1.Description, Is.EqualTo(entity1.Description));
                Assert.That(dto1.ScheduledStartup, Is.EqualTo(entity1.ScheduledStartup));
                Assert.That(dto1.ScheduledUptime, Is.EqualTo(entity1.ScheduledUptime));

                var dto2 = content.Environments.Single(e => e.Id.Equals(entity2.Id));
                Assert.That(dto2.Id, Is.EqualTo(entity2.Id));
                Assert.That(dto2.Name, Is.EqualTo(entity2.Name));
                Assert.That(dto2.Description, Is.EqualTo(entity2.Description));
                Assert.That(dto2.ScheduledStartup, Is.EqualTo(entity2.ScheduledStartup));
                Assert.That(dto2.ScheduledUptime, Is.EqualTo(entity2.ScheduledUptime));
            });
        }

        [Test]
        public async Task Return_empty_list_when_no_environments()
        {
            await SUT.Database.DeleteAllAsync<Environment>();
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/list");

            var (response, content) = await SUT.SendHttpRequest<ListEnvironmentsResponse>(request, new ListEnvironmentsRequest());

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Environments, Is.Not.Null);
                CollectionAssert.IsEmpty(content.Environments);
            });
        }
    }
}
