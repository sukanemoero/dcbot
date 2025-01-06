using System;

namespace dcbot.Exceptions.Database;

public class DatabaseNotFoundData : Exception
{
    public DatabaseNotFoundData()
    {
    }

    public DatabaseNotFoundData(string message) : base(message)
    {
    }

    public DatabaseNotFoundData(string message, Exception innerException) : base(message, innerException)
    {
    }
}