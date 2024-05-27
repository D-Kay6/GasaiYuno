namespace GasaiYuno.Discord.Core.Interfaces;

public interface ILocalization
{
    string Translate(string code);
    string Translate(string code, char separator);
    string Translate(string code, params object[] objects);
    string Translate(string code, char separator, params object[] objects);
}