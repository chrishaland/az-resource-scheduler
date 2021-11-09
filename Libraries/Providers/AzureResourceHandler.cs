using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Repository.Models;
using VirtualMachine = Repository.Models.VirtualMachine;
using VirtualMachineScaleSet = Repository.Models.VirtualMachineScaleSet;

namespace Libraries.Providers;

public class AzureResourceHandler : IProviderResourceHandler
{
    public async Task StartVirtualMachine(VirtualMachine resource, CancellationToken ct)
    {
        if (resource.Provider?.AzureProvider == null) return;

        var resourceGroup = await GetResourceGroup(resource.ResourceGroup, resource.Provider.AzureProvider, ct);
        
        var vm = await resourceGroup.GetVirtualMachines().GetAsync(resource.Name, null, ct);
        await vm.Value.PowerOnAsync(waitForCompletion: false, ct);
    }

    public async Task StopVirtualMachine(VirtualMachine resource, CancellationToken ct)
    {
        if (resource.Provider?.AzureProvider == null) return;

        var resourceGroup = await GetResourceGroup(resource.ResourceGroup, resource.Provider.AzureProvider, ct);

        var vm = await resourceGroup.GetVirtualMachines().GetAsync(resource.Name, null, ct);
        await vm.Value.DeallocateAsync(waitForCompletion: false, ct);
    }

    public async Task StartVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct)
    {
        if (resource.Provider?.AzureProvider == null) return;

        var resourceGroup = await GetResourceGroup(resource.ResourceGroup, resource.Provider.AzureProvider, ct);

        var vmss = await resourceGroup.GetVirtualMachineScaleSets().GetAsync(resource.Name, null, ct);
        
        var skus = vmss.Value.GetSkus(ct);
        var sku = skus.FirstOrDefault()?.Sku ?? new Sku();
        sku.Capacity = resource.Capacity;

        var parameters = new VirtualMachineScaleSetUpdate
        {
            Sku = sku
        };

        await vmss.Value.UpdateAsync(parameters, waitForCompletion: false, ct);
    }

    public async Task StopVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct)
    {
        if (resource.Provider?.AzureProvider == null) return;

        var resourceGroup = await GetResourceGroup(resource.ResourceGroup, resource.Provider.AzureProvider, ct);

        var vmss = await resourceGroup.GetVirtualMachineScaleSets().GetAsync(resource.Name, null, ct);

        var skus = vmss.Value.GetSkus(ct);
        var sku = skus.FirstOrDefault()?.Sku ?? new Sku();
        sku.Capacity = 0;

        var parameters = new VirtualMachineScaleSetUpdate
        {
            Sku = sku
        };

        await vmss.Value.UpdateAsync(parameters, waitForCompletion: false, ct);
    }

    private static async Task<Azure.ResourceManager.Resources.ResourceGroup?> GetResourceGroup(string name, AzureProvider az, CancellationToken ct)
    {
        var credentials = new ClientSecretCredential(
            tenantId: az.TenantId,
            clientId: az.ClientId,
            clientSecret: az.ClientSecret
        );

        var armClient = new ArmClient(credentials);
        var subscription = armClient?.GetSubscription(new ResourceIdentifier(az.SubscriptionId));

        if (subscription == null) return null;

        var resourceGroup = await subscription.GetResourceGroups().GetAsync(name, ct);
        return resourceGroup?.Value;
    }
}
