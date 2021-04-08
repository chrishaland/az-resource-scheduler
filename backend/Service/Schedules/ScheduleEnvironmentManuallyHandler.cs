using Microsoft.AspNetCore.Mvc;
using Service.Jobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Schedules
{
    public record ScheduleEnvironmentManuallyRequest(Guid EnvironmentId, int UptimeInMinutes);

    [Route("api/schedule/start-environment-manually")]
    public class ScheduleEnvironmentManuallyHandler : CommandHandlerBase<ScheduleEnvironmentManuallyRequest>
    {
        private readonly StartEnvironmentJob _startEnvironmentJob;

        public ScheduleEnvironmentManuallyHandler(StartEnvironmentJob startEnvironmentJob)
        {
            _startEnvironmentJob = startEnvironmentJob;
        }

        public override async Task<ActionResult<CommandResponse>> Execute([FromBody] ScheduleEnvironmentManuallyRequest request, CancellationToken ct)
        {
            await _startEnvironmentJob.Execute(
                environmentId: request.EnvironmentId, 
                uptimeInMinutes: request.UptimeInMinutes,
                allowWeekendRuns: true,
                allowHolidayRuns: true
            );
            return Ok(new CommandResponse(request.EnvironmentId.ToString()));
        }
    }
}
