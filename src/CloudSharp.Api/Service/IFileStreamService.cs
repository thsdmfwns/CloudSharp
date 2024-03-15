using CloudSharp.Data.Ticket;
using FluentResults;

namespace CloudSharp.Api.Service;

public interface IFileStreamService
{
    Result<FileStream> GetFileStream(FileStreamTicket fileStreamTicket);
}