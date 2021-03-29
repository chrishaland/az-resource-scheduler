using Repository.Models;
using System;

namespace Service.Resources
{
    public record UpsertResourceRequest(Guid? Id, string Name, string Description, string AzureId, int Kind, Guid[]TenantIds, Guid[]EnvironmentIds);

    public record QueryResourceRequest(Guid Id);

    public record QueryResourceResponse(ResourceDto Resource);

    public record QueryResourcesRequest(Guid? TenantId, Guid? EnvironmentId);

    public record QueryResourcesResponse
    {
        public ResourceDto[] Resources { get; init; } = Array.Empty<ResourceDto>();
    }

    public record ResourceDto(Guid Id, string Name, string Description, string AzureId, int Kind)
    {
        internal static ResourceDto FromEntity(Resource entity) => entity == null ? null : 
            new ResourceDto(entity.Id, entity.Name, entity.Description, entity.AzureId, (int)entity.Kind);
    }
}
