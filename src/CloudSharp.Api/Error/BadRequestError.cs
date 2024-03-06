namespace CloudSharp.Api.Error;

public class BadRequestError : HttpStatusCodeError
{
    protected override int HttpStatusCode => 400;
}