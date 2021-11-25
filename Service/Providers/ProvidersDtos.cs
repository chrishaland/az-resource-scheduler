namespace Service.Providers;

public record UpsertProviderRequest(
        Guid? Id,
        string Name,
        AzureProviderDto? AzureProviderExtentions
    );

public record GetProviderRequest(Guid Id);
public record GetProviderResponse(ProviderDto Provider);

public record ListProvidersRequest();
public record ListProvidersResponse
{
    public ProviderDto[] Providers { get; init; } = Array.Empty<ProviderDto>();
}

public record ProviderDto(
    Guid Id,
    string Name,
    AzureProviderDto? AzureProviderExtentions
)
{
    internal static ProviderDto FromEntity(AzureProvider entity) => new(
        Id: entity.Id,
        Name: entity.Name,
        AzureProviderExtentions: new AzureProviderDto(
            TenantId: entity.TenantId,
            SubscriptionId: entity.SubscriptionId,
            ClientId: entity.ClientId,
            ClientSecret: entity.ClientSecret
        )
    );
}

public record AzureProviderDto(
    string TenantId,
    string SubscriptionId,
    string ClientId,
    string ClientSecret
);
