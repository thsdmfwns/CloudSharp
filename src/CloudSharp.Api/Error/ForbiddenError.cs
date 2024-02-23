namespace CloudSharp.Api.Error;

public class ForbiddenError(string message) : HttpStatusCodeError(message)
{
    protected override int HttpStatusCode => 403;
}