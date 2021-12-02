namespace Service.TimeZones;

public record ListTimeZonesRequest;
public record ListTimeZonesResponse
{
    public TimeZoneDto[] TimeZones { get; init; } = Array.Empty<TimeZoneDto>();
}

public record TimeZoneDto(string Id);
