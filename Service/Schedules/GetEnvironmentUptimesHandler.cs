namespace Service.Schedules;

[Route("api/schedule/environment-uptimes/get")]
public class GetEnvironmentUptimesHandler : QueryHandlerBase<GetEnvironmentUptimesRequest, GetEnvironmentUptimesResponse>
{
    private readonly DatabaseContext _context;

    public GetEnvironmentUptimesHandler(DatabaseContext context)
    {
        _context = context;
    }
    
    public override async Task<ActionResult<GetEnvironmentUptimesResponse>> Execute([FromBody] GetEnvironmentUptimesRequest request, CancellationToken ct)
    {
        // We assume the resources are not stopped/restarted by other programs/users. Therefore, resources
        // scheduled to stop is by definition running.

        var resourcesCurrentlyRunning = await _context.ResourceStopJobs
            .AsNoTracking()
            .Include(rsj => rsj.Resource)
            .ToListAsync(ct);

        var resourcesCurrentlyRunningIds = resourcesCurrentlyRunning.Select(r => r.ResourceId);

        var environmentsCurrentlyRunning = await _context.Environments
            .AsNoTracking()
            .Include(e => e.Resources)
            .Where(e => e.Resources.Any(er => resourcesCurrentlyRunningIds.Any(id => id == er.Id)))
            .ToListAsync(ct);

        var environmentUptimes = new Dictionary<Guid, GetEnvironmentUptimesDto>();

        foreach(var environment in environmentsCurrentlyRunning)
        {
            var scheduledStopTime = resourcesCurrentlyRunning
                    .Where(r => environment.Resources.Any(er => er.Id == r.ResourceId))
                    .Select(r => r.StopAt)
                    .OrderBy(d => d)
                    .FirstOrDefault();

            environmentUptimes.Add(environment.Id, new GetEnvironmentUptimesDto(
                ScheduledStopTime: scheduledStopTime,
                ResourcesCount: environment.Resources.Count
            ));
        }

        return Ok(new GetEnvironmentUptimesResponse
        {
            EnvironmentUptimes = environmentUptimes
        });
    }
}
