using System.ComponentModel.DataAnnotations;
using CloudSharp.Api.Entities;
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
                var err = findResult.Errors.First();
                return Result.Fail(new UnauthorizedError(err.Message));
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                var err = passwordVerifyResult.Errors.First();
                return Result.Fail(new UnauthorizedError(err.Message));
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
                var err = findResult.Errors.First();
                return Result.Fail(new UnauthorizedError(err.Message));
            }
            var expectedPasswordHash = findResult.Value.Password;
            var passwordVerifyResult = PasswordHasher.VerifyHashedPassword(expectedPasswordHash, password);
            if (passwordVerifyResult.IsFailed)
            {
                var err = passwordVerifyResult.Errors.First();
                return Result.Fail(new UnauthorizedError(err.Message));
            }
        
            return findResult.Value.ToMemberDto();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result<MemberDto>> Register(string id, string password, ulong role, string email, string nickname, Guid? profileUrl)
    {
        try
        {
            var findMemberResult = await memberRepository.FindByLoginId(id);
            if (findMemberResult.IsSuccess)
            {
                return Result.Fail(new ConflictError("id exist"));
            }

            var findRoleResult = await memberRoleRepository.FindById(role);
            if (findRoleResult.IsFailed)
            {
                var err = findRoleResult.Errors.First();
                return Result.Fail(new NotFoundError(err.Message));
            }

            var memberId = Guid.NewGuid();
            var hashedPassword = PasswordHasher.HashPassword(password);
            var insertResult = await memberRepository.InsertMember(memberId, id, hashedPassword, findRoleResult.Value, email, nickname, profileUrl);
            if (insertResult.IsFailed)
            {
                var err = findRoleResult.Errors.First();
                return Result.Fail(err.Message);
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

    public async ValueTask<Result> UpdateRole(Guid id, MemberRole role)
    {
        try
        {
            var updateResult = await memberRepository.UpdateRole(id, role);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError(updateResult.Errors.First().Message));
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
            var updateResult = await memberRepository.UpdateEmail(id, email);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError(updateResult.Errors.First().Message));
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
            var updateResult = await memberRepository.UpdateNickname(id, nickname);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError(updateResult.Errors.First().Message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));
        }
    }

    public async ValueTask<Result> UpdatePassword(Guid id, string updatePassword)
    {
        try
        {
            var updateResult = await memberRepository.UpdatePassword(id, updatePassword);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError(updateResult.Errors.First().Message));
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
            //todo check file exist
            var updateResult = await memberRepository.UpdateProfileId(id, profileImageId);
            return Result.OkIf(updateResult.IsSuccess, new NotFoundError(updateResult.Errors.First().Message));
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
                var err = findResult.Errors.First();
                return Result.Fail(new NotFoundError(err.Message));
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
                var err = findResult.Errors.First();
                return Result.Fail(new NotFoundError(err.Message));
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


    
    public async ValueTask<Result> DeleteMember(Guid id)
    {
        try
        {
            var deleteMemberResult = await memberRepository.DeleteMember(id);
            return Result.OkIf(deleteMemberResult.IsSuccess, new NotFoundError(deleteMemberResult.Errors.First().Message));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed by Exception");
            return Result.Fail(new InternalServerError("failed by exception").CausedBy(e));   
        }
    }
}