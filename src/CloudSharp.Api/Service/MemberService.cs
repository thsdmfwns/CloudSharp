using System.ComponentModel.DataAnnotations;
using CloudSharp.Api.Error;
using CloudSharp.Api.Repository;
using CloudSharp.Api.Util;
using CloudSharp.Share.DTO;
using FluentResults;
using Exception = System.Exception;

namespace CloudSharp.Api.Service;

public class MemberService(IMemberRepository memberRepository, IMemberRoleRepository memberRoleRepository, ILogger<MemberService> _logger) : IMemberService
{
    #region Login & Register
    public async ValueTask<Result<MemberDto>> Login(string id, string password)
    {
        try
        {
            var findResult = await memberRepository.FindByLoginId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError("member not found"));
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError("bad password"));
            }
        
            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }
    
    private async ValueTask<Result<MemberDto>> LoginByMemberId(Guid id, string password)
    {
        try
        {
            var findResult = await memberRepository.FindByMemberId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError("member not found"));
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError("bad password"));
            }
        
            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result<MemberDto>> Register(string id, string password, ulong role, string email, string nickname, string? profileUrl)
    {
        try
        {
            //parameter check
            var member = await memberRepository.FindByLoginId(id);
            if (member.IsSuccess)
            {
                return Result.Fail(new ConflictError("id exist"));
            }

            var memberId = Guid.NewGuid();
            var hashedPassword = PasswordHasher.HashPassword(password);
            var insertResult = await memberRepository.InsertMember(memberId, id, hashedPassword, role, email, nickname, profileUrl);
            if (insertResult.IsFailed)
            {
                return Result.Fail(insertResult.Errors);
            }
            
            return insertResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }
    #endregion

    #region Update

    public async ValueTask<Result> UpdateRole(Guid id, ulong roleId)
    {
        try
        {
            var memberFindResult = await memberRepository.FindByMemberId(id);
            if (memberFindResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("member not found"));
            }

            var roleFindResult = await memberRoleRepository.FindById(roleId);
            if (roleFindResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("role not found"));
            }

            return await memberRepository.UpdateRole(id, roleId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdateEmail(Guid id, string email)
    {
        try
        {
            var memberFindResult = await memberRepository.FindByMemberId(id);
            if (memberFindResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("member not found"));
            }

            return await memberRepository.UpdateEmail(id, email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
        
    }

    public async ValueTask<Result> UpdateNickname(Guid id, string nickname)
    {
        try
        {
            var memberFindResult = await memberRepository.FindByMemberId(id);
            if (memberFindResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("member not found"));
            }

            return await memberRepository.UpdateNickname(id, nickname);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdatePassword(Guid id, string password, string updatePassword)
    {
        try
        {
            //todo login in controller?
            var loginResult = await LoginByMemberId(id, password);
            if (loginResult.IsFailed)
            {
                return Result.Fail(loginResult.Errors);
            }

            return await memberRepository.UpdatePassword(id, updatePassword);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdateProfileUrl(Guid id, Guid profileImageId)
    {
        try
        {
            var memberFindResult = await memberRepository.FindByMemberId(id);
            if (memberFindResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("member not found"));
            }
            
            return await memberRepository.UpdateProfileId(id, profileImageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    #endregion
    
    #region Find
    public async ValueTask<Result<MemberDto>> FindByMemberId(Guid id)
    {
        try
        {
            var findResult = await memberRepository.FindByMemberId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("member not found"));
            }

            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));   
        }
    }

    public async ValueTask<Result<MemberDto>> FindByLoginId(string id)
    {
        try
        {
            var findResult = await memberRepository.FindByLoginId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new NotFoundError("member not found"));
            }

            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));   
        }
    }

    

    #endregion


    
    public async ValueTask<Result> DeleteMember(Guid id, string password)
    {
        try
        {
            //todo login in controller?
            var loginResult = await LoginByMemberId(id, password);
            if (loginResult.IsFailed)
            {
                return Result.Fail(loginResult.Errors);
            }

            return await memberRepository.DeleteMember(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));   
        }
    }
}