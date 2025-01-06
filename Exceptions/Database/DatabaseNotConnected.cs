using System;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore.Storage;

namespace dcbot.Exceptions.Database;

public class DatabaseNotConnected : Exception
{
    public DatabaseNotConnected()
    {

    }

    public DatabaseNotConnected(string message) : base(message)
    {
    }

    public DatabaseNotConnected(string message, Exception innerException) : base(message, innerException)
    {
    }
}