using System;

namespace dcbot.Exceptions;

public class GetWrongData : Exception
{
    public GetWrongData()
    {
    }

    public GetWrongData(string message) : base(message)
    {
    }

    public GetWrongData(string message, Exception innerException) : base(message, innerException)
    {
    }
}