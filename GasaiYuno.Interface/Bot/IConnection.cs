using System.Threading.Tasks;

namespace GasaiYuno.Interface.Bot
{
    public interface IConnection
    {
        /// <summary>
        /// Starts the connection to the service.
        /// </summary>
        /// <returns>A value indicating if the connection has been successfully created.</returns>
        Task<bool> StartAsync();

        /// <summary>
        /// Stops the connection to the service.
        /// </summary>
        Task StopAsync();
    }
}