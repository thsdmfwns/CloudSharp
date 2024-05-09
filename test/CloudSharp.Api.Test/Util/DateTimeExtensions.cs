namespace CloudSharp.Api.Test.Util;

public static class DateTimeExtensions
{
    public static DateTime RemoveNanoSec(this DateTime dateTime)
        => dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerMicrosecond));
}