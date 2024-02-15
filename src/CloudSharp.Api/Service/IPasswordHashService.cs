using FluentResults;

namespace CloudSharp.Api.Service;

public interface IPasswordHashService
{
    public string HashPassword(string password);
    public Result VerifyPassword(string expect, string actual);
}