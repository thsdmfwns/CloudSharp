namespace CloudSharp.Api.Error;

public class InternalServerError(string message) : HttpStatusCodeError(message)
{
    protected override int HttpStatusCode => 500;
}