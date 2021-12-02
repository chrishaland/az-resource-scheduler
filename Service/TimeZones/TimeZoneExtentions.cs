using TimeZoneConverter;

namespace Service.TimeZones;

public static class TimeZoneExtentions
{
    public const string DefaultTimeZoneId = "W. Europe Standard Time";

    public static TimeZoneDto[] TimeZones => TZConvert.KnownWindowsTimeZoneIds
        .Select(tz => new TimeZoneDto(tz))
        .ToArray();

    public static string GetTimeZoneIdOrDefault(string? timeZoneId) =>
        !string.IsNullOrEmpty(timeZoneId) ? timeZoneId : DefaultTimeZoneId;

    public static TimeZoneInfo GetTimeZoneInfoOrDefault(string timeZoneId) =>
        TZConvert.GetTimeZoneInfo(!string.IsNullOrEmpty(timeZoneId) ? timeZoneId : DefaultTimeZoneId);

    public static bool IsValidTimeZone(string timeZoneId) => 
        TZConvert.TryGetTimeZoneInfo(timeZoneId, out var _);
}
