namespace CloudSharp.Api.Error;

public class NotFoundError(string message) : HttpStatusCodeError(message)
{
    protected override int HttpStatusCode => 404;
}