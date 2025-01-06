using System;
using System.Linq;
using System.Threading.Tasks;
using dcbot.Exceptions;
using dcbot.General.Components;
using dcbot.Util;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dcbot.Events.Commands;

public class Gacha : ModuleBase<SocketCommandContext>
{
    private static IHasButtons.ButtonIds ButtonsIds = new("GachaAccept", "Reject");
    private const int TimeLimit = 30;

    private static class GachaCharacter
    {
        public static IHasButtons.ButtonIds GachaCharacterButtonIds(ulong id , uint character, bool show) =>
            new($"GachaCharacter:{id}:Home:{character}:{show}", $"GachaCharacter:{id}:Character:{character}:{show}" , $"GachaCharacter:{id}:Character:{character}:{!show}");

        public static async Task<bool> FunAsync(SocketMessageComponent button)
        {
            var buttonValue = button.Data.CustomId.Split(':');

            if (button.User.IsBot || buttonValue is not { Length: 5 } || buttonValue.First() != "GachaCharacter") return false;

            if (!ulong.TryParse(buttonValue[1], out var id) || id != button.User.Id)
            {
                await button.DeferAsync();
                return true;
            }
            
            Util.User user = new(button.User.Id);
            var character = Characters.GetCharacter(uint.Parse(buttonValue[3]));
            var show = bool.Parse(buttonValue[4]);

            GachaMessageComponents.GachaCharacterComponents GMCGCC = new(button.User, user, GachaCharacterButtonIds(button.User.Id,character.Id,show));
            await button.Message.ModifyAsync(msg =>
            {
                msg.Embed = buttonValue[2] switch
                {
                    "Home" => GMCGCC.GetMainEmbed(),
                    "Character" => show ? GMCGCC.GetCharacterDetailEmbed(character) : GMCGCC.GetCharacterEmbed(character),
                    _ => null
                };
                msg.Components = GMCGCC.GetGachaLotteryButton(buttonValue[2] == "Character");
            });
            
            await button.DeferAsync();
            return true;
        }
    }

    public static async Task FunAsync(SocketMessageComponent button)
    {
        if (await GachaCharacter.FunAsync(button) || button.User.IsBot || !button.Message.MentionedUsers.Contains(button.User))
            return;
        


        var bt = ButtonsIds.IdContent(button.Data.CustomId);
        if (bt < 0) return;

        GachaMessageComponents GMC = new(button.User, new Util.User(button.User.Id), ButtonsIds);
        var time = (int)Math.Ceiling((DateTime.UtcNow - button.Message.CreatedAt.UtcDateTime).TotalSeconds);
        if (time > TimeLimit)
            await button.Message.ModifyAsync(msg =>
            {
                msg.Embed = GMC.GetTimeoutEmbed(time, TimeLimit);
                msg.Components = null;
            });
        else
            try
            {
                if (bt == 1)
                {
                    await button.Message.ModifyAsync(msg =>
                    {
                        msg.Embed = GMC.GetCancelEmbed();
                        msg.Components = null;
                    });
                }
                else
                {
                    Util.User user = new(button.User.Id);
                    var character = user.UseGacha();

                    await button.Message.ModifyAsync(msg =>
                    {
                        msg.Embed = GMC.GetAcceptEmbed();
                        msg.Components = null;
                    });
                    var temp = new GachaMessageComponents.GachaCharacterComponents(button.User,
                        new Util.User(button.User.Id), GachaCharacter.GachaCharacterButtonIds(button.User.Id, character,false));
                    var replyMessage = await button.Message.ReplyAsync(embed: temp.GetMainEmbed());
                    await replyMessage.ModifyAsync((msg) =>
                    {
                        msg.Components =
                            temp.GetGachaLotteryButton(false);
                    });
                }
            }
            catch (SpendMoreThanGacha e)
            {
                await button.Message.ModifyAsync(msg =>
                {
                    msg.Embed = GMC.GetRejectEmbed(e.Original, e.Cost);
                    msg.Components = null;
                });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                await button.Message.ModifyAsync(msg =>
                {
                    msg.Embed = GMC.GetLostEmbed();
                    msg.Components = null;
                });
            }

        await button.DeferAsync();
    }


    [Command("gacha")]
    [Summary("Get user's data")]
    public async Task GachaAsync(SocketUser user = null)
    {
        user ??= Context.User;
        if (user.IsBot) return;

        Util.User u = new(user.Id);

        GachaMessageComponents GMC = new(user, u, ButtonsIds);

        var sendMassage = await Context.Message.ReplyAsync(embed: GMC.GetRequestEmbed(), options: null);
        await sendMassage.ModifyAsync(message => message.Components = GMC.GetRequestButton());
    }
}