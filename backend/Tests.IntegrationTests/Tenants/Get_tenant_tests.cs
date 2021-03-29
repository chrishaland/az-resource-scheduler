using NUnit.Framework;
using Repository.Models;
using Service.Tenants;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Tenants
{
    [TestFixture]
    public class Get_tenant_tests
    {
        [Test]
        public async Task Get_existing_tenant()
        {
            var entity = new Tenant { Name = "hs", Description = "Han Solo" };
            await SUT.Database.Tenants.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();

            var data = new GetTenantRequest(Id: entity.Id);
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/get");

            var (response, content) = await SUT.SendHttpRequest<GetTenantResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.Multiple(() =>
            {
                Assert.That(content.Tenant.Id, Is.EqualTo(entity.Id));
                Assert.That(content.Tenant.Name, Is.EqualTo(entity.Name));
                Assert.That(content.Tenant.Description, Is.EqualTo(entity.Description));
            });
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Getting_non_existent_tenants_results_in_not_found_response(string id)
        {
            var data = new GetTenantRequest(Id: Guid.Parse(id));
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/get");

            var (response, _) = await SUT.SendHttpRequest<GetTenantResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
