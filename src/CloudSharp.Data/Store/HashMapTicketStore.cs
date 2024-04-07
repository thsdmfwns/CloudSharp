using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using CloudSharp.Data.Ticket;
using FluentResults;

namespace CloudSharp.Data.Store;

public class HashMapTicketStore : ITicketStore
{
    private readonly Object _ticketStoreLock = new();
    private ConcurrentDictionary<string, ConcurrentDictionary<Guid, object>> _ticketStored = new();

    private object? Get(string key, Guid token)
    {
        lock (_ticketStoreLock)
        {
            if (!_ticketStored.TryGetValue(key, out var tickets) 
                || !tickets.TryGetValue(token, out var ticket))
            {
                return null;
            }
            return ticket;
        }
    }

    private bool TryAdd(string key, Guid token, object value)
    {
        lock (_ticketStoreLock)
        {
            var tickets = _ticketStored.GetOrAdd(key, x => new ConcurrentDictionary<Guid, object>());
            return tickets.TryAdd(token, value);
        }
    }

    private bool Remove(string key, Guid token)
    {
        lock (_ticketStoreLock)
        {
            if (!_ticketStored.TryGetValue(key, out var tickets) 
                || !tickets.TryGetValue(token, out _))
            {
                return true;
            }

            return tickets.Remove(token, out _);
        }
    }
    
    
    public ValueTask<Result<T>> GetTicket<T>(Guid ticketToken) where T : ITicket<T>
    {
        var result = Get(T.RedisKey, ticketToken);
        if (result is not T ticket)
        {
            return ValueTask.FromResult<Result<T>>(Result.Fail("ticket not found"));
        }

        return ValueTask.FromResult<Result<T>>(ticket);
    }

    public ValueTask<Result<Guid>> AddTicket<T>(ITicket<T> ticket) where T : ITicket<T>
    {
        var result = TryAdd(T.RedisKey, ticket.Token, ticket);
        if (!result)
        {
            return ValueTask.FromResult<Result<Guid>>(Result.Fail("exist token"));
        }
        return ValueTask.FromResult<Result<Guid>>(ticket.Token);
    }

    public ValueTask<Result> RemoveTicket<T>(ITicket<T> ticket) where T : ITicket<T>
    {
        var result = Remove(T.RedisKey, ticket.Token);
        return ValueTask.FromResult(Result.OkIf(result, "ticket not found"));
    }

    public ValueTask<bool> ExistTicket<T>(Guid ticketToken) where T : ITicket<T>
    {
        var result = Get(T.RedisKey, ticketToken);
        return ValueTask.FromResult(result is T);
    }
}