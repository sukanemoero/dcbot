using System;
using dcbot.Database.SqlTable.Mysql;

namespace dcbot.Exceptions;

public class SpendMoreThanBonuses(IUserData.Bonuses original, IUserData.Bonuses cost) : Exception, IUserData
{
    public IUserData.Bonuses Original { get; init; } = original;
    public IUserData.Bonuses Cost { get; init; } = cost;
}