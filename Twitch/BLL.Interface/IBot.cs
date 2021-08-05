using BLL.Interface.Events;
using System;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IBot
    {
        event Func<ClientLogArgs, Task> OnLog;

        Task Start();
    }
}