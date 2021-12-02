using Microsoft.AspNetCore.Authorization;

namespace Service.Environments;

[Authorize(Policy = "admin")]
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

        var recurringEnvironmentJob = JobExtentions.GetRecurringEnvironmentJob(entity.Id);
        var timeZone = TimeZoneExtentions.GetTimeZoneIdOrDefault(recurringEnvironmentJob?.TimeZoneId);

        var dto = GetEnvironmentDto.FromEntity(entity, timeZone);
        return base.Ok(new GetEnvironmentResponse(dto));
    }
}
