using System;
using dcbot.Util;
using Discord;
using Discord.WebSocket;

namespace dcbot.General.Components;

public class UserInfoMessageComponents(SocketUser sUser, User user, IHasButtons.ButtonIds buttons) : IHasButtons
{
    private static Random Rand { get; } = new(DateTime.Now.Millisecond);
    public IHasButtons.ButtonIds Buttons { get; set; } = buttons;

    private EmbedBuilder UserInfoEmbedBuilder()
    {
        return new EmbedBuilder()
            .WithTitle($"**{sUser.GlobalName}**")
            .WithColor(Rand.Next(256), Rand.Next(256), Rand.Next(256))
            .WithThumbnailUrl(sUser.GetAvatarUrl() ?? sUser.GetDefaultAvatarUrl());
    }


    public MessageComponent GetUserButton(bool leftDisable = false, bool rightDisable = false, string middleText = null)
    {
        var r = new ComponentBuilder().WithButton("\u25c0", Buttons.First, ButtonStyle.Secondary,
            disabled: leftDisable);
        if (middleText is { Length: > 0 }) r.WithButton(middleText, Buttons.Second, ButtonStyle.Primary);

        r.WithButton("\u25b6", Buttons.Third, ButtonStyle.Secondary, disabled: rightDisable);
        return r.Build();
    }

    public Embed GetUserBonusInfoEmbed()
    {
        var bonus = user.GetBonus();
        return UserInfoEmbedBuilder()
            .WithDescription($"Bonus: {bonus.Hundred + bonus.ThreeHundred + bonus.Thousand}")
            .AddField("100%", bonus.Hundred, true)
            .AddField("300%", bonus.ThreeHundred, true)
            .AddField("1000%", bonus.Thousand, true)
            .Build();
    }

    public Embed GetUserLotteryInfoEmbed(bool repeatable = true)
    {
        var temp = repeatable ? "Lottery" : "Character";
        return UserInfoEmbedBuilder()
            .WithDescription($"{temp} Count: {user.GetLotteryCount(repeatable)}")
            .AddField(":star:", user.GetLotteryCount(repeatable, star: 1), true)
            .AddField(":star::star:", user.GetLotteryCount(repeatable, star: 2), true)
            .AddField(":star::star::star:", user.GetLotteryCount(repeatable, star: 3), true)
            .Build();
    }

    public Embed GetUserInfoEmbed()
    {
        return UserInfoEmbedBuilder()
            .WithDescription($"{sUser.Username}")
            .AddField("Pyroxene", user.Pyroxene, true)
            .AddField("Gacha", user.Gacha, true)
            .AddField("Language", Lang(user.Language), true)
            .Build();

        string Lang(uint i)
        {
            return i switch
            {
                2 => ":earth_americas:",
                _ => ":flag_jp:"
            };
        }
    }

    public Embed GetUserInfoLostEmbed()
    {
        return UserInfoEmbedBuilder()
            .WithDescription("Data Lost.")
            .WithColor(Color.Red)
            .Build();
    }
}