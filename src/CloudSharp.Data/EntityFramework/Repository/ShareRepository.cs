using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CloudSharp.Data.EntityFramework.Repository;

public class ShareRepository(DatabaseContext context): IShareRepository
{
    private DbSet<Entities.Share> Shares => context.Shares;
    
    public async ValueTask<Entities.Share?> GetShareById(Guid shareId)
    {
        return await Shares.FindAsync(shareId);
    }

    public async ValueTask<List<Entities.Share>> GetSharesByMemberId(Guid memberId)
    {
        return await Shares.Where(x => x.MemberId == memberId).ToListAsync();
        
    }

    public async ValueTask<List<Entities.Share>> GetSharesByFolderPath(Guid memberId, string folderPath)
    {
        return await Shares.Where(x => x.FilePath.StartsWith(folderPath)).ToListAsync();
    }

    public async ValueTask<Result<Guid>> InsertShare(Entities.Share share)
    {
        await Shares.AddAsync(share);
        var result = await context.SaveChangesAsync();
        if (result <= 0)
        {
            return Result.Fail("fail to insert");
        }
        return share.ShareId;
    }

    public async ValueTask<Result> UpdateShare(Guid shareId, Action<Entities.Share> updateAction)
    {
        var share = await Shares.FindAsync(shareId);
        if (share is null)
        {
            return Result.Fail("share not found");
        }
        updateAction.Invoke(share);
        Shares.Update(share);
        return Result.Ok();
    }

    public async ValueTask<Result> DeleteShare(Guid shareId)
    {
        var share = await Shares.FindAsync(shareId);
        if (share is null)
        {
            return Result.Fail("share not found");
        }

        Shares.Remove(share);
        await context.SaveChangesAsync();
        return Result.Ok();
    }

    public async ValueTask<Result> DeleteShareByPath(Guid memberId, string path)
    {
        var result = await Shares
            .Where(x => x.FilePath == path && x.MemberId == memberId)
            .ExecuteDeleteAsync();
        return Result.OkIf(result <= 0, "share not found");
    }

    public async ValueTask<Result> DeleteShareByStartWithPath(Guid memberId, string path)
    {
        var result = await Shares
            .Where(x => x.FilePath.StartsWith(path) && x.MemberId == memberId)
            .ExecuteDeleteAsync();
        return Result.OkIf(result <= 0, "share not found");
    }
}