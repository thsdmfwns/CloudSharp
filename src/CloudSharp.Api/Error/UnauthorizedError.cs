namespace CloudSharp.Api.Error;

public class UnauthorizedError : HttpStatusCodeError
{
    protected override int HttpStatusCode => 401;
}