namespace GasaiYuno.Discord.Core.Interfaces;

public interface ITranslation
{
    string Message(string path, params object[] objects);
    string Message(string path, char separator, params object[] objects);
}