using Cronos;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
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

        public UpsertEnvironmentHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<CommandResponse>> Execute([FromBody] UpsertEnvironmentRequest request, CancellationToken ct)
        {
            if (await IsBadRequest(request, ct))
            {
                return BadRequest();
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
            return entity.Entity.Id;
        }

        private async Task<bool> IsBadRequest(UpsertEnvironmentRequest request, CancellationToken ct)
        {
            // Validate request data
            if (request.Id.Equals(Guid.Empty)) return true;
            if (string.IsNullOrEmpty(request.Name)) return true;
            if (!string.IsNullOrEmpty(request.ScheduledStartup) && !IsCronExpressionValid(request.ScheduledStartup)) return true;

            async Task<bool> EntityExists(Guid id, CancellationToken ct)
            {
                var environment = await _context.Environments
                    .AsNoTracking()
                    .SingleOrDefaultAsync(e => e.Id.Equals(id), ct);
                return environment == null;
            }

            // Verify that entity with id exists if updating
            if (request.Id.HasValue && (await EntityExists(request.Id.Value, ct))) return true;

            return false;
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
