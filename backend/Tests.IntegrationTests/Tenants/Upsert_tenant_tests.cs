using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repository.Models;
using Service;
using Service.Tenants;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Tenants
{
    [TestFixture]
    public class Upsert_tenant_tests
    {
        [Test]
        public async Task Create_new_tenant()
        {
            var data = new UpsertTenantRequest(Id: null, Name: "hs", Description: "Han Solo");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content?.Id, Is.Not.Null);
                Assert.That(content?.Id, Is.Not.EqualTo(Guid.Empty));
            });

            var entity = await SUT.Database.Tenants
                .SingleOrDefaultAsync(e => e.Id.ToString() == content.Id);

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(Guid.Parse(content.Id)));
                Assert.That(entity.Name, Is.EqualTo(data.Name));
                Assert.That(entity.Description, Is.EqualTo(data.Description));
            });
        }

        [Test]
        public async Task Update_existing_tenant()
        {
            var entity = new Tenant { Name = "hs", Description = "Han Solo" };
            await SUT.Database.Tenants.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();
            SUT.Database.ChangeTracker.Clear();

            var data = new UpsertTenantRequest(Id: entity.Id, Name: "ls", Description: "Luke Skywalker");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            entity = await SUT.Database.Tenants
                .SingleOrDefaultAsync(e => e.Id.Equals(entity.Id));

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(data.Id));
                Assert.That(entity.Name, Is.EqualTo(data.Name));
                Assert.That(entity.Description, Is.EqualTo(data.Description));
            });
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Invalid_id_should_result_in_bad_request_response(string id)
        {
            var data = new UpsertTenantRequest(Id: Guid.Parse(id), Name: "hs", Description: "Han Solo");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/upsert");
            
            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Invalid_name_should_result_in_bad_request_response(string name)
        {
            var data = new UpsertTenantRequest(Id: null, Name: name, Description: null);
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/upsert");

            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
