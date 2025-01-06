using System;
using System.Threading.Tasks;
using dcbot.Exceptions;
using dcbot.General.Components;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dcbot.Events.Commands;

public class Bonus : ModuleBase<SocketCommandContext>
{
    [Command("bonus")]
    [Summary("Get user's lottery")]
    public async Task BonusAsync(string message = "1", SocketUser user = null)
    {
        user ??= Context.User;
        if (user.IsBot || message == null) return;

        var player = new Util.User(user.Id);

        if (!uint.TryParse(message, out var amount) || amount is 0 or >100)
        {
            await ReplyAsync($"{message} is not a valid argument.");
            return;
        }

        try
        {
            await ReplyAsync(
                embed: BonusMessageComponents.GetUserBonusLotteryEmbed(user, player,
                    player.BonusLottery(amount)));
        }
        catch (SpendMoreThanPyroxene e)
        {
            Console.WriteLine(e);
            await ReplyAsync(
                embed: BonusMessageComponents.BonusEmbedBuilder(user)
                    .WithDescription("Pyroxene not enough")
                    .WithColor(Color.Red)
                    .AddField("Pyroxene:", e.Original.ToString(), true)
                    .AddField("Cost:", e.Cost.ToString(), true).Build()
            );
        }
    }
}