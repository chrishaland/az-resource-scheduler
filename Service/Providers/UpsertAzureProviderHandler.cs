namespace Service.Providers;

public class UpsertAzureProviderHandler
{
    private readonly DatabaseContext _context;

    public UpsertAzureProviderHandler(DatabaseContext context)
    {
        _context = context;
    }

    internal async Task<Guid> Create(UpsertProviderRequest request, CancellationToken ct)
    {
        var Provider = new AzureProvider
        {
            Name = request.Name,
            TenantId = request.AzureProviderExtentions?.TenantId ?? string.Empty,
            SubscriptionId = request.AzureProviderExtentions?.SubscriptionId ?? string.Empty,
            ClientId = request.AzureProviderExtentions?.ClientId ?? string.Empty,
            ClientSecret = request.AzureProviderExtentions?.ClientSecret ?? string.Empty
        };

        var entity = await _context.AzureProviders.AddAsync(Provider, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Entity.Id;
    }

    internal async Task<Guid> Update(UpsertProviderRequest request, CancellationToken ct)
    {
        if (!request.Id.HasValue) return Guid.Empty;

        var provider = await _context.AzureProviders
            .SingleAsync(v => v.Id.Equals(request.Id.Value), ct);

        provider.Name = request.Name;
        provider.TenantId = request.AzureProviderExtentions?.TenantId ?? string.Empty;
        provider.SubscriptionId = request.AzureProviderExtentions?.SubscriptionId ?? string.Empty;
        provider.ClientId = request.AzureProviderExtentions?.ClientId ?? string.Empty;
        provider.ClientSecret = request.AzureProviderExtentions?.ClientSecret ?? string.Empty;

        var entity = _context.AzureProviders.Update(provider);

        await _context.SaveChangesAsync(ct);
        return entity.Entity.Id;
    }
}
