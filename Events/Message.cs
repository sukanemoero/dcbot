using System;
using System.Threading.Tasks;
using dcbot.Util;
using Discord.Commands;
using Discord.WebSocket;

namespace dcbot.Events;

public class Message(DiscordSocketClient client)
{
    public Task InitializeAsync()
    {
        client.MessageReceived += GetPyroxene;
        return Task.CompletedTask;
    }

    private Task GetPyroxene(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message) return Task.CompletedTask;

        var argPos = 0;
        if (message.HasCharPrefix('!', ref argPos)) return Task.CompletedTask;

        if (message.Author.IsBot ) return Task.CompletedTask;
        if (messageParam.Channel is SocketDMChannel)
        {
            Console.WriteLine($"[{messageParam.Channel.Name}] ({messageParam.Channel.Id}) -----------------------");
            Console.WriteLine($"\t{messageParam.Author.Username} ({messageParam.Author.Id}) Massage in DM channel ");
            return Task.CompletedTask;
        }
        var user = new User(message.Author.Id);
        var baseNumber = (uint)message.Content.Length + (uint)message.Attachments.Count * 50;
        user.ChattingToGetPyroxene(baseNumber);
        return Task.CompletedTask;
    }
}