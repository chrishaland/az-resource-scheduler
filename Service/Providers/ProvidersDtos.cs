namespace Service.Providers;

public record AzureProviderDto(string TenantId, string SubscriptionId, string ClientId, string ClientSecret);

public record UpsertProviderRequest(
        Guid? Id,
        string Name,
        AzureProviderDto? AzureProviderExtentions
    );

public record GetProviderRequest(Guid Id);

public record GetProviderResponse(GetProviderDto Provider);

public record GetProviderDto(Guid Id, string Name, AzureProviderDto? AzureProviderExtentions)
{
    internal static GetProviderDto FromEntity(AzureProvider entity) => new(
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

public record ListProvidersRequest();

public record ListProvidersResponse
{
    public ListProvidersDto[] Providers { get; init; } = Array.Empty<ListProvidersDto>();
}

public record ListProvidersDto(Guid Id, string Name)
{
    internal static ListProvidersDto FromEntity(Provider entity) => new(
        Id: entity.Id,
        Name: entity.Name
    );
}
