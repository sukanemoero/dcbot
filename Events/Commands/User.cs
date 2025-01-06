using System.Linq;
using System.Threading.Tasks;
using dcbot.General.Components;
using Discord.Commands;
using Discord.WebSocket;

namespace dcbot.Events.Commands;

public class User : ModuleBase<SocketCommandContext>
{
    private static IHasButtons.ButtonIds ButtonIds(ulong id, int page, bool action)
    {
        return new IHasButtons.ButtonIds($"UserInfo:{id}:{page - 1}:{action}:Page",
            $"UserInfo:{id}:{page}:{!action}:Action",
            $"UserInfo:{id}:{page + 1}:{action}:Page");
    }

    public static async Task FunAsync(SocketMessageComponent button)
    {
        var buttonValue = button.Data.CustomId.Split(':');
        if (buttonValue is not { Length: 5 } ||
            buttonValue.First() != "UserInfo" ||
            button.User.IsBot
            ) return;

        if (
            !ulong.TryParse(buttonValue[1], out var id) ||
            id != button.User.Id
           )
        {
            await button.DeferAsync();
            return;
        }

        var page = int.TryParse(buttonValue[2], out var pageNumber) ? pageNumber : 0;
        var show = bool.TryParse(buttonValue[3], out var result) && result;
        if (page is < 0 or >= 4) return;

        var user = new Util.User(button.User.Id);
        UserInfoMessageComponents UIMC = new(button.User, user, ButtonIds(button.User.Id, page, show ^ (page != 2 &&  buttonValue[4] == "Action")));
        if (page == 0 && buttonValue[4] == "Action") user.ChangeLanguage();

        await button.Message.ModifyAsync(message =>
        {
            message.Embed = page switch
            {
                1 => UIMC.GetUserBonusInfoEmbed(),
                2 => UIMC.GetUserLotteryInfoEmbed(show),
                _ => UIMC.GetUserInfoEmbed()
            };
            message.Components = UIMC.GetUserButton(page == 0, page == 2, page switch
            {
                2 => "\u2665",
                0 => "\ud83c\udf0e",
                _ => ""
            });
        });


        await button.DeferAsync();
    }


    [Command("user")]
    [Summary("Get user's data")]
    public async Task UserAsync(SocketUser user = null)
    {
        user ??= Context.User;
        if (user.IsBot) return;

        UserInfoMessageComponents UIMC = new(user, new Util.User(user.Id), ButtonIds(user.Id, 0, false));

        var sendMassage = await Context.Channel.SendMessageAsync(embed: UIMC.GetUserInfoEmbed(), options: null);
        await sendMassage.ModifyAsync(msg => msg.Components = UIMC.GetUserButton(true, middleText: "\ud83c\udf0e"));
    }
}