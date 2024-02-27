namespace CloudSharp.Api.Error;

public class ConflictError : HttpStatusCodeError
{
    protected override int HttpStatusCode => 409;
}