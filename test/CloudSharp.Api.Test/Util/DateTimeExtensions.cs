using Bogus;

namespace CloudSharp.Api.Test.Util;

public enum DateTimeCase
{
    Now,
    Future,
    Past
}

public static class DateTimeExtensions
{
    public static DateTime RemoveNanoSec(this DateTime dateTime)
        => dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerMicrosecond));

    public static DateTimeOffset RemoveNanoSec(this DateTimeOffset dateTime)
        => dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerMicrosecond));

    public static DateTime DateTimeCaseToDateTime(this DateTimeCase dateTimeCase)
    {
        return dateTimeCase switch
        {
            DateTimeCase.Now => DateTime.Now,
            DateTimeCase.Future => new Faker().Date.Future(),
            DateTimeCase.Past => new Faker().Date.Past(),
            _ => DateTime.Now
        };
    }
}