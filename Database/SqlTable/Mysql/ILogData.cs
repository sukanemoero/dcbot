using System.Collections.Generic;
using System.Numerics;

namespace dcbot.Database.SqlTable.Mysql;

public interface ILogData : IDatabase
{
    protected abstract record Tables
    {
        public const string Main = "log_user_value_changes";
        public static readonly Database.Mysql.TableValue MainId = new(Main, "id");
        public static readonly Database.Mysql.TableValue MainUserId = new(Main, "userID");
        public static readonly Database.Mysql.TableValue MainPyroxene = new(Main, "pyroxene");
        public static readonly Database.Mysql.TableValue MainLotteryAmount = new(Main, "lotteryAmount");
        public static readonly Database.Mysql.TableValue MainGacha = new(Main, "gacha");
        public static readonly Database.Mysql.TableValue MainBonusHundred = new(Main, "bonusHundred");
        public static readonly Database.Mysql.TableValue MainBonusThreeHundred = new(Main, "bonusThreeHundred");
        public static readonly Database.Mysql.TableValue MainBonusThousand = new(Main, "bonusThousand");
        public static readonly Database.Mysql.TableValue MainTime = new(Main, "time");
    }

    protected static void LogValueChange<T>(ulong userId, Database.Mysql.TableValue tableValue, T value)
        where T : INumber<T>
    {
        if (tableValue.Table is not Tables.Main) return;
        var column = tableValue.Value;
        Sql.Insert(Tables.Main,
            new Dictionary<string, object> { { Tables.MainUserId.Value, userId }, { column, value } },
            [Tables.MainTime.Value], [column]);
    }
}