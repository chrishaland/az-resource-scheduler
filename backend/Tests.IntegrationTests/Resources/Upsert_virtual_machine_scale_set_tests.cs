using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repository.Models;
using Service;
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
    public class Upsert_virtual_machine_scale_set_tests
    {
        [Test]
        public async Task Create_new_virtual_machine_scale_set()
        {
            var environment = await SUT.Database.Environments.AddAsync(new Environment { Name = "env" });
            await SUT.Database.SaveChangesAsync();

            var data = new
            {
                name = "dev-machine-1",
                resourceGroup = "dev-machines",
                description = "Dev Machine 1",
                virtualMachineScaleSetExtentions = new { capacity = 1 },
                environmentIds = new[] { environment.Entity.Id }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content?.Id, Is.Not.Null);
                Assert.That(content?.Id, Is.Not.EqualTo(Guid.Empty));
            });

            var entity = await SUT.Database.VirtualMachineScaleSets
                .Include(v => v.Environments)
                .SingleOrDefaultAsync(e => e.Id.ToString() == content.Id);

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(Guid.Parse(content.Id)));
                Assert.That(entity.Name, Is.EqualTo(data.name));
                Assert.That(entity.ResourceGroup, Is.EqualTo(data.resourceGroup));
                Assert.That(entity.Description, Is.EqualTo(data.description));
                Assert.That(entity.Capacity, Is.EqualTo(data.virtualMachineScaleSetExtentions.capacity));
                Assert.That(entity.Environments.Any(e => e.Id.Equals(data.environmentIds[0])), Is.True);
            });
        }

        [Test]
        public async Task Update_existing_virtual_machine_scale_set()
        {
            var env1 = await SUT.Database.Environments.AddAsync(new Environment());
            var env2 = await SUT.Database.Environments.AddAsync(new Environment());
            var entity = new VirtualMachineScaleSet { Name = "1", Description = "One", ResourceGroup = "RG1", Capacity = 1, Environments = new List<Environment> { env1.Entity } };
            await SUT.Database.VirtualMachineScaleSets.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();
            SUT.Database.ChangeTracker.Clear();

            var data = new
            {
                id = entity.Id,
                name = "2",
                resourceGroup = "RG2",
                description = "Two",
                virtualMachineScaleSetExtentions = new { capacity = 2 },
                environmentIds = new[] { env2.Entity.Id }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            entity = await SUT.Database.VirtualMachineScaleSets
                .Include(v => v.Environments)
                .SingleOrDefaultAsync(e => e.Id.Equals(entity.Id));

            Assert.Multiple(() =>
            {
                Assert.That(entity.Id, Is.EqualTo(data.id));
                Assert.That(entity.Name, Is.EqualTo(data.name));
                Assert.That(entity.ResourceGroup, Is.EqualTo(data.resourceGroup));
                Assert.That(entity.Description, Is.EqualTo(data.description));
                Assert.That(entity.Capacity, Is.EqualTo(data.virtualMachineScaleSetExtentions.capacity));
                Assert.That(entity.Environments.Count(), Is.EqualTo(1));
                Assert.That(entity.Environments.Any(e => e.Id.Equals(data.environmentIds[0])), Is.True);
            });
        }

        [Test]
        public async Task Update_should_remove_linked_environments_if_not_defined()
        {
            var environment = await SUT.Database.Environments.AddAsync(new Environment());
            var entity = new VirtualMachineScaleSet { Environments = new List<Environment> { environment.Entity } };
            await SUT.Database.VirtualMachineScaleSets.AddAsync(entity);
            await SUT.Database.SaveChangesAsync();
            SUT.Database.ChangeTracker.Clear();

            var data = new
            {
                id = entity.Id,
                name = " ",
                resourceGroup = " ",
                description = " ",
                virtualMachineScaleSetExtentions = new { }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/upsert");

            var (response, content) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            entity = await SUT.Database.VirtualMachineScaleSets
                .Include(v => v.Environments)
                .SingleOrDefaultAsync(e => e.Id.Equals(entity.Id));

            CollectionAssert.IsEmpty(entity.Environments);
        }


        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("d982041a-3789-40ea-909d-479386000602")]
        public async Task Invalid_id_should_result_in_bad_request_response(string id)
        {
            var data = new
            {
                id = Guid.Parse(id),
                name = " ",
                resourceGroup = " ",
                description = " ",
                virtualMachineScaleSetExtentions = new { }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/upsert");

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
                resourceGroup = " ",
                description = " ",
                virtualMachineScaleSetExtentions = new { }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/upsert");

            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Invalid_resource_group_should_result_in_bad_request_response(string resourceGroup)
        {
            var data = new
            {
                name = " ",
                resourceGroup = resourceGroup,
                description = " ",
                virtualMachineScaleSetExtentions = new { }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/resource/upsert");

            var (response, _) = await SUT.SendHttpRequest<CommandResponse>(request, data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
