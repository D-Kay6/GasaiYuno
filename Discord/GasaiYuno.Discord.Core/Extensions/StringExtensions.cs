using System.Security.Cryptography;

namespace GasaiYuno.Discord.Core.Extensions;

public static class StringExtensions
{
    public static string ToHash(this string input, string salt = "")
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var textBytes = System.Text.Encoding.UTF8.GetBytes(input + salt);
        var hashBytes = SHA256.HashData(textBytes);
        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }

    public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);

    public static bool IsNullOrWhiteSpace(this string input) => string.IsNullOrWhiteSpace(input);
}