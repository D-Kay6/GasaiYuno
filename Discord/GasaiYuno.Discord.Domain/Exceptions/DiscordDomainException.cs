using System;

namespace GasaiYuno.Discord.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class DiscordDomainException : Exception
{
    public DiscordDomainException() { }

    public DiscordDomainException(string message) : base(message) { }

    public DiscordDomainException(string message, Exception innerException) : base(message, innerException) { }
}