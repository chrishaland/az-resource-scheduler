namespace Service.Providers;

[Route("api/provider/list")]
public class ListResourcesHandler : QueryHandlerBase<ListProvidersRequest, ListProvidersResponse>
{
    private readonly DatabaseContext _context;

    public ListResourcesHandler(DatabaseContext context)
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
            .Select(ToDto)
            .Where(dto => dto != null)
            .ToArray();

        return Ok(new ListProvidersResponse
        {
            Providers = providers
        });
    }

    private static ProviderDto ToDto(Provider entity) => entity switch
    {
        AzureProvider az => ProviderDto.FromEntity(az),
        _ => new ProviderDto(
            Id: Guid.Empty,
            Name: string.Empty,
            AzureProviderExtentions: null
        ),
    };
}
