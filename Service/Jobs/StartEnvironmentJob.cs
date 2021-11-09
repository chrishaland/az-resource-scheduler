using Hangfire;
using Microsoft.Extensions.Logging;
using Nager.Date;

namespace Service.Jobs;

public class StartEnvironmentJob
{
    private readonly DatabaseContext _context;
    private readonly IBackgroundJobClient _backgroundJob;
    private readonly ILogger<StartEnvironmentJob> _logger;

    public StartEnvironmentJob(ILogger<StartEnvironmentJob> logger, DatabaseContext context, IBackgroundJobClient backgroundJob)
    {
        _logger = logger;
        _context = context;
        _backgroundJob = backgroundJob;
    }

    public async Task Execute(Guid environmentId, int uptimeInMinutes, bool allowWeekendRuns, bool allowHolidayRuns, DateTime? currentTime)
    {
        var now = DateTime.Now;
        if (currentTime.HasValue) now = currentTime.Value;

        var environment = await _context.Environments
            .AsNoTracking()
            .Include(e => e.Resources)
            .SingleOrDefaultAsync(e => e.Id.Equals(environmentId));

        if (environment == null) return;

        if (!allowWeekendRuns && DateSystem.IsWeekend(now, CountryCode.NO))
        {
            _logger.LogInformation("Scheduling environment start/stop for environment '{EnvironmentName}' ignored, it's weekend.", environment.Name);
            return;
        }

        if (!allowHolidayRuns && DateSystem.IsPublicHoliday(now, CountryCode.NO))
        {
            _logger.LogInformation("Scheduling environment start/stop for environment '{EnvironmentName}' ignored, it's holiday.", environment.Name);
            return;
        }

        _logger.LogInformation("Scheduling environment start/stop for environment '{EnvironmentName}'", environment.Name);

        var delay = TimeSpan.FromMinutes(uptimeInMinutes);
        var stopAt = now.Add(delay);

        foreach (var resource in environment.Resources)
        {
            await ScheduleResourceStartStop(resource.Id, delay, stopAt);
        }

        await _context.SaveChangesAsync();
    }

    private async Task ScheduleResourceStartStop(Guid resourceId, TimeSpan delay, DateTime stopAt)
    {
        // Start the resource immediately
        _backgroundJob.Enqueue<StartResourceJob>(job => job.Execute(resourceId));

        // Get list of already scheduled stop jobs for the resource
        var resourceStopJobs = await _context.ResourceStopJobs
            .Where(j => j.ResourceId.Equals(resourceId))
            .ToListAsync();

        // Get jobs that stop the resource before the new desired stop time
        // and remove them from the schedule.
        var invalidStopJobs = resourceStopJobs.Where(j => j.StopAt.CompareTo(stopAt) < 0);
        foreach (var invalidStopJob in invalidStopJobs)
        {
            _backgroundJob.Delete(invalidStopJob.JobId);
            _context.ResourceStopJobs.Remove(invalidStopJob);
        }

        // If the resouce is scheduled to stop at a later time than the desired
        // stop time, we return, keeing the already scheduled stop time.
        if (resourceStopJobs.Any(j => j.StopAt.CompareTo(stopAt) > 0)) return;

        // If not, we schedule the desired stop time and add it to the resource stop 
        // list for tracking. Add a default value of "" for testing purposes with in memory Hangfire.
        var jobId = _backgroundJob.Schedule<StopResourceJob>(job => job.Execute(resourceId), delay) ?? "";

        await _context.ResourceStopJobs.AddAsync(new ResourceStopJob
        {
            JobId = jobId,
            ResourceId = resourceId,
            StopAt = stopAt
        });
    }
}
