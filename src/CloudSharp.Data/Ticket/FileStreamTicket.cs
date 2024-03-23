using System.Text.Json;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;
using Microsoft.VisualBasic;

namespace CloudSharp.Data.Ticket;

public class FileStreamTicket : ITicket<FileStreamTicket>
{
    public required string TargetFilePath { get; init; }
    public required Guid TargetFileDirectoryId { get; init; }
    public required DirectoryType DirectoryType { get; init; }
    public DateTimeOffset? ExpireTime { get; init; } = DateTime.Now.AddMinutes(3);
    public Guid Token { get; init; } = Guid.NewGuid();
    public required MemberDto? TicketOwner { get; init; }
    public static FileStreamTicket? FromJson(string? json)
    {
        return json is null
            ? null
            : JsonSerializer.Deserialize<FileStreamTicket>(json);
    }
    public static string RedisKey => "file_stream";
}