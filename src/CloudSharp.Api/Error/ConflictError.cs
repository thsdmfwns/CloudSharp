namespace CloudSharp.Api.Error;

public class ConflictError(string message) : HttpStatusCodeError(message)
{
    protected override int HttpStatusCode => 409;
}