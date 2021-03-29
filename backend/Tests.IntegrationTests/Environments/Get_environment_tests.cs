using NUnit.Framework;
using Service.Environments;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Environment = Repository.Models.Environment;

namespace Tests.IntegrationTests.Environments
{
    [TestFixture]
    public class Get_environment_tests
    {
        [Test]
        public async Task Get_existing_environment()
        {
            var entity = new Environment { Name = "Development" };
            await SUT.Database.Environments.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();

            var data = new GetEnvironmentRequest(Id: entity.Id);
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/get");

            var (response, content) = await SUT.SendHttpRequest<GetEnvironmentResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.Multiple(() =>
            {
                Assert.That(content.Environment.Id, Is.EqualTo(entity.Id));
                Assert.That(content.Environment.Name, Is.EqualTo(entity.Name));
            });
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Getting_non_existent_environments_results_in_not_found_response(string id)
        {
            var data = new GetEnvironmentRequest(Id: Guid.Parse(id));
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/get");

            var (response, _) = await SUT.SendHttpRequest<GetEnvironmentResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
