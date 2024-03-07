namespace CloudSharp.Api.Test.Util;

public static class GuidExtension
{
    public static Guid ToGuid(this string guidString)
    {
        return string.IsNullOrEmpty(guidString) ? Guid.Empty : Guid.Parse(guidString);
    }
}