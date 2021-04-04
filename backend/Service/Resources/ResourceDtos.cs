using Repository.Models;
using System;

namespace Service.Resources
{
    public record UpsertResourceRequest(Guid? Id, string Name, string ResourceGroup, string Description, int Kind, Guid[] TenantIds, Guid[] EnvironmentIds);


    public record ListResourcesRequest(Guid? TenantId, Guid? EnvironmentId);

    public record ListResourcesResponse
    {
        public ResourceDto[] Resources { get; init; } = Array.Empty<ResourceDto>();
    }

    public record ResourceDto(Guid Id, string Name, string ResourceGroup, string Description, int Kind)
    {
        internal static ResourceDto FromEntity(Resource entity) => entity == null ? null : 
            new ResourceDto(entity.Id, entity.Name, entity.ResourceGroup, entity.Description, (int)entity.Kind);
    }
}
