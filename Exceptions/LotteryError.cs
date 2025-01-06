using System;

namespace dcbot.Exceptions;

public class LotteryError : Exception
{
    public LotteryError()
    {
    }

    public LotteryError(string message) : base(message)
    {
    }

    public LotteryError(string message, Exception innerException) : base(message, innerException)
    {
    }
}