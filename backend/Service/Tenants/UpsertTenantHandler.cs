using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Tenants
{
    [Authorize(Policy = "admin")]
    [Route("api/tenant/upsert")]
    public class UpsertTenantHandler : CommandHandlerBase<UpsertTenantRequest>
    {
        private readonly DatabaseContext _context;

        public UpsertTenantHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<CommandResponse>> Execute([FromBody] UpsertTenantRequest request, CancellationToken ct)
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

        private async Task<Guid> Create(UpsertTenantRequest request, CancellationToken ct)
        {
            var tenant = new Tenant
            {
                Name = request.Name,
                Description = request.Description
            };

            var entity = await _context.Tenants.AddAsync(tenant, ct);
            await _context.SaveChangesAsync(ct);
            return entity.Entity.Id;
        }

        private async Task<Guid> Update(UpsertTenantRequest request, CancellationToken ct)
        {
            var entity = _context.Tenants.Update(new Tenant
            {
                Id = request.Id.Value,
                Name = request.Name,
                Description = request.Description
            });

            await _context.SaveChangesAsync(ct);
            return entity.Entity.Id;
        }

        private async Task<bool> IsBadRequest(UpsertTenantRequest request, CancellationToken ct)
        {
            // Validate request data
            if (request.Id.Equals(Guid.Empty)) return true;
            if (string.IsNullOrEmpty(request.Name)) return true;

            async Task<bool> EntityExists(Guid id, CancellationToken ct)
            {
                var environment = await _context.Tenants
                    .AsNoTracking()
                    .SingleOrDefaultAsync(e => e.Id.Equals(id), ct);
                return environment == null;
            }

            // Verify that entity with id exists if updating
            if (request.Id.HasValue && (await EntityExists(request.Id.Value, ct))) return true;

            return false;
        }
    }
}
