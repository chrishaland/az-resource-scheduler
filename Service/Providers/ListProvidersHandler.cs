namespace Service.Providers;

[Route("api/provider/list")]
public class ListProvidersHandler : QueryHandlerBase<ListProvidersRequest, ListProvidersResponse>
{
    private readonly DatabaseContext _context;

    public ListProvidersHandler(DatabaseContext context)
    {
        _context = context;
    }

    public override async Task<ActionResult<ListProvidersResponse>> Execute([FromBody] ListProvidersRequest request, CancellationToken ct)
    {
        var entities = await _context.Providers
            .Include(p => p.AzureProvider)
            .AsNoTracking()
            .ToListAsync(ct);

        var providers = entities
            .Select(ListProvidersDto.FromEntity)
            .ToArray();

        return Ok(new ListProvidersResponse
        {
            Providers = providers
        });
    }
}
