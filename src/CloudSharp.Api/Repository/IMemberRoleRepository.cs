using CloudSharp.Api.Entities;
using FluentResults;

namespace CloudSharp.Api.Repository;

public interface IMemberRoleRepository
{
    public ValueTask<Result<MemberRole>> FindById(ulong id);

}