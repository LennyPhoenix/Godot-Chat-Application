using Godot;
using System;
using Discord;

public class DiscordManager : Node
{
    public Discord.Discord discord;
    public ActivityManager activityManager;

    long startTime;

    public override void _Ready()
    {
        discord = new Discord.Discord(763426273757495327, (ulong)Discord.CreateFlags.Default);
        activityManager = discord.GetActivityManager();
    }

    public void UpdateActivity(string state, bool resetTime)
    {
        if (resetTime)
        {
            startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        Activity activity = new Activity
        {
            State = state,
            Timestamps =
            {
                Start=startTime
            },
            Assets =
            {
                LargeImage="logo",
                LargeText="Godot Chat App"
            }
        };

        activityManager.UpdateActivity(activity, (result) =>
        {
            if (result == Result.Ok)
            {
                GD.Print("Successfully updated activity.");
            }
            else
            {
                GD.PrintErr("Failed to update activity: ", result);
            }
        });
    }

    public override void _Process(float delta)
    {
        discord.RunCallbacks();
    }
}
