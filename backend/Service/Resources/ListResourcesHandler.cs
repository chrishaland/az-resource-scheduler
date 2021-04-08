using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Resources
{
    [Route("api/resource/list")]
    public class ListResourcesHandler : QueryHandlerBase<ListResourcesRequest, ListResourcesResponse>
    {
        private readonly DatabaseContext _context;

        public ListResourcesHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<ListResourcesResponse>> Execute([FromBody] ListResourcesRequest request, CancellationToken ct)
        {
            var queryable = _context.Resources
                .Include(r => r.Environments)
                .Include(r => r.VirtualMachine)
                .Include(r => r.VirtualMachineScaleSet)
                .AsNoTracking();

            if (request.EnvironmentId.HasValue)
            {
                queryable = queryable.Where(r => r.Environments.Any(e => e.Id.Equals(request.EnvironmentId.Value)));
            }

            var entities = await queryable
                .ToListAsync(ct);

            var environments = entities
                .Select(ToDto)
                .Where(dto => dto != null)
                .ToArray();

            return Ok(new ListResourcesResponse
            {
                 Resources = environments
            });
        }

        private ResourceDto ToDto(Resource entity)
        {
            switch (entity)
            {
                case VirtualMachine vm:
                    return ResourceDto.FromEntity(vm);
                case VirtualMachineScaleSet vmss:
                    return ResourceDto.FromEntity(vmss);
                default:
                    return null;
            }
        }
    }
}
