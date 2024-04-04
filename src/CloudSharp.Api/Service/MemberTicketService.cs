using CloudSharp.Api.Error;
using CloudSharp.Api.Util;
using CloudSharp.Data.Store;
using CloudSharp.Data.Ticket;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;
using FluentResults;

namespace CloudSharp.Api.Service;

public class MemberTicketService(ITicketStore _ticketStore, IFileStore _fileStore, Logger<IMemberTicketService> _logger) : IMemberTicketService
{
    public async ValueTask<Result<Guid>> AddFileStreamTicket(Guid memberId, string targetPath)
    {
        var targetFindResult = _fileStore.GetFileInfo(DirectoryType.Member, memberId, targetPath);
        if (targetFindResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy(targetFindResult.Errors));
        }
        if (!targetFindResult.Value.Exists)
        {
            return Result.Fail(new NotFoundError().CausedBy("file not found"));
        }

        var ticket = new FileStreamTicket
        {
            TargetFilePath = targetPath,
            TargetFileDirectoryId = memberId,
            DirectoryType = DirectoryType.Member,
            TicketOwnerId = memberId,
        };
        var addTicketResult = await _ticketStore.AddTicket(ticket);
        if (addTicketResult.IsFailed)
        {
            return Result.Fail(new ConflictError().CausedBy("token exist"));
        }

        return addTicketResult.Value;
    }

    public async Task<Result<Guid>> AddFileUploadTicket(Guid memberId, string? targetFolderPath, string filename)
    {
        targetFolderPath ??= ".";
        var targetFindResult = _fileStore.GetFileInfo(DirectoryType.Member, memberId, Path.Combine(targetFolderPath, filename));
        if (targetFindResult.IsFailed)
        {
            return Result.Fail(new BadRequestError().CausedBy(targetFindResult.Errors));
        }

        if (!targetFindResult.Value.DirectoryExist())
        {
            return Result.Fail(new NotFoundError().CausedBy("folder not found"));
        }
        if (targetFindResult.Value.Exists)
        {
            return Result.Fail(new ConflictError().CausedBy("file exist"));
        }
        
        var ticket = new FileUploadTicket
        {
            TargetFileDirectoryId = memberId,
            DirectoryType = DirectoryType.Member,
            TicketOwnerId = memberId,
            TargetFileName = filename,
            TargetFolderPath = targetFolderPath,
        };
        var addTicketResult = await _ticketStore.AddTicket(ticket);
        if (addTicketResult.IsFailed)
        {
            return Result.Fail(new ConflictError().CausedBy("token exist"));
        }

        return addTicketResult.Value;
    }

    public async ValueTask<Result<T>> GetTicket<T>(Guid ticketToken) where T : ITicket<T>
    {
        var result = await _ticketStore.GetTicket<T>(ticketToken);
        if (result.IsFailed)
        {
            return new NotFoundError().CausedBy(result.Errors);
        }
        return result.Value;
    }

    public async ValueTask<Result> DeleteTicket<T>(Guid ticketToken) where T : ITicket<T>
    {
        var findTicketResult = await _ticketStore.GetTicket<T>(ticketToken);
        if (findTicketResult.IsFailed)
        {
            return new NotFoundError().CausedBy(findTicketResult.Errors);
        }
        
        return await _ticketStore.RemoveTicket(findTicketResult.Value);
    }
}