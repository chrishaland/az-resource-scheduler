namespace Service.Providers;

[Route("api/provider/get")]
public class GetProviderHandler : QueryHandlerBase<GetProviderRequest, GetProviderResponse>
{
    private readonly DatabaseContext _context;

    public GetProviderHandler(DatabaseContext context)
    {
        _context = context;
    }

    public override async Task<ActionResult<GetProviderResponse>> Execute([FromBody] GetProviderRequest request, CancellationToken ct)
    {
        var entity = await _context.Providers
            .Include(p => p.AzureProvider)
            .AsNoTracking()
            .Where(e => e.Id.Equals(request.Id))
            .SingleOrDefaultAsync(ct);

        if (entity == null)
        {
            return NotFound();
        }

        ProviderDto dto;
        switch (entity)
        {
            case AzureProvider vm:
                dto = ProviderDto.FromEntity(vm);
                break;
            default:
                return BadRequest();
        }

        return base.Ok(new GetProviderResponse(dto));
    }
}
