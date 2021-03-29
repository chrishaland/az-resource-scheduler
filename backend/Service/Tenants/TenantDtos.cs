using Repository.Models;
using System;

namespace Service.Tenants
{
    public record UpsertTenantRequest(Guid? Id, string Name, string Description);

    public record GetTenantRequest(Guid Id);

    public record GetTenantResponse(TenantDto Tenant);

    public record ListTenantsRequest;

    public record ListTenantsResponse
    {
        public TenantDto[] Tenants { get; init; } = Array.Empty<TenantDto>();
    }

    public record TenantDto(Guid Id, string Name, string Description)
    {
        internal static TenantDto FromEntity(Tenant entity) => entity == null ? null : 
            new TenantDto(entity.Id, entity.Name, entity.Description);
    }
}
