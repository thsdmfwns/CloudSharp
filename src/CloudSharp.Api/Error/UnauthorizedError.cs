namespace CloudSharp.Api.Error;

public class UnauthorizedError(string message) : HttpStatusCodeError(message)
{
    protected override int HttpStatusCode => 401;
}