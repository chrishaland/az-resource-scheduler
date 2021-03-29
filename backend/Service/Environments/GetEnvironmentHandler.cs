using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Environments
{
    [Route("api/environment/get")]
    public class GetEnvironmentHandler : QueryHandlerBase<GetEnvironmentRequest, GetEnvironmentResponse>
    {
        private readonly DatabaseContext _context;

        public GetEnvironmentHandler(DatabaseContext context)
        {
            _context = context;
        }

        public override async Task<ActionResult<GetEnvironmentResponse>> Execute([FromBody] GetEnvironmentRequest request, CancellationToken ct)
        {
            var entity = await _context.Environments
                .AsNoTracking()
                .Where(e => e.Id.Equals(request.Id))
                .SingleOrDefaultAsync(ct);

            if (entity == null)
            {
                return NotFound();
            }

            var dto = EnvironmentDto.FromEntity(entity);
            return base.Ok(new GetEnvironmentResponse(dto));
        }
    }
}
