﻿using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonMessageReactionAddEventArgs
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonPropertyName("message_author_id")]
    public ulong? MessageAuthorId { get; set; }

    [JsonPropertyName("burst")]
    public bool Burst { get; set; }

    [JsonPropertyName("burst_colors")]
    public Color[] BurstColors { get; set; }

    [JsonPropertyName("type")]
    public ReactionType Type { get; set; }
}
