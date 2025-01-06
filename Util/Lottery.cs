using System;
using System.Collections.Generic;
using dcbot.Database.SqlTable.Mysql;
using dcbot.Exceptions;

namespace dcbot.Util;

public class Lottery : ILotteryData
{
    private static Random Rand { get; } = new(DateTime.Now.Millisecond);

    public static List<uint> GetLotteryResults(uint amount)
    {
        if (amount > 10) throw new LotteryOutOfRange(nameof(GetLotteryResults));
        var lotteryList = new List<uint>();

        for (var i = 1; i <= amount; i++)
            lotteryList.Add(ILotteryData.GetRandomCharacter(Rand.Next(1000) switch
            {
                < 30 => 3,
                < 215 => 2,
                _ => i % 10 == (uint)0 ? (uint)2 : 1
            }));

        if (lotteryList.Count > 0) return lotteryList;

        throw new LotteryError($"No lottery results {nameof(lotteryList)}");
    }

    public static uint GetLotteryGlory()
    {
        return ILotteryData.GetRandomCharacter(3);
    }
    
    public static IUserData.Bonuses GetBonusLotteryResults(uint amount)
    {
        IUserData.Bonuses result = new() { Hundred = 0, ThreeHundred = 0, Thousand = 0 };
        for (var i = 0; i < amount; i++)
            switch (Rand.Next(100))
            {
                case < 75:
                    result.Hundred++;
                    break;
                case < 90:
                    result.ThreeHundred++;
                    break;
                default:
                    result.Thousand++;
                    break;
            }

        return result;
    }

    public static List<uint> GetLotteryCharacterIds(ulong id, uint school = 0, uint star = 0)
    {
        return ILotteryData.GetLotteryCharacterIds(id, school,star);
    }

    public static uint GetLotteryCount(ulong id, uint school = 0, uint star = 0)
    {
        return ILotteryData.GetLotteryCharacterCount(id, school, star, true);
    }

    public static uint GetLotteryCharacterCount(ulong id, uint school = 0, uint star = 0)
    {
        return ILotteryData.GetLotteryCharacterCount(id, school, star);
    }

    public static uint GetPyroxene(double baseNumber, uint bonus = 1)
    {
        return Math.Max(
            (uint)Math.Round((Math.Pow(baseNumber, 3d / 5d) * (Rand.NextDouble() / 2) + Math.Min(baseNumber / 2, 50)) *
                             bonus), 1);
    }
}