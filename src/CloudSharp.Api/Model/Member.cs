using CloudSharp.Share.DTO;

namespace CloudSharp.Api.Model;

public class Member
{
    public required ulong Idx { get; init; }
    public required string Id { get; init; }
    public required string Password { get; init; }
    public required ulong Role { get; init; }
    public required string Email { get; init; }
    public required string Nickname { get; init; }
    public required string Directory { get; init; }
    public string? ProfileImageURL { get; init; }


    public static explicit operator MemberDto(Member member)
    {
        return new MemberDto
        {
            Directory = member.Directory,
            Email = member.Email,
            Id = member.Id,
            Idx = member.Idx,
            Nickname = member.Nickname,
            ProfileImageURL = member.ProfileImageURL,
            Role = member.Role
        };
    }
};