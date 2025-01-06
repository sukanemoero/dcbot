using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dcbot.Database.SqlTable.Mysql;
using dcbot.Exceptions;
using dcbot.Exceptions.Database;
using dcbot.General.Components;
using dcbot.Util;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dcbot.Events.Commands;

public class Lottery : ModuleBase<SocketCommandContext>
{
    private static IHasButtons.ButtonIds ButtonIds(ulong id, int page, bool show, List<uint> characterIds)
    {
        var temp = string.Join(":", characterIds);
        return new IHasButtons.ButtonIds($"Lottery:{id}:{page - 1}:{show}:{temp}",
            $"Lottery:{id}:{page}:{!show}:{temp}", $"Lottery:{id}:{page + 1}:{show}:{temp}");
    }


    public static async Task FunAsync(SocketMessageComponent button)
    {
        var buttonValue = button.Data.CustomId.Split(':');

        if (button.User.IsBot || buttonValue is not { Length: > 4 } || buttonValue.First() != "Lottery") return;


        bool show;
        int page;
        List<uint> characterIds = [];
        {
            if (!ulong.TryParse(buttonValue[1], out var id) || id != button.User.Id)
            {
                await button.DeferAsync();
                return;
            }

            page = int.TryParse(buttonValue[2], out var p) ? p : 0;
            show = bool.TryParse(buttonValue[3], out var s) && s;

            for (var i = 4; i < buttonValue.Length; i++) characterIds.Add(uint.Parse(buttonValue[i]));
        }

        if (page > characterIds.Count || page < 0) 
        {
            await button.DeferAsync();
            return;
        }


        LotteryMessageComponents LMC = new(button.User, new Util.User(button.User.Id),
            ButtonIds(button.User.Id, page, show, characterIds));
        try
        {
            await button.Message.ModifyAsync(message =>
            {
                if (page == 0)
                {
                    var v = characterIds.Select(Characters.GetCharacter).ToList();
                    message.Embed = LMC.GetUserLotteryEmbed(v);
                }
                else
                {
                    var character = Characters.GetCharacter(characterIds[page - 1]);

                    message.Embed =
                        show
                            ? LMC.CharacterDetailsEmbed(character, page - 1, (uint)characterIds.Count)
                            : LMC.CharacterEmbed(character, page - 1, (uint)characterIds.Count);
                }

                message.Components =
                    LMC.GetLotteryButton(page == 0,
                        page >= characterIds.Count, page != 0);
            });

            await button.DeferAsync();
        }
        catch (DatabaseNotFoundData)
        {
            await button.Message.ModifyAsync(message =>
            {
                message.Embed = LMC.GetLotteryLostEmbed();
                message.Components = null;
            });
            await button.DeferAsync();
            

        }
    }


    [Command("lottery")]
    [Summary("Get user's lottery")]
    private async Task LotteryAsync(string message = "1", SocketUser user = null)
    {
        user ??= Context.User;
        if (user.IsBot || message == null) return;

        var player = new Util.User(user.Id);


        if (!int.TryParse(message, out var amount) || amount is not 1 and not 10)
        {
            await ReplyAsync($"{message} is not a valid argument.");
            return;
        }

        try
        {
            var lottery = player.Lottery(uint.Parse(amount.ToString()));

            var LMC = new LotteryMessageComponents(user, player, ButtonIds(user.Id, 0, false, lottery));

            List<ICharacterData.CharacterDataType> charactersData = [];
            charactersData.AddRange(lottery.Select(Characters.GetCharacter));

            var sendMassage =
                await Context.Channel.SendMessageAsync(embed: LMC.GetUserLotteryEmbed(charactersData), options: null);
            await sendMassage.ModifyAsync(msg =>
                msg.Components = LMC.GetLotteryButton(charactersData.Count > 0));
        }
        catch (SpendMoreThanPyroxene e)
        {
            Console.WriteLine(e);
            await ReplyAsync(
                embed: new LotteryMessageComponents(user, new Util.User(user.Id), null).LotteryEmbedBuilder()
                    .WithDescription("Pyroxene not enough")
                    .WithColor(Color.Red)
                    .AddField("Pyroxene:", e.Original.ToString(), true)
                    .AddField("Cost:", e.Cost.ToString(), true).Build()
            );
        }
    }
}