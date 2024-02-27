namespace CloudSharp.Api.Error;

public class NotFoundError : HttpStatusCodeError
{
    protected override int HttpStatusCode => 404;
}