using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Environment = Repository.Models.Environment;

namespace Service.Resources
{
    [Authorize(Policy = "admin")]
    [Route("api/resource/upsert")]
    public class UpsertResourceHandler : CommandHandlerBase<UpsertResourceRequest>
    {
        private readonly DatabaseContext _context;
        private readonly UpsertVirtualMachineHandler _vmHandler;
        private readonly UpsertVirtualMachineScaleSetHandler _vmssHandler;

        public UpsertResourceHandler(DatabaseContext context, UpsertVirtualMachineHandler vmHandler, UpsertVirtualMachineScaleSetHandler vmssHandler)
        {
            _context = context;
            _vmHandler = vmHandler;
            _vmssHandler = vmssHandler;
        }

        public override async Task<ActionResult<CommandResponse>> Execute([FromBody] UpsertResourceRequest request, CancellationToken ct)
        {
            var (isBadRequest, errorMessages) = await IsBadRequest(request, ct);
            if (isBadRequest)
            {
                return BadRequest(errorMessages);
            }

            var environments = new List<Environment>();
            if (request.EnvironmentIds?.Any() == true)
            {
                environments = await _context.Environments
                    .Where(e => request.EnvironmentIds.Contains(e.Id))
                    .ToListAsync();
            }

            Guid id;
            if (request.Id.HasValue)
            {
                if (request.VirtualMachineExtentions != null)
                {
                    id = await _vmHandler.Update(request, environments, ct);
                }
                else
                {
                    id = await _vmssHandler.Update(request, environments, ct);
                }
            }
            else
            {
                if (request.VirtualMachineExtentions != null)
                {
                    id = await _vmHandler.Create(request, environments, ct);
                }
                else
                {
                    id = await _vmssHandler.Create(request, environments, ct);
                }
            }

            return Ok(new CommandResponse(id.ToString()));
        }

        private async Task<(bool isBadRequest, string[] errorMessages)> IsBadRequest(UpsertResourceRequest request, CancellationToken ct)
        {
            var isBadRequest = false;
            var errorMessages = new List<string>();

            async Task<bool> IsIdInvalid()
            {
                if (!request.Id.HasValue) return false;
                if (request.Id.Equals(Guid.Empty)) return true;

                Resource resource = null;
                if (request.VirtualMachineExtentions != null)
                {
                    resource = await _context.VirtualMachines
                        .AsNoTracking()
                        .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), ct);
                }
                else
                {
                    resource = await _context.VirtualMachineScaleSets
                        .AsNoTracking()
                        .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), ct);
                }

                return resource == null;
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

            if (string.IsNullOrEmpty(request.ResourceGroup))
            {
                isBadRequest = true;
                errorMessages.Add("missing_resourceGroup");
            }

            if ((request.VirtualMachineExtentions == null && request.VirtualMachineScaleSetExtentions == null) ||
                (request.VirtualMachineExtentions != null && request.VirtualMachineScaleSetExtentions != null))
            {
                isBadRequest = true;
                errorMessages.Add("invalid_extentions, must provide either 'virtualMachineExtentions' or 'virtualMachineScaleSetExtentions'");
            }

            return (isBadRequest, errorMessages.ToArray());
        }
    }
}
