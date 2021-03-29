using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Environments
{
    [Route("api/environment/list")]
    public class ListEnvironmentsHandler : QueryHandlerBase<ListEnvironmentsRequest, ListEnvironmentsResponse>
    {
        private readonly DatabaseContext _context;

        public ListEnvironmentsHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<ListEnvironmentsResponse>> Execute([FromBody] ListEnvironmentsRequest request, CancellationToken ct)
        {
            var entities = await _context.Environments
                .AsNoTracking()
                .ToListAsync(ct);

            var environments = entities
                .Select(EnvironmentDto.FromEntity)
                .ToArray();

            return Ok(new ListEnvironmentsResponse
            {
                 Environments = environments
            });
        }
    }
}
