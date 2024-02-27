namespace CloudSharp.Api.Error;

public class InternalServerError : HttpStatusCodeError
{
    protected override int HttpStatusCode => 500;
}