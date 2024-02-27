using System.ComponentModel.DataAnnotations;
using CloudSharp.Api.Entities;
using CloudSharp.Api.Error;
using CloudSharp.Api.Repository;
using CloudSharp.Api.Util;
using CloudSharp.Share.DTO;
using FluentResults;
using Exception = System.Exception;

namespace CloudSharp.Api.Service;

public class MemberService(IMemberRepository memberRepository, ILogger<MemberService> _logger) : IMemberService
{
    #region Login & Register
    public async ValueTask<Result<MemberDto>> Login(string id, string password)
    {
        try
        {
            var findResult = await memberRepository.FindByLoginId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError().CausedBy(findResult.Errors));
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError().CausedBy(passwordVerifyResult.Errors));
            }
        
            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    private async ValueTask<Result<MemberDto>> LoginByMemberId(Guid id, string password)
    {
        try
        {
            var findResult = await memberRepository.FindByMemberId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError().CausedBy(findResult.Errors));
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                return Result.Fail(new UnauthorizedError().CausedBy(passwordVerifyResult.Errors));
            }
        
            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public async ValueTask<Result<MemberDto>> Register(string id, string password, MemberRole role, string email, string nickname, Guid? profileUrl)
    {
        try
        {
            var findMemberResult = await memberRepository.FindByLoginId(id);
            if (findMemberResult.IsSuccess)
            {
                return Result.Fail(new ConflictError().CausedBy("id exist"));
            }
            
            var memberId = Guid.NewGuid();
            var hashedPassword = PasswordHasher.HashPassword(password);
            var member = new Member
            {
                MemberId = memberId,
                LoginId = id,
                Password = hashedPassword,
                Role = role,
                Email = email,
                Nickname = nickname,
                ProfileImageId = profileUrl
            };
            
            var insertResult = await memberRepository.InsertMember(member);
            if (insertResult.IsFailed)
            {
                return Result.Fail(new ConflictError().CausedBy(insertResult.Errors));
            }
            
            return insertResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }
    #endregion

    #region Update

    public async ValueTask<Result> UpdateRole(Guid id, MemberRole role)
    {
        try
        {
            var updateResult = await memberRepository.UpdateRole(id, role);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdateEmail(Guid id, string email)
    {
        try
        {
            var updateResult = await memberRepository.UpdateEmail(id, email);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
        
    }

    public async ValueTask<Result> UpdateNickname(Guid id, string nickname)
    {
        try
        {
            var updateResult = await memberRepository.UpdateNickname(id, nickname);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdatePassword(Guid id, string password)
    {
        try
        {
            var passwordHash = PasswordHasher.HashPassword(password);
            var updateResult = await memberRepository.UpdatePassword(id, passwordHash);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdateProfileUrl(Guid id, Guid profileImageId)
    {
        try
        {
            //todo check file exist
            var updateResult = await memberRepository.UpdateProfileId(id, profileImageId);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError().CausedBy(updateResult.Errors));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));
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
                return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
            }

            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));   
        }
    }

    public async ValueTask<Result<MemberDto>> FindByLoginId(string id)
    {
        try
        {
            var findResult = await memberRepository.FindByLoginId(id);
            if (findResult.IsFailed)
            {
                return Result.Fail(new NotFoundError().CausedBy(findResult.Errors));
            }

            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));   
        }
    }

    

    #endregion


    
    public async ValueTask<Result> DeleteMember(Guid id)
    {
        try
        {
            var deleteResult = await memberRepository.DeleteMember(id);
            return Result.OkIf(deleteResult.IsSuccess, new NotFoundError().CausedBy(deleteResult.Errors));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError().CausedBy(e));   
        }
    }
}