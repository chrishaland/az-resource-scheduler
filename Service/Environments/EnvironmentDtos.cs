namespace Service.Environments;

public record UpsertEnvironmentRequest(Guid? Id, string Name, string Description, string ScheduledStartup, int ScheduledUptime, string? TimeZoneId);

public record GetEnvironmentRequest(Guid Id);

public record GetEnvironmentResponse(EnvironmentDto Environment, TimeZoneDto[] TimeZones);

public record ListEnvironmentsRequest;

public record ListEnvironmentsResponse
{
    public EnvironmentDto[] Environments { get; init; } = Array.Empty<EnvironmentDto>();
}

public record TimeZoneDto(string Id);
public record EnvironmentDto(Guid Id, string Name, string Description, string ScheduledStartup, int ScheduledUptime)
{
    internal static EnvironmentDto FromEntity(Environment entity) => new(
        entity.Id,
        entity.Name,
        entity.Description,
        entity.ScheduledStartup,
        entity.ScheduledUptime
    );
}
