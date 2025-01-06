using System;

namespace dcbot.Exceptions;

public class SetLanguageError : Exception
{
    public SetLanguageError()
    {
    }

    public SetLanguageError(string message) : base(message)
    {
    }

    public SetLanguageError(string message, Exception innerException) : base(message, innerException)
    {
    }
}