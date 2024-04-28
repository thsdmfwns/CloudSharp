using CloudSharp.Share.DTO;

namespace CloudSharp.Data.Ticket;

public interface ITicket<out T> where T : ITicket<T>
{
    public DateTimeOffset? ExpireTime { get; init; }
    public Guid Token { get; init; } 
    public Guid? TicketOwnerId { get; init; }
    public static abstract T? FromJson(string? json);
    public static abstract string RedisKey { get; }
}