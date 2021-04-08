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
    public class StopResourceJob
    {
        private readonly DatabaseContext _context;
        private readonly ComputeManagementClient _client;

        public StopResourceJob(DatabaseContext context, ComputeManagementClient client)
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
                    await StopVirtualMachine(vm, ct);
                    break;
                case VirtualMachineScaleSet vmss:
                    await StopVirtualMachineScaleSet(vmss, ct);
                    break;
                default:
                    return;
            }
        }

        private async Task StopVirtualMachine(VirtualMachine resource, CancellationToken ct)
        {
            await _client.VirtualMachines.StartPowerOffAsync(
                resourceGroupName: resource.ResourceGroup,
                vmName: resource.Name,
                cancellationToken: ct
            );
        }

        private async Task StopVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct)
        {
            var vmss = await _client.VirtualMachineScaleSets.GetAsync(
                resourceGroupName: resource.ResourceGroup,
                vmScaleSetName: resource.Name,
                cancellationToken: ct
            );

            vmss.Value.Sku.Capacity = 0;

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
