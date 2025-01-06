using System.Linq;
using System.Threading.Tasks;
using dcbot.General.Components;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace dcbot.Events.Commands;

public class Character : ModuleBase<SocketCommandContext>
{
    
    public static IHasButtons.ButtonIds CharacterListButtonIds(uint page, int index, bool show = false, uint type = 0)
    {
        return new IHasButtons.ButtonIds(
            $"CharacterList:{page - 1}:0:{show}:{type}",
            $"CharacterList:{page}:{index - 1}:{show}:{type}",
            $"CharacterList:Info:{page}:{index}:{show}:{type}",
            $"CharacterList:{page}:{index + 1}:{show}:{type}",
            $"CharacterList:{page + 1}:0:{show}:{type}"
        );
    }

    public static async Task FunAsync(SocketMessageComponent button)
    {
        var buttonId = button.Data.CustomId;
        var message = button.Message;

        var buttonValue = buttonId.Split(":");
        if (buttonValue is not { Length: > 0 } || message.Reference == null ||
            buttonValue.First() != "CharacterList") return;
        IUserMessage referencedMessage;


        switch (button.Channel)
        {
            case ITextChannel channel:
                referencedMessage =
                    await channel.GetMessageAsync(message.Reference.MessageId.Value) as SocketUserMessage;
                break;
            case IDMChannel dmChannel:
                referencedMessage =
                    await dmChannel.GetMessageAsync(button.Message.Reference.MessageId.Value) as IUserMessage;
                break;
            default:
                return;
        }


        if (
            referencedMessage == null ||
            (button.Channel is not IDMChannel &&
             !(
                 referencedMessage is SocketMessage socketMessage &&
                 socketMessage.MentionedUsers.Contains(button.User)
             )) ||
            message.Author.Id != referencedMessage.Author.Id
        )
        {
            await button.DeferAsync();
            return;
        }

        if
        (
            uint.TryParse(buttonValue[1], out var page) &&
            int.TryParse(buttonValue[2], out var index) &&
            bool.TryParse(buttonValue[3], out var show) &&
            uint.TryParse(buttonValue[4], out var type)
        )
        {
            var user = button.User;
            var e = new CharacterMessageComponents(user, new Util.User(user.Id),
                CharacterListButtonIds(page, index, show, type));
            var c = type switch { 1 => e.GetCharactersLotteryEmbedByStar(page + 1, index, show), _ => e.GetCharactersLotteryEmbedBySchool(page + 1, index, show) };
            await referencedMessage.ModifyAsync(msg => msg.Embed =  c?.CharactersLotteryEmbed);
            await message.ModifyAsync(msg =>
            {
                msg.Embed = c.CharacterInfoEmbed;
                msg.Components = c.Buttons;
            });

            await button.DeferAsync();
        }
        else if (buttonValue[1] == "Info")
        {
            if (uint.TryParse(buttonValue[2], out var iPage) && int.TryParse(buttonValue[3], out var iIndex) &&
                bool.TryParse(buttonValue[4], out var iShow) && uint.TryParse(buttonValue[5], out var iType))
            {
                var user = button.User;

                var e = new CharacterMessageComponents(user, new Util.User(user.Id),
                    CharacterListButtonIds(iPage, iIndex, !iShow, iType));
                var c = iType switch { 1 => e.GetCharactersLotteryEmbedByStar(page + 1, iIndex, !iShow), _ => e.GetCharactersLotteryEmbedBySchool(page + 1, iIndex, !iShow) };

                await referencedMessage.ModifyAsync(msg => msg.Embed = c.CharactersLotteryEmbed);
                await message.ModifyAsync(msg =>
                {
                    msg.Embed = c.CharacterInfoEmbed;
                    msg.Components = c.Buttons;
                });

                await button.DeferAsync();
            }
        }
    }


    [Command("character")]
    [Summary("Get user's lottery")]
    public async Task CharacterAsync(string message = "star", SocketUser user = null)
    {
        user ??= Context.User;
        if (user.IsBot) return;
        var e = new CharacterMessageComponents(user, new Util.User(user.Id), CharacterListButtonIds(0, 0,type: message switch{"star" => 1, _ => 0}));
        var c = message switch { "star" => e.GetCharactersLotteryEmbedByStar(1), _ => e.GetCharactersLotteryEmbedBySchool(1) };

        var sendMassage =
            await Context.Message.ReplyAsync(embed: c.CharactersLotteryEmbed, options: null);
        var characterReply = await sendMassage
            .ReplyAsync(embed: c.CharacterInfoEmbed, options: null);
        await characterReply.ModifyAsync(msg => { msg.Components = c.Buttons; });
    }
}