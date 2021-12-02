using TimeZoneConverter;

namespace Service.TimeZones;

internal static class TimeZoneExtentions
{
    private const string DefaultTimeZoneId = "W. Europe Standard Time";

    internal static TimeZoneDto[] TimeZones => TZConvert.KnownWindowsTimeZoneIds
        .Select(tz => new TimeZoneDto(tz))
        .ToArray();

    internal static string GetTimeZoneIdOrDefault(string? timeZoneId) =>
        !string.IsNullOrEmpty(timeZoneId) ? timeZoneId : DefaultTimeZoneId;

    internal static TimeZoneInfo GetTimeZoneInfoOrDefault(string timeZoneId) =>
        TZConvert.GetTimeZoneInfo(!string.IsNullOrEmpty(timeZoneId) ? timeZoneId : DefaultTimeZoneId);

    internal static bool IsValidTimeZone(string timeZoneId) => 
        TZConvert.TryGetTimeZoneInfo(timeZoneId, out var _);
}
