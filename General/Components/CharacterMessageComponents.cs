using dcbot.Database.SqlTable.Mysql;
using dcbot.Util;
using Discord;
using Discord.WebSocket;

namespace dcbot.General.Components;

public class CharacterMessageComponents(SocketUser sUser, User user, IHasButtons.ButtonIds buttons) : IHasButtons
{
    public IHasButtons.ButtonIds Buttons { get; set; } = buttons;
    public EmbedBuilder GetCharacterEmbedBuilder => new EmbedBuilder().WithTitle($"{sUser.GlobalName} lottery bag");

    private MessageComponent GetCharacterButton
    (
        bool leftDisable = true,
        bool leftMiddleDisable = true,
        bool middleDisable = true,
        bool rightMiddleDisable = true,
        bool rightDisable = true
    )
    {
        return new ComponentBuilder()
            .WithButton("\u23f4\u23f4", Buttons.First, disabled: leftDisable, style: ButtonStyle.Secondary)
            .WithButton("\u25c0", Buttons.Second, disabled: leftMiddleDisable, style: ButtonStyle.Primary)
            .WithButton("\u26d8", Buttons.Third, disabled: middleDisable,
                style: middleDisable ? ButtonStyle.Danger : ButtonStyle.Success)
            .WithButton("\u25b6", Buttons.Forth, disabled: rightMiddleDisable, style: ButtonStyle.Primary)
            .WithButton("\u23f5\u23f5", Buttons.Fifth, disabled: rightDisable, style: ButtonStyle.Secondary)
            .Build();
    }

    public class GetCharactersLotteryResult
    {
        public Embed CharactersLotteryEmbed { init; get; }
        public Embed CharacterInfoEmbed { init; get; }
        public MessageComponent Buttons { init; get; }
    }

    private Embed GetCharacterInfoEmbed(ICharacterData.CharacterDataType characterId, bool changeShow)
    {
        return changeShow
            ? new LotteryMessageComponents(null, user, null).CharacterDetailsEmbed(characterId)
            : new LotteryMessageComponents(null, user, null).CharacterEmbed(characterId);
    }

    private Embed GetCharacterEmptyEmbed()
    {
        return new EmbedBuilder()
            .WithTitle($"No choice")
            .WithThumbnailUrl(sUser.GetAvatarUrl() ?? sUser.GetDefaultAvatarUrl())
            .WithColor(Color.DarkGrey)
            .Build();
    }

    public GetCharactersLotteryResult GetCharactersLotteryEmbedBySchool(uint school, int index = 0,
        bool changeShow = false)
    {
        var data = user.GetCharactersBySchool(school);
        var result = GetCharacterEmbedBuilder;

        data.Reverse();

        var description = School(school);
        ICharacterData.CharacterDataType targetCharacter = null;

        for (var i = 0; i < data.Count; i++)
        {
            var character = Characters.GetCharacter(data[i]);
            if (i % 5 == 0) description += '\n';
            if (index == i)
            {
                description += ":star2: ";
                targetCharacter ??= character;
            }
            else
            {
                description += Quality(character.BaseStar);
            }
        }

        return new GetCharactersLotteryResult
        {
            CharactersLotteryEmbed = result
                .WithDescription(description)
                .AddField("Total Count: ", user.GetLotteryCount(school: school), true)
                .Build(),
            CharacterInfoEmbed = targetCharacter != null
                ? GetCharacterInfoEmbed(targetCharacter, changeShow)
                : GetCharacterEmptyEmbed(),
            Buttons = GetCharacterButton(
                school <= 1,
                index <= 0,
                targetCharacter == null,
                index + 1 >= data.Count,
                school >= ICharacterData.SchoolCount
            )
        };

        string Quality(uint star)
        {
            return star switch
            {
                3 => ":purple_square: ",
                2 => ":yellow_square: ",
                1 => ":blue_square: ",
                _ => ":black_square: "
            };
        }

        string School(uint id)
        {
            return id switch
            {
                1 => "<:abydos:1323117900176822313> Abydos",
                2 => "<:arius:1322858238735024158> Arius",
                3 => "<:etc:1322858236541407233> ETC",
                4 => "<:gehenna:1322858233937006662> Gehenna",
                5 => "<:hyakkiyako:1322858223245594654> Hyakkiyako",
                6 => "<:millennium:1322858225401597993> Millennium",
                7 => "<:redwinter:1322858226970263658> RedWinter",
                8 => "<:shanhaijing:1322858229616738304> Shanhaijing",
                9 => "<:srt:1322858231936319539> SRT",
                10 => "<:sakugawa:1322858236541407233> Sakugawa",
                11 => "<:trinity:1322858247064916032> Trinity",
                12 => "<:valkyrie:1322858249434697798> Valkyrie",
                _ => ""
            };
        }
    }
    public GetCharactersLotteryResult GetCharactersLotteryEmbedByStar(uint star, int index = 0,
        bool changeShow = false)
    {
        var data = user.GetCharactersByStar(star);
        var result = GetCharacterEmbedBuilder;

        data.Reverse();

        var description = Star(star);
        ICharacterData.CharacterDataType targetCharacter = null;

        for (var i = 0; i < data.Count; i++)
        {
            var character = Characters.GetCharacter(data[i]);
            if (i % 5 == 0) description += '\n';
            if (index == i)
            {
                description += ":star2: ";
                targetCharacter ??= character;
            }
            else
            {
                description += Quality(character.BaseStar);
            }
        }

        return new GetCharactersLotteryResult
        {
            CharactersLotteryEmbed = result
                .WithDescription(description)
                .AddField("Total Count: ", user.GetLotteryCount(star: star), true)
                .Build(),
            CharacterInfoEmbed = targetCharacter != null
                ? GetCharacterInfoEmbed(targetCharacter, changeShow)
                : GetCharacterEmptyEmbed(),
            Buttons = GetCharacterButton(
                star <= 1,
                index <= 0,
                targetCharacter == null,
                index + 1 >= data.Count,
                star >= ICharacterData.StarCount
            )
        };

        string Quality(uint s)
        {
            return s switch
            {
                3 => ":purple_square: ",
                2 => ":yellow_square: ",
                1 => ":blue_square: ",
                _ => ":black_square: "
            };
        }
        string Star(uint s)
        {
            return s switch
            {
                3 => ":star::star::star: ",
                2 => ":star::star: ",
                1 => ":star: ",
                _ => ":black_square: "
            };
        }

        
    }
}