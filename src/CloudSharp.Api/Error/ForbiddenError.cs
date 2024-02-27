namespace CloudSharp.Api.Error;

public class ForbiddenError : HttpStatusCodeError
{
    protected override int HttpStatusCode => 403;
}