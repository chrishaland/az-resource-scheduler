namespace Service.Resources;

[Route("api/resource/list")]
public class ListResourcesHandler : QueryHandlerBase<ListResourcesRequest, ListResourcesResponse>
{
    private readonly DatabaseContext _context;

    public ListResourcesHandler(DatabaseContext context)
    {
        _context = context;
    }

    public override async Task<ActionResult<ListResourcesResponse>> Execute([FromBody] ListResourcesRequest request, CancellationToken ct)
    {
        var queryable = _context.Resources
            .Include(r => r.Provider)
            .Include(r => r.Environments)
            .Include(r => r.VirtualMachine)
            .Include(r => r.VirtualMachineScaleSet)
            .AsNoTracking();

        if (request.EnvironmentId.HasValue)
        {
            queryable = queryable.Where(r => r.Environments.Any(e => e.Id.Equals(request.EnvironmentId.Value)));
        }

        var entities = await queryable
            .ToListAsync(ct);

        var environments = entities
            .Select(ToDto)
            .Where(dto => dto != null)
            .ToArray();

        return Ok(new ListResourcesResponse
        {
            Resources = environments
        });
    }

    private static ResourceDto ToDto(Resource entity) => entity switch
    {
        VirtualMachine vm => ResourceDto.FromEntity(vm),
        VirtualMachineScaleSet vmss => ResourceDto.FromEntity(vmss),
        _ => new ResourceDto(
            Id: Guid.Empty,
            Name: string.Empty,
            ResourceGroup: string.Empty,
            Description: string.Empty,
            ProviderId: Guid.Empty,
            EnvironmentIds: Array.Empty<Guid>(),
            VirtualMachineExtentions: null,
            VirtualMachineScaleSetExtentions: null
        ),
    };
}
