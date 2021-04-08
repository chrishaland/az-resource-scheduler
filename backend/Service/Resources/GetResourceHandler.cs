using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Resources
{
    [Route("api/resource/get")]
    public class GetResourceHandler : QueryHandlerBase<GetResourceRequest, GetResourceResponse>
    {
        private readonly DatabaseContext _context;

        public GetResourceHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<GetResourceResponse>> Execute([FromBody] GetResourceRequest request, CancellationToken ct)
        {
            var entity = await _context.Resources
                .AsNoTracking()
                .Include(r => r.Environments)
                .Include(r => r.VirtualMachine)
                .Include(r => r.VirtualMachineScaleSet)
                .Where(e => e.Id.Equals(request.Id))
                .SingleOrDefaultAsync(ct);

            if (entity == null)
            {
                return NotFound();
            }

            ResourceDto dto;
            switch(entity)
            {
                case VirtualMachine vm:
                    dto = ResourceDto.FromEntity(vm);
                    break;
                case VirtualMachineScaleSet vmss:
                    dto = ResourceDto.FromEntity(vmss);
                    break;
                default:
                    return BadRequest();
            }

            return base.Ok(new GetResourceResponse(dto));
        }
    }
}
