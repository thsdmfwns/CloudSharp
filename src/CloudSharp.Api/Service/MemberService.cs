using System.ComponentModel.DataAnnotations;
using CloudSharp.Api.Repository;
using CloudSharp.Api.Util;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public class MemberService(IMemberRepository memberRepository, ILogger<MemberService> _logger) : IMemberService
{
    public async ValueTask<Result<MemberDto>> Login(string id, string password)
    {
        try
        {
            var findResult = await memberRepository.FindById(id);
            if (findResult.IsFailed)
            {
                return Result.Fail("member is null");
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                return Result.Fail("bad password");
            }
        
            return (MemberDto)findResult.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Login failed by Exception");
            return Result.Fail(new Error("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result<MemberDto>> Register(string id, string password, ulong role, string email, string nickname, string? profileUrl)
    {
        try
        {
            //parameter check
            var member = await memberRepository.FindById(id);
            if (member.IsSuccess)
            {
                return Result.Fail("id exist");
            }

            if (!new EmailAddressAttribute().IsValid(email))
            {
                return Result.Fail("bad email");
            }

            var hashedPassword = PasswordHasher.HashPassword(password);
            var insertResult = await memberRepository.InsertMember(id, hashedPassword, role, email, nickname, profileUrl);
            if (insertResult.IsFailed)
            {
                return Result.Fail(insertResult.Errors);
            }
            
            var findResult = await memberRepository.FindByIdx(insertResult.Value);
            if (findResult.IsFailed)
            {
                return Result.Fail(findResult.Errors);
            }
            
            return (MemberDto)findResult.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new Error("failed by exception").CausedBy(e));
        }
    }

    public ValueTask<Result<MemberDto>> UpdateRole(ulong idx, ulong role)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<MemberDto>> UpdateEmail(ulong idx, string email)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<MemberDto>> UpdateNickname(ulong idx, string nickname)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<MemberDto>> UpdatePassword(ulong idx, string password)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result<MemberDto>> UpdateProfileUrl(ulong idx, string profileUrl)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Result<MemberDto>> FindByIdx(ulong idx)
    {
        try
        {
            var findResult = await memberRepository.FindByIdx(idx);
            if (findResult.IsFailed)
            {
                return Result.Fail("member is null");
            }

            return (MemberDto)findResult.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new Error("failed by exception").CausedBy(e));   
        }
    }

    public async ValueTask<Result<MemberDto>> FindById(string id)
    {
        try
        {
            var findResult = await memberRepository.FindById(id);
            if (findResult.IsFailed)
            {
                return Result.Fail("member is null");
            }

            return (MemberDto)findResult.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new Error("failed by exception").CausedBy(e));   
        }
    }

    public ValueTask<Result<MemberDto>> FindByDirectory(Guid directory)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Result> DeleteMember(ulong idx, string password)
    {
        throw new NotImplementedException();
    }
}