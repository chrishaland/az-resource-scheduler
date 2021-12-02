namespace Service.TimeZones;

[Route("api/timezones/get")]
public class GetTimeZonesHandler : QueryHandlerBase<ListTimeZonesRequest, ListTimeZonesResponse>
{
    public override async Task<ActionResult<ListTimeZonesResponse>> Execute([FromBody] ListTimeZonesRequest request, CancellationToken ct)
    {
        await Task.CompletedTask;
        return Ok(new ListTimeZonesResponse
        {
            TimeZones = TimeZoneExtentions.TimeZones
        });
    }

}
