using Repository.Models;
using System;

namespace Service.Resources
{
    public record UpsertResourceRequest(
        Guid? Id, 
        string Name, 
        string ResourceGroup, 
        string Description, 
        VirtualMachineDto VirtualMachineExtentions,
        VirtualMachineScaleSetDto VirtualMachineScaleSetExtentions, 
        Guid[] EnvironmentIds
    );

    public record GetResourceRequest(Guid Id);
    public record GetResourceResponse(ResourceDto Resource);

    public record ListResourcesRequest(Guid? EnvironmentId);
    public record ListResourcesResponse
    {
        public ResourceDto[] Resources { get; init; } = Array.Empty<ResourceDto>();
    }

    public record ResourceDto(
        Guid Id, 
        string Name,
        string ResourceGroup, 
        string Description,
        VirtualMachineDto VirtualMachineExtentions,
        VirtualMachineScaleSetDto VirtualMachineScaleSetExtentions
    )
    {
        internal static ResourceDto FromEntity(VirtualMachine entity) => entity == null ? null :
            new ResourceDto(
                Id: entity.Id,
                Name: entity.Name,
                ResourceGroup: entity.ResourceGroup,
                Description: entity.Description,
                VirtualMachineExtentions: new VirtualMachineDto(),
                VirtualMachineScaleSetExtentions: null
            );

        internal static ResourceDto FromEntity(VirtualMachineScaleSet entity) => entity == null ? null :
            new ResourceDto(
                Id: entity.Id,
                Name: entity.Name,
                ResourceGroup: entity.ResourceGroup,
                Description: entity.Description,
                VirtualMachineExtentions: null,
                VirtualMachineScaleSetExtentions: new VirtualMachineScaleSetDto(
                    Capacity: entity.Capacity
                )
            );
    }

    public record VirtualMachineDto;
    public record VirtualMachineScaleSetDto(int? Capacity);
}
