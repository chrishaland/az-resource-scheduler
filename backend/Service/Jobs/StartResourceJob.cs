using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualMachine = Repository.Models.VirtualMachine;
using VirtualMachineScaleSet = Repository.Models.VirtualMachineScaleSet;

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

        public async Task Execute(Guid resourceId)
        {
            CancellationToken ct = default;

            var entity = await _context.Resources
               .AsNoTracking()
               .Include(r => r.VirtualMachine)
               .Include(r => r.VirtualMachineScaleSet)
               .Where(e => e.Id.Equals(resourceId))
               .SingleOrDefaultAsync(ct);

            if (entity == null) return;

            switch (entity)
            {
                case VirtualMachine vm:
                    await StartVirtualMachine(vm, ct);
                    break;
                case VirtualMachineScaleSet vmss:
                    await StartVirtualMachineScaleSet(vmss, ct);
                    break;
                default:
                    return;
            }
        }

        private async Task StartVirtualMachine(VirtualMachine resource, CancellationToken ct)
        {
            await _client.VirtualMachines.StartStartAsync(
                resourceGroupName: resource.ResourceGroup,
                vmName: resource.Name,
                cancellationToken: ct
           );
        }

        private async Task StartVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct)
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
