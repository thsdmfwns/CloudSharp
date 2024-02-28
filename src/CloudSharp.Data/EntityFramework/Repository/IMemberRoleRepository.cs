using CloudSharp.Data.EntityFramework.Entities;
using FluentResults;

namespace CloudSharp.Data.EntityFramework.Repository;

public interface IMemberRoleRepository
{
    public ValueTask<Result<MemberRole>> FindById(ulong id);

}