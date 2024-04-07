using CloudSharp.Data.Ticket;
using CloudSharp.Share.DTO;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IMemberTicketService
{
    ValueTask<Result<Guid>> AddFileStreamTicket(Guid memberId, string targetPath);
    Task<Result<Guid>> AddFileUploadTicket(Guid memberId, string? targetFolderPath, string filename);
    ValueTask<Result<T>> GetTicket<T>(Guid ticketToken) where T : ITicket<T>;
    ValueTask<Result> DeleteTicket<T>(Guid ticketToken) where T : ITicket<T>;
}