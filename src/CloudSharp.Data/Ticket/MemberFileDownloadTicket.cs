using System.Text.Json;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Share.Enum;

namespace CloudSharp.Data.Ticket;

public class MemberFileDownloadTicket : ITicket<MemberFileDownloadTicket>
{
    public required string TargetFilePath { get; init; }
    public required FileDownloadType FileDownloadType { get; init; }
    public required DateTimeOffset? ExpireTime { get; init; } = DateTime.Now.AddMinutes(3);
    public Guid Token { get; init; } = Guid.NewGuid();
    public Member? TicketOwner { get; init; }
    public static MemberFileDownloadTicket? FromJson(string? json)
    {
        return json is null
            ? null
            : JsonSerializer.Deserialize<MemberFileDownloadTicket>(json);
    }
    public static string RedisKey => "member_dl";
}