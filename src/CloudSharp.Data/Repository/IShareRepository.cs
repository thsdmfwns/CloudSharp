using FluentResults;

namespace CloudSharp.Data.Repository;

public interface IShareRepository
{
    public ValueTask<Entities.Share?> GetShareById(Guid shareId);
    public ValueTask<List<Entities.Share>> GetSharesByMemberId(Guid memberId);
    public ValueTask<List<Entities.Share>> GetSharesByFolderPath(Guid memberId, string folderPath);

    public ValueTask<Result<Guid>> InsertShare(Entities.Share share);

    public ValueTask<Result> UpdateShare(Guid shareId, Action<Entities.Share> updateAction);

    public ValueTask<Result> DeleteShare(Guid shareId);
    public ValueTask<Result> DeleteShareByPath(Guid memberId, string path);
    public ValueTask<Result> DeleteShareByStartWithPath(Guid memberId, string path);
}