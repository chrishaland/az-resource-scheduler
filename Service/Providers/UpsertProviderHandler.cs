using Microsoft.AspNetCore.Authorization;

namespace Service.Providers;

[Authorize(Policy = "admin")]
[Route("api/provider/upsert")]
public class UpsertProviderHandler : CommandHandlerBase<UpsertProviderRequest>
{
    private readonly DatabaseContext _context;
    private readonly UpsertAzureProviderHandler _azureHandler;

    public UpsertProviderHandler(DatabaseContext context, UpsertAzureProviderHandler azureHandler)
    {
        _context = context;
        _azureHandler = azureHandler;
    }

    public override async Task<ActionResult<CommandResponse>> Execute([FromBody] UpsertProviderRequest request, CancellationToken ct)
    {
        var (isBadRequest, errorMessages) = await IsBadRequest(request, ct);
        if (isBadRequest)
        {
            return BadRequest(errorMessages);
        }

        Guid id;
        if (request.Id.HasValue)
        {
            id = await _azureHandler.Update(request, ct);
        }
        else
        {
            id = await _azureHandler.Create(request, ct);
        }

        return Ok(new CommandResponse(id.ToString()));
    }

    private async Task<(bool isBadRequest, string[] errorMessages)> IsBadRequest(UpsertProviderRequest request, CancellationToken ct)
    {
        var isBadRequest = false;
        var errorMessages = new List<string>();

        async Task<bool> IsIdInvalid()
        {
            if (!request.Id.HasValue) return false;
            if (request.Id.Equals(Guid.Empty)) return true;

            Provider? provider = null;
            if (request.AzureProviderExtentions != null)
            {
                provider = await _context.AzureProviders
                    .AsNoTracking()
                    .SingleOrDefaultAsync(e => e.Id.Equals(request.Id.Value), ct);
            }

            return provider == null;
        }

        // Validate request data
        if (await IsIdInvalid())
        {
            isBadRequest = true;
            errorMessages.Add("invalid_id");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            isBadRequest = true;
            errorMessages.Add("missing_name");
        }

        if (request.AzureProviderExtentions == null)
        {
            isBadRequest = true;
            errorMessages.Add("invalid_extentions, must provide 'azureProviderExtentions'");
        }

        return (isBadRequest, errorMessages.ToArray());
    }
}
