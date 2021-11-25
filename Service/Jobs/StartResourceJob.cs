using Libraries.Providers;
using VirtualMachine = Repository.Models.VirtualMachine;
using VirtualMachineScaleSet = Repository.Models.VirtualMachineScaleSet;

namespace Service.Jobs;

public class StartResourceJob
{
    private readonly DatabaseContext _context;
    private readonly ProviderResourceHandlerDelegate _providerResourceHandlerDelegate;

    private IProviderResourceHandler? _providerResourceHandler;

    public StartResourceJob(DatabaseContext context, ProviderResourceHandlerDelegate providerResourceHandler)
    {
        _context = context;
        _providerResourceHandlerDelegate = providerResourceHandler;
    }

    public async Task Execute(Guid resourceId)
    {
        CancellationToken ct = default;

        var entity = await _context.Resources
           .AsNoTracking()
           .Include(r => r.Provider)
           .Include(r => r.VirtualMachine)
           .Include(r => r.VirtualMachineScaleSet)
           .Where(e => e.Id.Equals(resourceId))
           .SingleOrDefaultAsync(ct);

        if (entity?.Provider == null) return;

        _providerResourceHandler = _providerResourceHandlerDelegate(entity.Provider.GetType().Name);

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
        if (_providerResourceHandler == null) return;
        await _providerResourceHandler.StartVirtualMachine(resource, ct);
    }

    private async Task StartVirtualMachineScaleSet(VirtualMachineScaleSet resource, CancellationToken ct)
    {
        if (_providerResourceHandler == null) return;
        await _providerResourceHandler.StartVirtualMachineScaleSet(resource, ct);
    }
}
