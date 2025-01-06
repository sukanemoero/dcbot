using System;
using dcbot.Exceptions.Database;

namespace dcbot.Database;

public interface IDatabase
{
    protected static Mysql Sql =>
        (Db ?? throw new DatabaseNotConnected()).IsConnected() ? Db : Db.Connect();

    private static Mysql Db { get; set; }

    public static void SetDatabase(Mysql database)
    {
        Db = database ?? throw new ArgumentNullException(nameof(database));
    }
}