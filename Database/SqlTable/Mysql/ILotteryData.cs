using System.Collections.Generic;
using System.Linq;
using dcbot.General;

namespace dcbot.Database.SqlTable.Mysql;

public interface ILotteryData : ICharacterData, IUserData
{
    protected static List<uint> GetLotteryCharacterIds(ulong id, uint school = 0, uint star = 0)
    {
        var filter = new Dictionary<Database.Mysql.TableValue, object>
        {
            { IUserData.Tables.LotteryId, id }
        };
        if(school != 0) filter.Add(ICharacterData.Tables.MainSchool,school);
        if(star != 0) filter.Add(ICharacterData.Tables.MainBaseStar,star);
        var data = Sql.FindWithJoinTable(
            [ICharacterData.Tables.Main, IUserData.Tables.Lottery],
            filter,
            new Dictionary<Database.Mysql.TableValue, Database.Mysql.TableValue>
            {
                { IUserData.Tables.LotteryLotteryId, ICharacterData.Tables.MainId }
            },
            [ICharacterData.Tables.MainId],
            orderBy: ICharacterData.Tables.MainBaseStar
        );

        return data is not { Count: > 0 }
            ? []
            : data.Select(k => DataConverter.NumberConvert<uint>(ICharacterData.Tables.MainId.Value, k)).ToList();
    }

    protected static uint GetLotteryCharacterCount(ulong id, uint school = 0, uint star = 0, bool repeatable = false)
    {
        Dictionary<Database.Mysql.TableValue, object> filter = new() { { IUserData.Tables.LotteryId, id } };
        if (school != 0) filter.Add(ICharacterData.Tables.MainSchool, school);
        if (star != 0) filter.Add(ICharacterData.Tables.MainBaseStar, star);

        var data = Sql.FindWithJoinTable(
            [IUserData.Tables.Lottery, ICharacterData.Tables.Main],
            filter,
            new Dictionary<Database.Mysql.TableValue, Database.Mysql.TableValue>
            {
                { ICharacterData.Tables.MainId, IUserData.Tables.LotteryLotteryId }
            },
            count: !repeatable,
            sumColumn: repeatable ? IUserData.Tables.LotteryQuantity : null
        );
        

        return data is not { Count: > 0 } ? 0 : DataConverter.NumberConvert<uint>(IUserData.Tables.Lottery, data.First());
    }
}