namespace Service.Environments;

public record UpsertEnvironmentRequest(Guid? Id, string Name, string Description, string ScheduledStartup, int ScheduledUptime, string? TimeZoneId);

public record GetEnvironmentRequest(Guid Id);

public record GetEnvironmentResponse(GetEnvironmentDto Environment, TimeZoneDto[] TimeZones);

public record GetEnvironmentDto(Guid Id, string Name, string Description, string ScheduledStartup, int ScheduledUptime)
{
    internal static GetEnvironmentDto FromEntity(Environment entity) => new(
        entity.Id,
        entity.Name,
        entity.Description,
        entity.ScheduledStartup,
        entity.ScheduledUptime
    );
}

public record ListEnvironmentsRequest;

public record ListEnvironmentsResponse
{
    public ListEnvironmentsDto[] Environments { get; init; } = Array.Empty<ListEnvironmentsDto>();
}

public record ListEnvironmentsDto(Guid Id, string Name, string Description)
{
    internal static ListEnvironmentsDto FromEntity(Environment entity) => new(
        entity.Id,
        entity.Name,
        entity.Description
    );
}

public record TimeZoneDto(string Id);
