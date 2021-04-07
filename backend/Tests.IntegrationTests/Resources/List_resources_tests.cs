using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repository.Models;
using Service.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Environment = Repository.Models.Environment;

namespace Tests.IntegrationTests.Resources
{
    [TestFixture]
    public class List_resources_tests
    {
        [Test]
        public async Task Return_list_of_environment_specific_resources()
        {
            await SUT.Database.DeleteAllAsync<Resource>();
            var environment = await SUT.Database.Environments.AddAsync(new Environment { Name = "env" });
            var entity1 = new VirtualMachine { Name = "vm", ResourceGroup = "rg", Description = "VM", Environments = new List<Environment> { environment.Entity } };
            var entity2 = new VirtualMachineScaleSet { Name = "vmss1", ResourceGroup = "rgss1", Description = "VMSS1", Capacity = 10, Environments = new List<Environment> { environment.Entity } };
            var entity3 = new VirtualMachineScaleSet { Name = "vmss2", ResourceGroup = "rgss2", Description = "VMSS2", Capacity = 20 };
            await SUT.Database.Resources.AddRangeAsync(entity1, entity2, entity3);
            await SUT.Database.SaveChangesAsync();

            var data = new { environmentId = environment.Entity.Id };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/list");

            var (response, content) = await SUT.SendHttpRequest<ListResourcesResponse>(request, data);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Resources.Length, Is.EqualTo(2));

                var dto1 = content.Resources.Single(e => e.Id.Equals(entity1.Id));
                Assert.That(dto1.Id, Is.EqualTo(entity1.Id));

                var dto2 = content.Resources.Single(e => e.Id.Equals(entity2.Id));
                Assert.That(dto2.Id, Is.EqualTo(entity2.Id));
            });
        }

        [Test]
        public async Task Return_list_of_all_resources()
        {
            await SUT.Database.DeleteAllAsync<Resource>();
            var entity1 = new VirtualMachine { Name = "vm", ResourceGroup = "rg", Description = "VM" };
            var entity2 = new VirtualMachineScaleSet { Name = "vmss", ResourceGroup = "rgss", Description = "VMSS", Capacity = 10 };
            await SUT.Database.Resources.AddRangeAsync(entity1, entity2);
            await SUT.Database.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/list");

            var (response, content) = await SUT.SendHttpRequest<ListResourcesResponse>(request, new { });

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Resources.Length, Is.EqualTo(2));

                var dto1 = content.Resources.Single(e => e.Id.Equals(entity1.Id));
                Assert.That(dto1.Id, Is.EqualTo(entity1.Id));
                Assert.That(dto1.Name, Is.EqualTo(entity1.Name));
                Assert.That(dto1.ResourceGroup, Is.EqualTo(entity1.ResourceGroup));
                Assert.That(dto1.Description, Is.EqualTo(entity1.Description));

                var dto2 = content.Resources.Single(e => e.Id.Equals(entity2.Id));
                Assert.That(dto2.Id, Is.EqualTo(entity2.Id));
                Assert.That(dto2.Name, Is.EqualTo(entity2.Name));
                Assert.That(dto2.ResourceGroup, Is.EqualTo(entity2.ResourceGroup));
                Assert.That(dto2.Description, Is.EqualTo(entity2.Description));
                Assert.That(dto2.VirtualMachineScaleSetExtentions.Capacity, Is.EqualTo(entity2.Capacity));
            });
        }

        [TestCase(null)]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Return_empty_list_when_no_resources(string guid)
        {
            await SUT.Database.DeleteAllAsync<Resource>();

            object data = guid == null ? new { } : new { environmentId = Guid.Parse(guid) };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/list");

            var (response, content) = await SUT.SendHttpRequest<ListResourcesResponse>(request, data);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content.Resources, Is.Not.Null);
                CollectionAssert.IsEmpty(content.Resources);
            });
        }
    }
}
