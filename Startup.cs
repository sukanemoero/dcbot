using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using dcbot.Database;
using dcbot.Events;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using TokenType = Discord.TokenType;

namespace dcbot;

internal sealed class Startup 
{
    private static DiscordSocketClient Client { get; set; }
    private static Mysql Connection { get; set; }

    private static async Task<Dictionary<string, string>> GetConfig(string name)
    {
        string location = null;
        try
        {
            var parentFullName = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            if (parentFullName == null) return null;
            location = $"{parentFullName}/config/{name}.json";
            var content = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, location)
            );
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }
        catch (Exception)
        {
            await Console.Error.WriteLineAsync($"WARNING: Cannot read file ({location ?? ""})");
            return new Dictionary<string, string>();
        }
    }

    private static async Task Main(string[] args)
    {
        await DatabaseInit();

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100
        });




        Client.Log += LogAsync;
        Client.Ready += ReadyAsync;

        var h = new CommandHandler(Client);
        var m = new Message(Client);


        var token = (await GetConfig("bot_config")).GetValueOrDefault("token", "");

        await Client.LoginAsync(TokenType.Bot, token);
        
        await Client.StartAsync();

        await h.InitializeAsync();
        await m.InitializeAsync();

        await Task.Delay(-1);
    }

    private static async Task DatabaseInit()
    {
        var sqlConfig = await GetConfig("database_config");
        var host = sqlConfig.GetValueOrDefault("host", "localhost");
        var user = sqlConfig.GetValueOrDefault("user", "");
        var password = sqlConfig.GetValueOrDefault("password", "");
        var database = sqlConfig.GetValueOrDefault("database", "");

        Connection = new Mysql(host, user, password, database);
        Connection.Connect();
        IDatabase.SetDatabase(Connection);
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private static Task ReadyAsync()
    {
        Console.WriteLine($"{Client.CurrentUser} is running!");
        return Task.CompletedTask;
    }
}