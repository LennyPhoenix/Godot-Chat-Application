using Godot;
using System;
using Discord;
using System.Text;

public class DiscordManager : Node
{
    [Signal]
    public delegate void JoinedLobby();

    [Signal]
    public delegate void LeftLobby();

    [Signal]
    public delegate void ReceivedMessage(string username, string message);

    public Discord.Discord discord;
    public ActivityManager activityManager;
    public LobbyManager lobbyManager;
    public UserManager userManager;

    long startTime;
    long lobbyId = 0;

    public override void _Ready()
    {
        // System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "1");

        discord = new Discord.Discord(763426273757495327, (ulong)CreateFlags.Default);
        activityManager = discord.GetActivityManager();
        lobbyManager = discord.GetLobbyManager();
        userManager = discord.GetUserManager();

        activityManager.OnActivityJoin += (secret) => {
            lobbyManager.ConnectLobbyWithActivitySecret(secret, (Result result, ref Lobby lobby) =>
            {
                if (result == Result.Ok)
                {
                    GD.Print("Successfully joined a lobby: ", lobby.Id);
                    lobbyId = lobby.Id;
                    UpdateActivity("In lobby...", true, true);
                    EmitSignal("JoinedLobby");
                }
                else
                {
                    GD.PrintErr("Failed to join a lobby: ", result);
                }
            });
        };

        lobbyManager.OnMemberConnect += (lobbyId, userId) =>
        {
            UpdateActivity("In lobby...", false, true);
        };

        lobbyManager.OnMemberDisconnect += (lobbyId, userId) =>
        {
            UpdateActivity("In lobby...", false, true);
        };

        lobbyManager.OnLobbyMessage += (lobbyId, userId, data) =>
        {
            string message = Encoding.UTF8.GetString(data);
            string username = lobbyManager.GetMemberUser(lobbyId, userId).Username;
            EmitSignal("ReceivedMessage", username, message);
        };

        UpdateActivity("In menu...", true, false);
    }

    public void OnChatBoxSendMessage(string message)
    {
        lobbyManager.SendLobbyMessage(lobbyId, Encoding.UTF8.GetBytes(message), (result) =>
        {
            if (result == Discord.Result.Ok)
            {
                GD.Print("Message sent successfully.");
            }
            else
            {
                GD.PrintErr("Failed to send message.");
            }
        });
    }

    public void CreateLobby()
    {
        LobbyTransaction txn = lobbyManager.GetLobbyCreateTransaction();

        lobbyManager.CreateLobby(txn, (Result result, ref Lobby lobby) =>
        {
            if (result == Result.Ok)
            {
                GD.Print("Successfully created a new lobby: ", lobby.Id);
                lobbyId = lobby.Id;
                UpdateActivity("In lobby...", true, true);
                EmitSignal("JoinedLobby");
            }
            else
            {
                GD.PrintErr("Failed to create a new lobby: ", result);
            }
        });
    }

    public void LeaveLobby()
    {
        lobbyManager.DisconnectLobby(lobbyId, (result) =>
        {
            if (result == Discord.Result.Ok)
            {
                GD.Print("Successfully left the lobby.");
                UpdateActivity("In menu...", true, false);
                EmitSignal("LeftLobby");
            }
            else
            {
                GD.PrintErr("Failed to leave the lobby: ", result);
            }
        });
    }

    public void UpdateActivity(string state, bool resetTime, bool inLobby)
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
                Start = startTime
            },
            Assets =
            {
                LargeImage = "logo",
                LargeText = "Godot Chat App"
            }
        };

        if (inLobby)
        {
            activity.Secrets.Join = lobbyManager.GetLobbyActivitySecret(lobbyId);
            activity.Party.Id = lobbyId.ToString();
            activity.Party.Size = new PartySize
            {
                MaxSize = 10,
                CurrentSize = lobbyManager.MemberCount(lobbyId)
            };
        }

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

    public string GetUsername()
    {
        return userManager.GetCurrentUser().Username;
    }

    public override void _Process(float delta)
    {
        discord.RunCallbacks();
        lobbyManager.FlushNetwork();
    }
}
