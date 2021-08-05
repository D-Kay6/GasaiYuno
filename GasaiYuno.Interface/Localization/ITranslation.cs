namespace GasaiYuno.Interface.Localization
{
    public interface ITranslation
    {
        string Message(string path, params object[] objects);
        string Message(string path, char separator, params object[] objects);

        string UserPraise(params object[] objects);
        string GroupPraise(params object[] objects);
    }
}