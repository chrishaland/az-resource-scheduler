namespace Service.Schedules;
public record ScheduleEnvironmentManuallyRequest(Guid EnvironmentId, int UptimeInMinutes);

public record GetEnvironmentUptimesRequest;

public record GetEnvironmentUptimesResponse
{
    public Dictionary<Guid, GetEnvironmentUptimesDto> EnvironmentUptimes { get; init; } = new Dictionary<Guid, GetEnvironmentUptimesDto>();
}

public record GetEnvironmentUptimesDto(DateTime ScheduledStopTime, int ResourcesCount);
