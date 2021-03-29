using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Tenants
{
    [Route("api/tenant/list")]
    public class ListTenantsHandler : QueryHandlerBase<ListTenantsRequest, ListTenantsResponse>
    {
        private readonly DatabaseContext _context;

        public ListTenantsHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<ListTenantsResponse>> Execute([FromBody] ListTenantsRequest request, CancellationToken ct)
        {
            var entities = await _context.Tenants
                .AsNoTracking()
                .ToListAsync(ct);

            var Tenants = entities
                .Select(TenantDto.FromEntity)
                .ToArray();

            return Ok(new ListTenantsResponse
            {
                 Tenants = Tenants
            });
        }
    }
}
