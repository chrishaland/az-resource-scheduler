using Hangfire;
using Hangfire.Storage;

namespace Service.Jobs;

internal static class JobExtentions
{
    internal static RecurringJobDto? GetRecurringEnvironmentJob(Guid environmentId)
    {
        var storageConnection = JobStorage.Current.GetConnection();
        var recurringJobs = storageConnection.GetRecurringJobs();
        return recurringJobs?.SingleOrDefault(job => job.Id == EnvironmentJobName(environmentId));
    }

    internal static void AddOrUpdateEnvironmentJob(this IRecurringJobManager recurringJob, Environment environment, TimeZoneInfo timeZone)
    {
        var recurringJobName = EnvironmentJobName(environment.Id);

        recurringJob.RemoveIfExists(recurringJobName);
        if (environment.ScheduledUptime > 0)
        {
            recurringJob.AddOrUpdate<StartEnvironmentJob>(
                recurringJobId: recurringJobName,
                methodCall: job => job.Execute(environment.Id, environment.ScheduledUptime * 60, false, false, null),
                cronExpression: environment.ScheduledStartup,
                timeZone: timeZone);
        }
    }

    private static string EnvironmentJobName(Guid environmentId) => $"start_environment_{environmentId}";
}
