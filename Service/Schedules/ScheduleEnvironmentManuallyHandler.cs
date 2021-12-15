namespace Service.Schedules;

[Route("api/schedule/start-environment-manually")]
public class ScheduleEnvironmentManuallyHandler : CommandHandlerBase<ScheduleEnvironmentManuallyRequest>
{
    private readonly StartEnvironmentJob _startEnvironmentJob;
    private readonly ILogger<ScheduleEnvironmentManuallyHandler> _logger;

    public ScheduleEnvironmentManuallyHandler(StartEnvironmentJob startEnvironmentJob, ILogger<ScheduleEnvironmentManuallyHandler> logger)
    {
        _startEnvironmentJob = startEnvironmentJob;
        _logger = logger;
    }

    public override async Task<ActionResult<CommandResponse>> Execute([FromBody] ScheduleEnvironmentManuallyRequest request, CancellationToken ct)
    {
        _logger.LogInformation("User '{User}' scheduled environment '{Environment}' to run for {Uptime} minutes",
            HttpContext.User?.Identity?.Name, request.EnvironmentId, request.UptimeInMinutes);

        var (isBadRequest, errorMessages) = IsBadRequest(request, ct);
        if (isBadRequest)
        {
            return BadRequest(errorMessages);
        }

        await _startEnvironmentJob.Execute(
            environmentId: request.EnvironmentId,
            uptimeInMinutes: request.UptimeInMinutes,
            allowWeekendRuns: true,
            allowHolidayRuns: true,
            currentTime: null
        );
        return Ok(new CommandResponse(request.EnvironmentId.ToString()));
    }

    private (bool isBadRequest, string[] errorMessages) IsBadRequest(ScheduleEnvironmentManuallyRequest request, CancellationToken ct)
    {
        var isBadRequest = false;
        var errorMessages = new List<string>();

        if (request.UptimeInMinutes <= 0 || request.UptimeInMinutes > 1440)
        {
            isBadRequest = true;
            errorMessages.Add("invalid_uptime_in_minutes");
        }

        return (isBadRequest, errorMessages.ToArray());
    }
}
