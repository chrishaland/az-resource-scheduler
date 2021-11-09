namespace Service.Resources;

public record UpsertResourceRequest(
        Guid? Id,
        string Name,
        string ResourceGroup,
        string Description,
        Guid ProviderId,
        VirtualMachineDto? VirtualMachineExtentions,
        VirtualMachineScaleSetDto? VirtualMachineScaleSetExtentions,
        Guid[] EnvironmentIds
    );

public record GetResourceRequest(Guid Id);
public record GetResourceResponse(ResourceDto Resource);

public record ListResourcesRequest(Guid? EnvironmentId, Guid? ProviderId);
public record ListResourcesResponse
{
    public ResourceDto[] Resources { get; init; } = Array.Empty<ResourceDto>();
}

public record ResourceDto(
    Guid Id,
    string Name,
    string ResourceGroup,
    string Description,
    Guid ProviderId,
    Guid[] EnvironmentIds,
    VirtualMachineDto? VirtualMachineExtentions,
    VirtualMachineScaleSetDto? VirtualMachineScaleSetExtentions
)
{
    internal static ResourceDto FromEntity(VirtualMachine entity) => new(
            Id: entity.Id,
            Name: entity.Name,
            ResourceGroup: entity.ResourceGroup,
            Description: entity.Description,
            ProviderId: entity.Provider?.Id ?? Guid.Empty,
            EnvironmentIds: entity.Environments?.Select(e => e.Id).ToArray() ?? Array.Empty<Guid>(),
            VirtualMachineExtentions: new VirtualMachineDto(),
            VirtualMachineScaleSetExtentions: null
        );

    internal static ResourceDto FromEntity(VirtualMachineScaleSet entity) => new(
            Id: entity.Id,
            Name: entity.Name,
            ResourceGroup: entity.ResourceGroup,
            Description: entity.Description,
            ProviderId: entity.Provider?.Id ?? Guid.Empty,
            EnvironmentIds: entity.Environments?.Select(e => e.Id).ToArray() ?? Array.Empty<Guid>(),
            VirtualMachineExtentions: null,
            VirtualMachineScaleSetExtentions: new VirtualMachineScaleSetDto(
                Capacity: entity.Capacity
            )
        );
}

public record VirtualMachineDto;
public record VirtualMachineScaleSetDto(int? Capacity);
