using System.Threading.Tasks;

namespace GasaiYuno.Interface.Bot
{
    public interface IConnection
    {
        /// <summary>
        /// Whether the connection needs to be restarted.
        /// </summary>
        bool Restart { get; }

        /// <summary>
        /// Starts the connection to the service.
        /// </summary>
        Task ConnectAsync();

        /// <summary>
        /// Stops the connection to the service.
        /// </summary>
        Task DisconnectAsync();
    }
}