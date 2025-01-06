using System;
using System.Numerics;

namespace dcbot.Exceptions;

public class SpendMoreThanGacha(uint original, uint cost) : Exception
{
    public uint Original { get; init; } = original;
    public uint Cost { get; init; } = cost;
}