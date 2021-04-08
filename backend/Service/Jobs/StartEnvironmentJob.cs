using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nager.Date;
using Repository;
using System;
using System.Threading.Tasks;

namespace Service.Jobs
{
    public class StartEnvironmentJob
    {
        private readonly ILogger<StartEnvironmentJob> _logger;
        private readonly DatabaseContext _context;
        private readonly StartResourceJob _startResourceJob;
        private readonly StopResourceJob _stopResourceJob;

        public StartEnvironmentJob(ILogger<StartEnvironmentJob> logger, DatabaseContext context, StartResourceJob startResourceJob, StopResourceJob stopResourceJob)
        {
            _logger = logger;
            _context = context;
            _startResourceJob = startResourceJob;
            _stopResourceJob = stopResourceJob;
        }

        public async Task Execute(Guid environmentId, int uptimeInMinutes, bool allowWeekendRuns, bool allowHolidayRuns)
        {
            var environment = await _context.Environments
                .Include(e => e.Resources)
                .SingleOrDefaultAsync(e => e.Id.Equals(environmentId));

            if (environment == null) return;

            if (!allowWeekendRuns && DateSystem.IsWeekend(DateTime.Now, CountryCode.NO))
            {
                _logger.LogInformation("Scheduling environment start/stop for environment '{EnvironmentName}' ignored, it's weekend.", environment.Name);
                return;
            }

            if (!allowHolidayRuns && DateSystem.IsPublicHoliday(DateTime.Now, CountryCode.NO))
            {
                _logger.LogInformation("Scheduling environment start/stop for environment '{EnvironmentName}' ignored, it's holiday.", environment.Name);
                return;
            }

            _logger.LogInformation("Scheduling environment start/stop for environment '{EnvironmentName}'", environment.Name);

            foreach (var resource in environment.Resources)
            {
                BackgroundJob.Enqueue(() => _startResourceJob.Execute(resource.Id));
                BackgroundJob.Schedule(() => _stopResourceJob.Execute(resource.Id), TimeSpan.FromMinutes(uptimeInMinutes));
            }
        }
    }
}
