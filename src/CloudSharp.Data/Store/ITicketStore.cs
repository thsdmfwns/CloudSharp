using CloudSharp.Data.Ticket;
using FluentResults;

namespace CloudSharp.Data.Store;

public interface ITicketStore
{
    public ValueTask<Result<T>> GetTicket<T>(Guid ticketToken) where T : ITicket<T>;
    public ValueTask<Result<Guid>> AddTicket<T>(ITicket<T> ticket) where T : ITicket<T>;
    public ValueTask<Result> RemoveTicket<T>(ITicket<T> ticket) where T : ITicket<T>;
    public ValueTask<bool> ExistTicket<T>(Guid ticketToken) where T : ITicket<T>;
}