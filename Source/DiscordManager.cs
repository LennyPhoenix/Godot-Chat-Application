using Godot;
using System;
using Discord;

public class DiscordManager : Node2D
{
    public Discord.Discord discord = new Discord.Discord(763426273757495327, (ulong)Discord.CreateFlags.Default);
}
