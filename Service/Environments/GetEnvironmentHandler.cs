using Microsoft.AspNetCore.Authorization;
using TimeZoneConverter;

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

        var dto = GetEnvironmentDto.FromEntity(entity);
        return base.Ok(new GetEnvironmentResponse(dto, TimeZones));
    }

    private static TimeZoneDto[] TimeZones => TZConvert.KnownWindowsTimeZoneIds
        .Select(tz => new TimeZoneDto(tz))
        .ToArray();
}
