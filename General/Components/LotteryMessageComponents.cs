using System.Collections.Generic;
using dcbot.Database.SqlTable.Mysql;
using dcbot.Util;
using Discord;
using Discord.WebSocket;

namespace dcbot.General.Components;

public class LotteryMessageComponents(SocketUser sUser, User user, IHasButtons.ButtonIds buttons) : IHasButtons
{
    public EmbedBuilder LotteryEmbedBuilder()
    {
        return new EmbedBuilder().WithTitle($"{sUser.GlobalName} Lottery!");
    }

    public IHasButtons.ButtonIds Buttons { get; set; } = buttons;

    public MessageComponent GetLotteryButton(bool leftDisable = false, bool rightDisable = false,
        bool middleEnable = false)
    {
        var r = new ComponentBuilder().WithButton("\u25c0", Buttons.First, ButtonStyle.Secondary,
            disabled: leftDisable);
        if (middleEnable) r.WithButton("\u2665", Buttons.Second, ButtonStyle.Primary);
        r.WithButton("\u25b6", Buttons.Third, ButtonStyle.Secondary, disabled: rightDisable);
        return r.Build();
    }

    public Embed GetUserLotteryEmbed(List<ICharacterData.CharacterDataType> charactersData)
    {
        var result = "";
        var count = 0;
        foreach (var baseStar in charactersData)
        {
            if (count++ % 5 == 0) result += "\n";
            result += baseStar.BaseStar switch
            {
                1 => ":blue_square: ",
                2 => ":yellow_square: ",
                3 => ":purple_square: ",
                _ => ":black_large_square: "
            };
        }

        return GetUserLotteryEmbed(result, (uint)charactersData.Count);
    }

    public Embed GetUserLotteryEmbed(string message, uint count)
    {
        return LotteryEmbedBuilder()
            .WithImageUrl(sUser.GetAvatarUrl() ?? sUser.GetDefaultAvatarUrl())
            .WithDescription(message)
            .WithColor(Color.Green)
            .AddField("Pyroxene Left: ", user.Pyroxene, true)
            .AddField("Cost:", User.LotteryCost * count, true)
            .Build();
    }

    public Embed GetLotteryLostEmbed()
    {
        return LotteryEmbedBuilder()
            .WithImageUrl(sUser.GetAvatarUrl() ?? sUser.GetDefaultAvatarUrl())
            .WithDescription("Data Lost.")
            .WithColor(Color.Red)
            .Build();
    }

    public Embed CharacterDetailsEmbed(ICharacterData.CharacterDataType character, int index = -1, uint count = 0)
    {
        return new EmbedBuilder()
            .WithTitle(user.Language == 1 ? character.Name : character.EnName)
            .WithDescription(index >= 0 && count > 0 && index < count
                ? $"{Quality(character.BaseStar)} Lottery {index + 1} / {count}"
                : "")
            .WithImageUrl(
                $"https://raw.githubusercontent.com/SchaleDB/SchaleDB/refs/heads/main/images/student/icon/{character.Id}.webp")
            .WithColor(QualityColor(character.BaseStar))
            .AddField("School", $"{character.School} {School(character.School)}", true)
            .AddField("Firearm", $"{character.WeaponType} {Firearm(character.WeaponType)}")
            .AddField("Detail", user.Language == 1 ? character.Profile : character.EnProfile)
            .Build();

        string Firearm(string firearm)
        {
            return firearm switch
            {
                "AR" => $"<:{firearm}:1323120769273692270>",
                "FT" => $"<:{firearm}:1323120791658954782>",
                "GL" => $"<:{firearm}:1323120782058197043>",
                "HG" => $"<:{firearm}:1323120765926641664>",
                "MG" => $"<:{firearm}:1323120771564048424>",
                "MT" => $"<:{firearm}:1323120789507276861>",
                "RG" => $"<:{firearm}:1323120787225579582>",
                "RL" => $"<:{firearm}:1323120785023565824>",
                "SG" => $"<:{firearm}:1323120774407782410>",
                "SMG" => $"<:{firearm}:1323120776622375014>",
                "SR" => $"<:{firearm}:1323120779402936461>",
                _ => ""
            };
        }

        string School(string school)
        {
            return school switch
            {
                "Abydos" => $"<:{school}:1323117900176822313>",
                "Arius" => $"<:{school}:1322858238735024158>",
                "ETC" => $"<:{school}:1322858236541407233>",
                "Gehenna" => $"<:{school}:1322858233937006662>",
                "Hyakkiyako" => $"<:{school}:1322858223245594654>",
                "Millennium" => $"<:{school}:1322858225401597993>",
                "RedWinter" => $"<:{school}:1322858226970263658>",
                "SRT" => $"<:{school}:1322858231936319539>",
                "Sakugawa" => $"<:{school}:1322858236541407233>",
                "Trinity" => $"<:{school}:1322858247064916032>",
                "Valkyrie" => $"<:{school}:1322858249434697798>",
                _ => ""
            };
        }

        Color QualityColor(uint star)
        {
            return star switch
            {
                3 => Color.Purple,
                2 => Color.Gold,
                1 => Color.Blue,
                _ => Color.DarkGrey
            };
        }

        string Quality(uint star)
        {
            return star switch
            {
                3 => ":purple_circle:",
                2 => ":yellow_circle:",
                1 => ":blue_circle:",
                _ => ":black_circle:"
            };
        }
    }

    public Embed CharacterEmbed(ICharacterData.CharacterDataType character, int index = -1, uint count = 0)
    {
        return new EmbedBuilder()
            .WithTitle(user.Language == 1 ? character.Name : character.EnName)
            .WithDescription(index >= 0 && count > 0 && index < count
                ? $"{Quality(character.BaseStar)} Lottery {index + 1} / {count}"
                : "")
            .WithImageUrl(
                $"https://raw.githubusercontent.com/SchaleDB/SchaleDB/refs/heads/main/images/student/icon/{character.Id}.webp")
            .WithColor(QualityColor(character.BaseStar))
            .AddField(":star:", Number(character.BaseStar), true)
            .AddField(" <:emoji:1322862855959085067> | <:emoji:1322862859159470080>  ",
                $" {Type(character.AttackType)} | {Type(character.ArmorType)} ", true)
            .AddField(" <:emoji:1322858245198712932> | <:emoji:1322858242845442100> | <:emoji:1322858240765198336> ",
                $" {Affinity(character.DamageDealt.City)} | {Affinity(character.DamageDealt.Desert)} | {Affinity(character.DamageDealt.Indoor)} ",
                true)
            .AddField("Squad",
                character.SquadType == "Special"
                    ? $":blue_circle: {character.SquadType}"
                    : $":red_circle: {character.SquadType}", true)
            .Build();

        Color QualityColor(uint star)
        {
            return star switch
            {
                3 => Color.Purple,
                2 => Color.Gold,
                1 => Color.Blue,
                _ => Color.DarkGrey
            };
        }

        string Number(uint star)
        {
            return star switch
            {
                1 => ":one:",
                2 => ":two:",
                3 => ":three:",
                _ => ":black_square:"
            };
        }

        string Quality(uint star)
        {
            return star switch
            {
                3 => ":purple_circle:",
                2 => ":yellow_circle:",
                1 => ":blue_circle:",
                _ => ":black_circle:"
            };
        }

        string Affinity(uint affinity)
        {
            return affinity switch
            {
                >= 120 => $"<:{affinity}:1322857910359035957>",
                >= 110 => $"<:{affinity}:1322857944349671504>",
                >= 100 => $"<:{affinity}:1322857908077334631>",
                >= 90 => $"<:{affinity}:1322857905934045245>",
                >= 80 => $"<:{affinity}:1322857904159723625>",
                _ => ":black_square:"
            };
        }

        string Type(string type)
        {
            return type switch
            {
                "Light" or "Explosive" => ":red_square:",
                "Heavy" or "Penetration" => ":orange_square:",
                "Special" or "Mystic" => ":blue_square:",
                "Elastic" or "Sonic" => ":purple_square:",
                _ => ":black_square:"
            };
        }
    }
}