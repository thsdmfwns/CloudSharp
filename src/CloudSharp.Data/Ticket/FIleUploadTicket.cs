using System.Text.Json;
using CloudSharp.Data.EntityFramework.Entities;
using CloudSharp.Share.Enum;

namespace CloudSharp.Data.Ticket;

public class FileUploadTicket : ITicket<FileUploadTicket>
{
    public required string TargetFileName { get; init; }
    public required string? TargetFolderPath { get; init; }
    public required Guid TargetFileDirectoryId { get; init; }
    public required DirectoryType DirectoryType { get; init; }
    public DateTimeOffset? ExpireTime { get; init; }
    public Guid Token { get; init; }
    public Member? TicketOwner { get; init; }
    public static FileUploadTicket? FromJson(string? json)
    {
        return json is null
            ? null
            : JsonSerializer.Deserialize<FileUploadTicket>(json);
    }

    public static string RedisKey => "file_upload";
}