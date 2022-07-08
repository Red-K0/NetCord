﻿namespace NetCord.Services;

public class RequireBotPermissionAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IGuildContext, IChannelContext
{
    public Permission GuildPermission { get; }
    public Permission ChannelPermission { get; }
    public string GuildPermissionFormat { get; }
    public string? ChannelPermissionFormat { get; }

    /// <param name="guildPermission"></param>
    /// <param name="guildPermissionFormat">{0} - missing guild permissions</param>
    public RequireBotPermissionAttribute(Permission guildPermission, string? guildPermissionFormat = null)
    {
        GuildPermission = guildPermission;
        GuildPermissionFormat = guildPermissionFormat ?? "Required bot permissions: {0}";
    }

    /// <param name="guildPermission"></param>
    /// <param name="channelPermission"></param>
    /// <param name="guildPermissionFormat">{0} - missing guild permissions</param>
    /// <param name="channelPermissionFormat">{0} - missing channel permissions</param>
    public RequireBotPermissionAttribute(Permission guildPermission, Permission channelPermission, string? guildPermissionFormat = null, string? channelPermissionFormat = null) : this(guildPermission, guildPermissionFormat)
    {
        ChannelPermission = channelPermission;
        ChannelPermissionFormat = channelPermissionFormat ?? "Required bot channel permissions: {0}";
    }

    public override Task EnsureCanExecuteAsync(TContext context)
    {
        var guild = context.Guild;
        if (guild != null && guild.OwnerId != context.Client.User!.Id)
        {
            var guildUser = guild.Users[context.Client.User.Id];
            Permission permissions = guild.EveryoneRole.Permissions;
            foreach (var role in guildUser.GetRoles(guild))
                permissions |= role.Permissions;
            if (!permissions.HasFlag(Permission.Administrator))
            {
                if (!permissions.HasFlag(GuildPermission))
                {
                    var missingPermissions = GuildPermission & ~permissions;
                    throw new PermissionException(string.Format(GuildPermissionFormat, missingPermissions), missingPermissions, PermissionExceptionEntityType.Bot, PermissionExceptionPermissionType.Guild);
                }
                if (ChannelPermission != default)
                {
                    if (context.Channel == null)
                        throw new EntityNotFoundException("Current channel could not be found");
                    var permissionOverwrites = ((IGuildChannel)context.Channel).PermissionOverwrites;
                    Permission denied = default;
                    Permission allowed = default;
                    foreach (var r in guildUser.RoleIds)
                    {
                        if (permissionOverwrites.TryGetValue(r, out var permissionOverwrite))
                        {
                            denied |= permissionOverwrite.Denied;
                            allowed |= permissionOverwrite.Allowed;
                        }
                    }
                    if (permissionOverwrites.TryGetValue(guildUser.Id, out var o))
                    {
                        denied |= o.Denied;
                        allowed |= o.Allowed;
                    }
                    permissions = (permissions & ~denied) | allowed;
                    if (!permissions.HasFlag(ChannelPermission))
                    {
                        var missingPermissions = ChannelPermission & ~permissions;
                        throw new PermissionException(string.Format(ChannelPermissionFormat!, missingPermissions), missingPermissions, PermissionExceptionEntityType.Bot, PermissionExceptionPermissionType.Channel);
                    }
                }
            }
        }
        return Task.CompletedTask;
    }
}