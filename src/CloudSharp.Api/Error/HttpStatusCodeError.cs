using Microsoft.AspNetCore.Mvc;

namespace CloudSharp.Api.Error;

public abstract class HttpStatusCodeError : FluentResults.Error
{
    protected abstract int HttpStatusCode { get; }

    public IActionResult ToActionResult()
     => new StatusCodeResult(HttpStatusCode);
}