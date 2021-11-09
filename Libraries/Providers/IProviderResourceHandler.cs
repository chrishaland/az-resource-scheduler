using Repository.Models;

namespace Libraries.Providers;

public interface IProviderResourceHandler
{
    Task StartVirtualMachine(VirtualMachine resource, CancellationToken ct);
    Task StopVirtualMachine(VirtualMachine resource, CancellationToken ct);
    Task StartVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct);
    Task StopVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct);
}

public delegate IProviderResourceHandler ProviderResourceHandlerDelegate(string key);
