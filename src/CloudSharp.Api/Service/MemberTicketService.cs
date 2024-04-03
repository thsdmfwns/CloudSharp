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
    public async ValueTask<Result<Guid>> AddFileStreamTicket(MemberDto memberDto, string targetPath)
    {
        var directoryId = Guid.Parse(memberDto.MemberId);
        var targetFindResult = _fileStore.GetFileInfo(DirectoryType.Member, directoryId, targetPath);
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
            TargetFileDirectoryId = directoryId,
            DirectoryType = DirectoryType.Member,
            TicketOwner = memberDto,
        };
        var addTicketResult = await _ticketStore.AddTicket(ticket);
        if (addTicketResult.IsFailed)
        {
            return Result.Fail(new ConflictError().CausedBy("token exist"));
        }

        return addTicketResult.Value;
    }

    public async Task<Result<Guid>> AddFileUploadTicket(MemberDto memberDto, string? targetFolderPath, string filename)
    {
        targetFolderPath ??= string.Empty;
        var directoryId = Guid.Parse(memberDto.MemberId);
        var targetFindResult = _fileStore.GetFileInfo(DirectoryType.Member, directoryId, Path.Combine(targetFolderPath, filename));
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
            TargetFileDirectoryId = directoryId,
            DirectoryType = DirectoryType.Member,
            TicketOwner = memberDto,
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
}