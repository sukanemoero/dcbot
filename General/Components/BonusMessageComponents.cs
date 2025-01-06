using dcbot.Database.SqlTable.Mysql;
using dcbot.Util;
using Discord;
using Discord.WebSocket;

namespace dcbot.General.Components;

public static class BonusMessageComponents
{
    public static EmbedBuilder BonusEmbedBuilder(SocketUser u)
    {
        return new EmbedBuilder().WithTitle($"{u.GlobalName} Bonus Lottery!");
    }

    public static Embed GetUserBonusLotteryEmbed(SocketUser socketUser, User user, IUserData.Bonuses bonus)
    {
        var userBonus = user.GetBonus();
        return BonusEmbedBuilder(socketUser).WithColor(Color.Green)
            .AddField("100%", $"{userBonus.Hundred - bonus.Hundred} -> {userBonus.Hundred}", true)
            .AddField("300%", $"{userBonus.ThreeHundred - bonus.ThreeHundred} -> {userBonus.ThreeHundred}", true)
            .AddField("1000%", $"{userBonus.Thousand - bonus.Thousand} -> {userBonus.Thousand}", true)
            .Build();
    }
}