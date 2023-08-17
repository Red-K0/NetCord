﻿using NetCord.Services.Commands;

namespace NetCord.Test.Sharded;

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("ping")]
    public Task PingAsync()
    {
        return ReplyAsync($"Pong! {Math.Round(Context.Client.Latency.TotalMilliseconds)} ms");
    }
}
