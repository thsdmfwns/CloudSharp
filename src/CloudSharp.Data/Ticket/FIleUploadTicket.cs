using System.Text.Json;
using CloudSharp.Share.DTO;
using CloudSharp.Share.Enum;

namespace CloudSharp.Data.Ticket;

public record FileUploadTicket : ITicket<FileUploadTicket>
{
    public required string TargetFileName { get; init; }
    public required string? TargetFolderPath { get; init; }
    public required Guid TargetFileDirectoryId { get; init; }
    public required DirectoryType DirectoryType { get; init; }
    public DateTimeOffset? ExpireTime { get; init; } = DateTimeOffset.Now.AddMinutes(3);
    public Guid Token { get; init; } = Guid.NewGuid();
    public required Guid? TicketOwnerId { get; init; }
    public static FileUploadTicket? FromJson(string? json)
    {
        return json is null
            ? null
            : JsonSerializer.Deserialize<FileUploadTicket>(json);
    }

    public static string RedisKey => "file_upload";
}