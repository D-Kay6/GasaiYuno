namespace GasaiYuno.Interface.Storage
{
    public interface IConfigStorage
    {
        /// <summary>
        /// Get the configuration file from the disk.
        /// If it does not exist, a new one is created.
        /// </summary>
        /// <typeparam name="T">The type of the configuration file.</typeparam>
        /// <returns><see cref="T"/></returns>
        T Read<T>() where T : struct;

        /// <summary>
        /// Get the configuration file from the disk.
        /// If it does not exist, a new one is created.
        /// </summary>
        /// <typeparam name="T">The type of the configuration file.</typeparam>
        /// <param name="name">The name of the configuration file.</param>
        /// <returns><see cref="T"/></returns>
        T Read<T>(string name) where T : struct;
    }
}