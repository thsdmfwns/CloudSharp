using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IMemberTicketService
{
    ValueTask<Result<Guid>> AddFileStreamTicket(MemberDto memberDto, string targetPath);
    Task<Result<Guid>> AddFileUploadTicket(MemberDto memberDto, string? targetFolderPath, string filename);
}