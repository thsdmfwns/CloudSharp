using System.Text.Json;

namespace CloudSharp.Share.DTO.EqualityComparer;

public class GuildMemberDtoEqualityCompare  : IEqualityComparer<GuildMemberDto>
{
    public bool Equals(GuildMemberDto? x, GuildMemberDto? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        var result = x.GuildId == y.GuildId 
                     && x.GuildMemberId == y.GuildMemberId 
                     && x.MemberId.Equals(y.MemberId) 
                     && x.IsBanned == y.IsBanned 
                     && x.CreatedOn.Equals(y.CreatedOn) 
                     && Nullable.Equals(x.UpdatedOn, y.UpdatedOn) 
                     && x.HadRoles.OrderBy(x => x.GuildMemberRoleId)
                         .SequenceEqual(y.HadRoles.OrderBy(y => y.GuildMemberRoleId));
        if (result) return result;
        
        Console.WriteLine("GuildMember is diff");
        Console.WriteLine($"x : {JsonSerializer.Serialize(x)}");
        Console.WriteLine($"y : {JsonSerializer.Serialize(y)}");
        return result;
    }

    public int GetHashCode(GuildMemberDto obj)
    {
        return HashCode.Combine(obj.GuildId, obj.GuildMemberId, obj.MemberId, obj.IsBanned, obj.CreatedOn, obj.UpdatedOn, obj.HadRoles);
    }
}