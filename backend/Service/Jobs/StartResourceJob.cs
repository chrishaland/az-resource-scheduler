using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Resource = Repository.Models.Resource;

namespace Service.Jobs
{
    public class StartResourceJob
    {
        private readonly DatabaseContext _context;
        private readonly ComputeManagementClient _client;

        public StartResourceJob(DatabaseContext context, ComputeManagementClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task Execute(Guid resourceId, CancellationToken ct)
        {
            //TODO
        }

        private async Task StartVirtualMachine(Resource resource, CancellationToken ct = default)
        {
            await _client.VirtualMachines.StartStartAsync(
                resourceGroupName: resource.ResourceGroup,
                vmName: resource.Name,
                cancellationToken: ct
           );
        }

        private async Task StartNodePool(Resource resource, CancellationToken ct)
        {
            var vmss = await _client.VirtualMachineScaleSets.GetAsync(
                resourceGroupName: resource.ResourceGroup,
                vmScaleSetName: resource.Name,
                cancellationToken: ct
            );

            vmss.Value.Sku.Capacity = resource.VirtualMachineScaleSet.Capacity;

            var parameters = new VirtualMachineScaleSetUpdate
            {
                Sku = vmss.Value.Sku
            };

            await _client.VirtualMachineScaleSets.StartUpdateAsync(
                resourceGroupName: resource.ResourceGroup,
                vmScaleSetName: resource.Name,
                parameters: parameters,
                cancellationToken: ct
            );
        }
    }
}
