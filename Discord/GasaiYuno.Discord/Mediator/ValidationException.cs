using System;

namespace GasaiYuno.Discord.Mediator;

/// <summary>
/// Exception type for mediator validation errors
/// </summary>
public class ValidationException : Exception
{
    public ValidationException() { }

    public ValidationException(string message) : base(message) { }

    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}