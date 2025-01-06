using dcbot.Database.SqlTable.Mysql;
using dcbot.Util;
using Discord;
using Discord.WebSocket;

namespace dcbot.General.Components;

public class GachaMessageComponents(SocketUser sUser, User user, IHasButtons.ButtonIds buttons) : IHasButtons
{
    public IHasButtons.ButtonIds Buttons { get; set; } = buttons;

    private EmbedBuilder GachaEmbedBuilder => new EmbedBuilder().WithTitle($"{sUser.GlobalName} use gacha point.")
        .WithThumbnailUrl(sUser.GetAvatarUrl() ?? sUser.GetDefaultAvatarUrl());


    public class GachaCharacterComponents(SocketUser sUser, User user, IHasButtons.ButtonIds buttons) : IHasButtons
    {
        public IHasButtons.ButtonIds Buttons { get; set; } = buttons;

        public MessageComponent GetGachaLotteryButton(bool isCharacter)
        {

            return isCharacter
                ? new ComponentBuilder()
                    .WithButton("\u2617", Buttons.First, ButtonStyle.Secondary)
                    .WithButton("\u2605", Buttons.Third)
                    .Build()
                : new ComponentBuilder()
                    .WithButton("\u2665", Buttons.Second, ButtonStyle.Success)
                .Build();
        }


        public Embed GetMainEmbed()
        {
            return new EmbedBuilder()
                .WithTitle($"{sUser.GlobalName} got the SSR character!")
                .WithImageUrl(sUser.GetAvatarUrl() ?? sUser.GetDefaultAvatarUrl())
                .WithDescription(":star2: ")
                .WithColor(Color.LightOrange)
                .AddField("Gacha Left: ", user.Gacha, true)
                .AddField("Cost:", User.GachaCost, true)
                .Build();
        }

        public Embed GetCharacterEmbed(ICharacterData.CharacterDataType character)
        {
            return new LotteryMessageComponents(null, user, null).CharacterEmbed(character);
        }

        public Embed GetCharacterDetailEmbed(ICharacterData.CharacterDataType character)
        {
            return new LotteryMessageComponents(null, user, null).CharacterDetailsEmbed(character);
        }
    }

    public Embed GetRequestEmbed()
    {
        var gacha = user.Gacha;
        var after = (int)gacha - (int)User.GachaCost;
        return GachaEmbedBuilder
            .WithColor(Color.DarkPurple)
            .WithDescription($"Spend {User.GachaCost} Gacha Points to get random SSR character.")
            .AddField("Before: ", gacha, true)
            .AddField("After: ", after >= 0 ? after.ToString() : "Not enough.", true)
            .Build();
    }

    public Embed GetAcceptEmbed()
    {
        return GachaEmbedBuilder
            .WithColor(Color.Green)
            .WithDescription("Accept")
            .AddField("Left Gacha Point: ", user.Gacha, true)
            .AddField("Cost: ", User.GachaCost, true)
            .Build();
    }

    public Embed GetCancelEmbed()
    {
        return GachaEmbedBuilder
            .WithColor(Color.DarkGreen)
            .WithDescription("You canceled.")
            .Build();
    }

    public Embed GetTimeoutEmbed(int spend, int cost = 0)
    {
        return GachaEmbedBuilder
            .WithColor(Color.Red)
            .WithDescription($"Timeout: {cost - spend}s.")
            .Build();
    }

    public Embed GetLostEmbed()
    {
        return GachaEmbedBuilder
            .WithColor(Color.Red)
            .WithDescription($"Data Lost")
            .Build();
    }

    public Embed GetRejectEmbed(uint o, uint c)
    {
        return GachaEmbedBuilder
            .WithColor(Color.Red)
            .WithDescription("Gacha point is not enough!")
            .AddField("Gacha: ", o, true)
            .AddField("Cost: ", c, true)
            .Build();
    }

    public MessageComponent GetRequestButton()
    {
        return new ComponentBuilder()
            .WithButton("\u2713", Buttons.First, ButtonStyle.Success)
            .WithButton("\u2715", Buttons.Second, ButtonStyle.Danger)
            .Build();
    }
}