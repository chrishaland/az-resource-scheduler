using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Tenants
{
    [Route("api/tenant/get")]
    public class GetTenantHandler : QueryHandlerBase<GetTenantRequest, GetTenantResponse>
    {
        private readonly DatabaseContext _context;

        public GetTenantHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<GetTenantResponse>> Execute([FromBody] GetTenantRequest request, CancellationToken ct)
        {
            var entity = await _context.Tenants
                .AsNoTracking()
                .Where(e => e.Id.Equals(request.Id))
                .SingleOrDefaultAsync(ct);

            if (entity == null)
            {
                return NotFound();
            }

            var dto = TenantDto.FromEntity(entity);
            return base.Ok(new GetTenantResponse(dto));
        }
    }
}
