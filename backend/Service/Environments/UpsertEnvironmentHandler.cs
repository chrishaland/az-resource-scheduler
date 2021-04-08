using Cronos;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service.Jobs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Environment = Repository.Models.Environment;

namespace Service.Environments
{
    [Authorize(Policy = "admin")]
    [Route("api/environment/upsert")]
    public class UpsertEnvironmentHandler : CommandHandlerBase<UpsertEnvironmentRequest>
    {
        private readonly DatabaseContext _context;
        private readonly StartEnvironmentJob _startEnvironmentJob;

        public UpsertEnvironmentHandler(DatabaseContext context, StartEnvironmentJob startEnvironmentJob)
        {
            _context = context;
            _startEnvironmentJob = startEnvironmentJob;
        }

        public override async Task<ActionResult<CommandResponse>> Execute([FromBody] UpsertEnvironmentRequest request, CancellationToken ct)
        {
            var (isBadRequest, errorMessages) = await IsBadRequest(request, ct);
            if (isBadRequest)
            {
                return BadRequest(errorMessages);
            }

            Guid id;
            if (request.Id.HasValue)
            {
                id = await Update(request, ct);
                if (id.Equals(Guid.Empty)) return NotFound();
            }
            else
            {
                id = await Create(request, ct);
            }

            return Ok(new CommandResponse(id.ToString()));
        }

        private async Task<Guid> Create(UpsertEnvironmentRequest request, CancellationToken ct)
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
            AddOrUpdateRecurringJob(entity.Entity);
            return entity.Entity.Id;
        }

        private async Task<Guid> Update(UpsertEnvironmentRequest request, CancellationToken ct)
        {
            var entity = _context.Environments.Update(new Environment 
            {
                Id = request.Id.Value,
                Name = request.Name,
                Description = request.Description,
                ScheduledStartup = request.ScheduledStartup,
                ScheduledUptime = request.ScheduledUptime
            });

            await _context.SaveChangesAsync(ct);
            AddOrUpdateRecurringJob(entity.Entity);
            return entity.Entity.Id;
        }

        private void AddOrUpdateRecurringJob(Environment environment)
        {
            var recurringJobName = $"start_environment_{environment.Id}";

            RecurringJob.RemoveIfExists(recurringJobName);
            if (environment.ScheduledUptime > 0)
            {
                RecurringJob.AddOrUpdate(
                    recurringJobId: recurringJobName,
                    methodCall: () => _startEnvironmentJob.Execute(environment.Id, environment.ScheduledUptime * 60, false, false),
                    cronExpression: environment.ScheduledStartup,
                    timeZone: TimeZoneInfo.Local);
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

            return (isBadRequest, errorMessages.ToArray());
        }

        private static bool IsCronExpressionValid(string cronExpression)
        {
            try
            {
                CronExpression.Parse(cronExpression);
            }
            catch(CronFormatException)
            {
                return false;
            }

            return true;
        }
    }
}
