using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repository.Models;
using Service.Tenants;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.Tenants
{
    [TestFixture]
    public class List_tenants_tests
    {
        [Test]
        public async Task Return_list_of_tenants()
        {
            await SUT.Database.DeleteAllAsync<Tenant>();

            var entity1 = new Tenant { Name = "hs", Description = "Han Solo" };
            var entity2 = new Tenant { Name = "ls", Description = "Luke Skywalker" };
            await SUT.Database.Tenants.AddRangeAsync(entity1, entity2);
            await SUT.Database.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/list");

            var (response, content) = await SUT.SendHttpRequest<ListTenantsResponse>(request, new ListTenantsRequest());

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Tenants.Length, Is.EqualTo(2));

                var dto1 = content.Tenants.Single(e => e.Id.Equals(entity1.Id));
                Assert.That(dto1.Id, Is.EqualTo(entity1.Id));
                Assert.That(dto1.Name, Is.EqualTo(entity1.Name));

                var dto2 = content.Tenants.Single(e => e.Id.Equals(entity2.Id));
                Assert.That(dto2.Id, Is.EqualTo(entity2.Id));
                Assert.That(dto2.Name, Is.EqualTo(entity2.Name));
            });
        }

        [Test]
        public async Task Return_empty_list_when_no_tenants()
        {
            await SUT.Database.DeleteAllAsync<Tenant>();
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/tenant/list");

            var (response, content) = await SUT.SendHttpRequest<ListTenantsResponse>(request, new ListTenantsRequest());

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Tenants, Is.Not.Null);
                CollectionAssert.IsEmpty(content.Tenants);
            });
        }
    }
}
