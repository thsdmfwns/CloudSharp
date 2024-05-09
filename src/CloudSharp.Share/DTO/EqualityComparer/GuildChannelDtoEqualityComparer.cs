using System.Text.Json;

namespace CloudSharp.Share.DTO.EqualityComparer;

public class GuildChannelDtoEqualityComparer : IEqualityComparer<GuildChannelDto>
{
    public bool Equals(GuildChannelDto? x, GuildChannelDto? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        var result = x.GuildId == y.GuildId 
                     && x.ChannelId.Equals(y.ChannelId) 
                     && x.ChannelName == y.ChannelName 
                     && x.CreatedOn.Equals(y.CreatedOn) 
                     && Nullable.Equals(x.UpdatedOn, y.UpdatedOn) 
                     && x.ChannelRoles.OrderBy(x => x.GuildChannelRoleId)
                         .SequenceEqual(y.ChannelRoles.OrderBy(y => y.GuildChannelRoleId));
        if (result) return result;
        
        
        Console.WriteLine("GuildChannelDto is diff");
        Console.WriteLine($"x : {JsonSerializer.Serialize(x)}");
        Console.WriteLine($"y : {JsonSerializer.Serialize(y)}");
        return result;
    }

    public int GetHashCode(GuildChannelDto obj)
    {
        return HashCode.Combine(obj.GuildId, obj.ChannelId, obj.ChannelName, obj.CreatedOn, obj.UpdatedOn, obj.ChannelRoles);
    }
}