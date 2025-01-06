using System;
using System.Collections.Generic;
using System.Linq;
using dcbot.Database.SqlTable.Mysql;
using dcbot.Exceptions;

namespace dcbot.Util;

public class User : IUserData
{
    private ulong Id { get; }

    public const uint LotteryCost = 120;
    public const uint BonusCost = 120;
    public const uint GachaCost = 200;

    public uint Pyroxene => GetPyroxene();
    public uint Gacha => GetGacha();
    public uint Language => GetLanguage();

    public User(ulong id)
    {
        Id = id;
        IUserData.InitUser(Id);
    }

    private uint GetPyroxene()
    {
        return IUserData.GetUserPyroxene(Id);
    }

    private void AdjustPyroxene(int adjust)
    {
        var op = GetPyroxene();
        if (op +  adjust < 0) throw new SpendMoreThanPyroxene(op, (uint)Math.Abs(adjust));
        IUserData.AdjustUserPyroxene(Id, int.Parse(adjust.ToString()));
    }

    public void ChattingToGetPyroxene(uint chatLength)
    {
        if (chatLength <= 0) return;
        var bonus = (uint)1;
        try
        {
            bonus = UseBonus(1);
        }
        catch (SpendMoreThanBonuses)
        {
            bonus = 1;
        }
        finally
        {
            AdjustPyroxene((int)Util.Lottery.GetPyroxene(chatLength, bonus));
        }
    }

    private void UpdateBonus(IUserData.Bonuses bonuses)
    {
        IUserData.AdjustUserBonuses(Id, bonuses);
    }

    private void UpdateGacha(int amount)
    {
        IUserData.AdjustUserGacha(Id, amount);
    }

    private void UpdateLottery(List<uint> lottery)
    {
        foreach (var l in lottery) IUserData.AddLotteryCharacterId(Id, l);
    }

    private uint GetLanguage()
    {
        return IUserData.GetUserLanguage(Id);
    }

    public void ChangeLanguage()
    {
        IUserData.ChangeLanguage(Id);
    }

    public List<uint> Lottery(uint amount)
    {
        if (amount > 10) throw new LotteryOutOfRange(nameof(Lottery));

        var data = GetPyroxene();
        var cost = (int)amount * (int)LotteryCost;
        if ((int)data - cost < 0) throw new SpendMoreThanPyroxene(data, (uint)cost);

        var lottery = Util.Lottery.GetLotteryResults(amount);
        if (lottery.Count != amount) throw new LotteryError($"Lottery count mismatch {lottery.Count} and {amount}");
        foreach (var l in lottery) IUserData.AddLotteryCharacterId(Id, l);
        AdjustPyroxene(cost * -1);
        IUserData.AdjustUserGacha(Id, (int)amount);
        
        return lottery;
    }

    public IUserData.Bonuses BonusLottery(uint amount)
    {
        if (amount <= 0) throw new LotteryOutOfRange(nameof(BonusLottery));

        var data = GetPyroxene();
        var cost = amount * BonusCost;
        if (data < cost) throw new SpendMoreThanPyroxene(data, cost);

        var bonus = Util.Lottery.GetBonusLotteryResults(amount);

        UpdateBonus(bonus);
        
        return bonus;
    }

    public IUserData.Bonuses GetBonus()
    {
        return IUserData.GetUserBonuses(Id);
    }

    private uint UseBonus(uint amount)
    {
        if (amount <= 0) return 1;

        var data = IUserData.GetUserBonuses(Id);


        var resultBonus = new IUserData.Bonuses();

        for (var i = 0; i < amount; i++)
        {
            if (data.Thousand - resultBonus.Thousand > 0)
            {
                resultBonus.Thousand++;
                continue;
            }

            if (data.ThreeHundred - resultBonus.ThreeHundred > 0)
            {
                resultBonus.ThreeHundred++;
                continue;
            }

            if (data.Hundred - resultBonus.Hundred <= 0)
                throw new SpendMoreThanBonuses(resultBonus,
                    new IUserData.Bonuses
                    {
                        Hundred = resultBonus.Hundred + 1,
                        ThreeHundred = resultBonus.ThreeHundred,
                        Thousand = resultBonus.Thousand
                    });
            resultBonus.Hundred++;
        }


        IUserData.AdjustUserBonuses(Id, new IUserData.Bonuses(
            resultBonus.Hundred * -1,
            resultBonus.ThreeHundred * -1,
            resultBonus.Thousand * -1));
        var r = 1 + Math.Abs(resultBonus.Hundred) + Math.Abs(resultBonus.ThreeHundred) * 3 +
                Math.Abs(resultBonus.Thousand) * 10;

        return uint.Parse(r.ToString());
    }

    private uint GetGacha()
    {
        return IUserData.GetUserGacha(Id);
    }

    public uint UseGacha()
    {
        var point = GetGacha();
        if (point < GachaCost) throw new SpendMoreThanGacha(point, GachaCost);

        var lottery = new List<uint> { Util.Lottery.GetLotteryGlory() };

        UpdateLottery(lottery);
        UpdateGacha(int.Parse(GachaCost.ToString()) * -1);

        return lottery.First();
    }

    public uint GetLotteryCount(bool repeatable = false, uint school = 0, uint star = 0)
    {
        return repeatable
            ? Util.Lottery.GetLotteryCount(Id, school, star)
            : Util.Lottery.GetLotteryCharacterCount(Id, school, star);
    }

    public List<uint> GetCharactersBySchool(uint school)
    {
        return Util.Lottery.GetLotteryCharacterIds(Id, school);
    }
    public List<uint> GetCharactersByStar(uint star)
    {
        return Util.Lottery.GetLotteryCharacterIds(Id, star: star);
    }
}