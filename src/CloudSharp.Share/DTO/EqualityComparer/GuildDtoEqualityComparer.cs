namespace CloudSharp.Share.DTO.EqualityComparer;

public class GuildDtoEqualityComparer : IEqualityComparer<GuildDto>
{
    public bool Equals(GuildDto? x, GuildDto? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        var memberEquals = x.Members.OrderBy(x => x.GuildMemberId)
            .SequenceEqual(y.Members.OrderBy(y => y.GuildMemberId),
                new GuildMemberDtoEqualityCompare());
        var channelEquals = x.Channels.OrderBy(x => x.ChannelId)
            .SequenceEqual(y.Channels.OrderBy(y => y.ChannelId),
                new GuildChannelDtoEqualityComparer());
        var roleEquals = x.Roles.OrderBy(x => x.GuildRoleId)
            .SequenceEqual(y.Roles.OrderBy(y => y.GuildRoleId));
        return x.GuildId == y.GuildId 
               && x.GuildName == y.GuildName 
               && Nullable.Equals(x.GuildProfileId, y.GuildProfileId) 
               && x.CreatedOn.Equals(y.CreatedOn) 
               && Nullable.Equals(x.UpdatedOn, y.UpdatedOn) 
               && memberEquals
               && channelEquals
               && roleEquals;
    }

    public int GetHashCode(GuildDto obj)
    {
        return HashCode.Combine(obj.GuildId, obj.GuildName, obj.GuildProfileId, obj.CreatedOn, obj.UpdatedOn,
            obj.Members, obj.Channels, obj.Roles);
    }
}