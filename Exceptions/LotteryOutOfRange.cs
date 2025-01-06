using System;

namespace dcbot.Exceptions;

public class LotteryOutOfRange : Exception
{
    public LotteryOutOfRange()
    {
    }

    public LotteryOutOfRange(string message) : base(message)
    {
    }

    public LotteryOutOfRange(string message, Exception innerException) : base(message, innerException)
    {
    }
}