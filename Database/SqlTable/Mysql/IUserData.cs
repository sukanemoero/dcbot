using System.Collections.Generic;
using System.Linq;
using dcbot.General;

namespace dcbot.Database.SqlTable.Mysql;

public interface IUserData : ILogData
{
    protected new record Tables
    {
        public const string Main = "users";
        public static Database.Mysql.TableValue MainId => new(Main, "id");
        public static Database.Mysql.TableValue MainPyroxene => new(Main, "pyroxene");
        public static Database.Mysql.TableValue MainLanguage => new(Main, "language");

        public static string Gacha = "user_gacha";
        public static Database.Mysql.TableValue GachaId => new(Gacha, "id");
        public static Database.Mysql.TableValue GachaPoint => new(Gacha, "point");

        public static string Bonuses = "user_bonuses";
        public static Database.Mysql.TableValue BonusesId => new(Bonuses, "id");
        public static Database.Mysql.TableValue BonusesHundred => new(Bonuses, "hundred");
        public static Database.Mysql.TableValue BonusesThreeHundred => new(Bonuses, "threeHundred");
        public static Database.Mysql.TableValue BonusesThousand => new(Bonuses, "thousand");


        public static string Lottery = "user_lottery";
        public static Database.Mysql.TableValue LotteryId => new(Lottery, "id");
        public static Database.Mysql.TableValue LotteryLotteryId => new(Lottery, "lotteryID");
        public static Database.Mysql.TableValue LotteryQuantity => new(Lottery, "quantity");
    }

    public record Bonuses(int Hundred = 0, int ThreeHundred = 0, int Thousand = 0)
    {
        public int Hundred { get; set; } = Hundred;
        public int ThreeHundred { get; set; } = ThreeHundred;
        public int Thousand { get; set; } = Thousand;
    }

    protected static void InitUser(ulong userId)
    {
        if (Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainId.Value, userId } }, 1) is not
            { Count: > 0 })
            Sql.Insert(Tables.Main, new Dictionary<string, object> { { Tables.MainId.Value, userId } });
    }

    protected static uint GetUserLanguage(ulong userId)
    {
        var data = Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainId.Value, userId } }, 1);
        return data is not { Count: > 0 }
            ? 1
            : GetLanguageByName(DataConverter.StringConvert(Tables.MainLanguage.Value, data.First()));
    }

    protected static uint ChangeLanguage(ulong userId)
    {
        var data = Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainId.Value, userId } }, 1);
        if (data is not { Count: > 0 }) return 1;
        var lang = GetLanguageByName(DataConverter.StringConvert(Tables.MainLanguage.Value, data.First())) == 2
            ? (uint)1
            : 2;
        Sql.Update(Tables.Main, new Dictionary<string, object> { { Tables.MainLanguage.Value, lang } },
            new Dictionary<string, object> { { Tables.MainId.Value, userId } });
        return lang;
    }

    protected static uint GetLanguageByName(string n)
    {
        return n switch
        {
            "en" => 2,
            _ => 1
        };
    }

    protected static uint GetUserPyroxene(ulong userId)
    {
        var data = Sql.Find(Tables.Main, new Dictionary<string, object> { { Tables.MainId.Value, userId } }, 1);
        return data is not { Count: > 0 }
            ? 0
            : DataConverter.NumberConvert<uint>(Tables.MainPyroxene.Value, data.First());
    }

    protected static void AdjustUserPyroxene(ulong userId, int amount)
    {
        Sql.Insert(Tables.Main,
            new Dictionary<string, object>
            {
                { Tables.MainId.Value, userId },
                { Tables.MainPyroxene.Value, amount }
            }, [Tables.MainId.Value], [Tables.MainPyroxene.Value]
        );
        LogValueChange(userId, ILogData.Tables.MainPyroxene, amount);
    }

    protected static uint GetUserGacha(ulong userId)
    {
        var data = Sql.Find(Tables.Gacha, new Dictionary<string, object> { { Tables.GachaId.Value, userId } });
        return data is not { Count: > 0 }
            ? 0
            : DataConverter.NumberConvert<uint>(Tables.GachaPoint.Value, data.First());
    }

    protected static void AdjustUserGacha(ulong userId, int amount)
    {
        Sql.Insert(Tables.Gacha,
            new Dictionary<string, object>
            {
                { Tables.GachaId.Value, userId },
                { Tables.GachaPoint.Value, amount }
            }, [Tables.GachaId.Value], [Tables.GachaPoint.Value]
        );
        LogValueChange(userId, ILogData.Tables.MainGacha, amount);
    }

    protected static Bonuses GetUserBonuses(ulong userId)
    {
        var data = Sql.Find(Tables.Bonuses, new Dictionary<string, object> { { Tables.GachaId.Value, userId } });
        return data is not { Count: > 0 }
            ? new Bonuses()
            : new Bonuses
            (
                DataConverter.NumberConvert<int>(Tables.BonusesHundred.Value, data.First()),
                DataConverter.NumberConvert<int>(Tables.BonusesThreeHundred.Value, data.First()),
                DataConverter.NumberConvert<int>(Tables.BonusesThousand.Value, data.First())
            );
    }

    protected static void AdjustUserBonuses(ulong id, Bonuses bonuses)
    {
        var idFilter = new Dictionary<string, object> { { Tables.BonusesId.Value, id } };
        var insertBonus = new Dictionary<string, object>
        {
            { Tables.BonusesHundred.Value, bonuses.Hundred },
            { Tables.BonusesThreeHundred.Value, bonuses.ThreeHundred },
            { Tables.BonusesThousand.Value, bonuses.ThreeHundred }
        };
        if (Sql.Find(Tables.Bonuses, idFilter) is not { Count: > 0 })
        {
            insertBonus.Add(Tables.BonusesId.Value, id);
            Sql.Insert(Tables.Bonuses, insertBonus);
        }
        else
        {
            Sql.Adjust(Tables.Bonuses, Tables.BonusesHundred.Value, bonuses.Hundred, idFilter);
            Sql.Adjust(Tables.Bonuses, Tables.BonusesThreeHundred.Value, bonuses.ThreeHundred, idFilter);
            Sql.Adjust(Tables.Bonuses, Tables.BonusesThousand.Value, bonuses.Thousand, idFilter);
        }

        LogValueChange(id, ILogData.Tables.MainBonusHundred, bonuses.Hundred);
        LogValueChange(id, ILogData.Tables.MainBonusThreeHundred, bonuses.ThreeHundred);
        LogValueChange(id, ILogData.Tables.MainBonusThousand, bonuses.Thousand);
    }

    protected static void AddLotteryCharacterId(ulong id, uint characterId)
    {
        Sql.Insert(
            Tables.Lottery,
            new Dictionary<string, object>
            {
                { Tables.LotteryId.Value, id },
                { Tables.LotteryLotteryId.Value, characterId },
                { Tables.LotteryQuantity.Value, 1 }
            },
            [Tables.LotteryId.Value],
            [Tables.LotteryQuantity.Value]
        );
        LogValueChange(id, ILogData.Tables.MainLotteryAmount, 1);
    }
}