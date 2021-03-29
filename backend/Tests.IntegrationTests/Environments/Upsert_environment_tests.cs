using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Service;
using Service.Environments;
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
            var data = new UpsertEnvironmentRequest(Id: null, Name: "Test");
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
                Assert.That(entity.Name, Is.EqualTo(data.Name));
            });
        }

        [Test]
        public async Task Update_existing_environment()
        {
            var entity = new Environment { Name = "Original" };
            await SUT.Database.Environments.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();
            SUT.Database.ChangeTracker.Clear();

            var data = new UpsertEnvironmentRequest(Id: entity.Id, Name: "Updated");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            entity = await SUT.Database.Environments
                .SingleOrDefaultAsync(e => e.Id.Equals(entity.Id));

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(data.Id));
                Assert.That(entity.Name, Is.EqualTo(data.Name));
            });
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Invalid_id_should_result_in_bad_request_response(string id)
        {
            var data = new UpsertEnvironmentRequest(Id: Guid.Parse(id), Name: "Invalid id");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");
            
            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Invalid_name_should_result_in_bad_request_response(string name)
        {
            var data = new UpsertEnvironmentRequest(Id: null, Name: name);
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/environment/upsert");

            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
