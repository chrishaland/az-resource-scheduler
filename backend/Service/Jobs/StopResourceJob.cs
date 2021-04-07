using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Repository.Models;
using System.Threading;
using System.Threading.Tasks;
using Resource = Repository.Models.Resource;

namespace Service.Jobs
{
    public class StopResourceJob
    {
        private readonly ComputeManagementClient _client;

        public StopResourceJob(ComputeManagementClient client)
        {
            _client = client;
        }

        public async Task Execute(Resource resource, CancellationToken ct)
        {
            //TODO
        }

        private async Task StopVirtualMachine(Resource resource, CancellationToken ct = default)
        {
            await _client.VirtualMachines.StartPowerOffAsync(
                resourceGroupName: resource.ResourceGroup,
                vmName: resource.Name,
                cancellationToken: ct
            );
        }

        private async Task StopNodePool(Resource resource, CancellationToken ct)
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
