using Cronos;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Service.Jobs;
using TimeZoneConverter;

namespace Service.Environments;

[Authorize(Policy = "admin")]
[Route("api/environment/upsert")]
public class UpsertEnvironmentHandler : CommandHandlerBase<UpsertEnvironmentRequest>
{
    private readonly DatabaseContext _context;
    private readonly IRecurringJobManager _recurringJob;

    public UpsertEnvironmentHandler(DatabaseContext context, IRecurringJobManager recurringJob)
    {
        _context = context;
        _recurringJob = recurringJob;
    }

    public override async Task<ActionResult<CommandResponse>> Execute([FromBody] UpsertEnvironmentRequest request, CancellationToken ct)
    {
        var (isBadRequest, errorMessages) = await IsBadRequest(request, ct);
        if (isBadRequest)
        {
            return BadRequest(errorMessages);
        }

        var timeZone = TZConvert.GetTimeZoneInfo(
            string.IsNullOrEmpty(request.TimeZoneId) ? "W. Europe Standard Time" : request.TimeZoneId);

        Guid id;
        if (request.Id.HasValue)
        {
            id = await Update(request, timeZone, ct);
            if (id.Equals(Guid.Empty)) return NotFound();
        }
        else
        {
            id = await Create(request, timeZone, ct);
        }

        return Ok(new CommandResponse(id.ToString()));
    }

    private async Task<Guid> Create(UpsertEnvironmentRequest request, TimeZoneInfo timeZone, CancellationToken ct)
    {
        var environment = new Environment
        {
            Name = request.Name,
            Description = request.Description,
            ScheduledStartup = request.ScheduledStartup,
            ScheduledUptime = request.ScheduledUptime
        };

        var entity = await _context.Environments.AddAsync(environment, ct);
        await _context.SaveChangesAsync(ct);
        AddOrUpdateRecurringJob(entity.Entity, timeZone);
        return entity.Entity.Id;
    }

    private async Task<Guid> Update(UpsertEnvironmentRequest request, TimeZoneInfo timeZone, CancellationToken ct)
    {
        if (!request.Id.HasValue) return Guid.Empty;

        var entity = _context.Environments.Update(new Environment
        {
            Id = request.Id.Value,
            Name = request.Name,
            Description = request.Description,
            ScheduledStartup = request.ScheduledStartup,
            ScheduledUptime = request.ScheduledUptime
        });

        await _context.SaveChangesAsync(ct);
        AddOrUpdateRecurringJob(entity.Entity, timeZone);
        return entity.Entity.Id;
    }

    private void AddOrUpdateRecurringJob(Environment environment, TimeZoneInfo timeZone)
    {
        var recurringJobName = $"start_environment_{environment.Id}";

        _recurringJob.RemoveIfExists(recurringJobName);
        if (environment.ScheduledUptime > 0)
        {
            _recurringJob.AddOrUpdate<StartEnvironmentJob>(
                recurringJobId: recurringJobName,
                methodCall: job => job.Execute(environment.Id, environment.ScheduledUptime * 60, false, false, null),
                cronExpression: environment.ScheduledStartup,
                timeZone: timeZone);
        }
    }

    private async Task<(bool isBadRequest, string[] errorMessages)> IsBadRequest(UpsertEnvironmentRequest request, CancellationToken ct)
    {
        var isBadRequest = false;
        var errorMessages = new List<string>();

        async Task<bool> IsIdInvalid()
        {
            if (!request.Id.HasValue) return false;
            if (request.Id.Equals(Guid.Empty)) return true;

            var environment = await _context.Environments
                    .AsNoTracking()
                    .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), ct);

            return environment == null;
        }

        // Validate request data
        if (await IsIdInvalid())
        {
            isBadRequest = true;
            errorMessages.Add("invalid_id");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            isBadRequest = true;
            errorMessages.Add("missing_name");
        }

        if (!string.IsNullOrEmpty(request.ScheduledStartup) && !IsCronExpressionValid(request.ScheduledStartup))
        {
            isBadRequest = true;
            errorMessages.Add("invalid_scheduledStartup");
        }

        if (!string.IsNullOrEmpty(request.TimeZoneId) &&
            !TZConvert.TryGetTimeZoneInfo(request.TimeZoneId, out var _))
        {
            isBadRequest = true;
            errorMessages.Add("invalid_time_zone_id");
        }

        return (isBadRequest, errorMessages.ToArray());
    }

    private static bool IsCronExpressionValid(string cronExpression)
    {
        try
        {
            CronExpression.Parse(cronExpression);
        }
        catch (CronFormatException)
        {
            return false;
        }

        return true;
    }
}
