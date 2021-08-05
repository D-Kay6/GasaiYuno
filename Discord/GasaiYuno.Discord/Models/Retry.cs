using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Models
{
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int maxAttemptCount = 3) => Do(() => action, retryInterval, maxAttemptCount);
        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();
            for (var attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                        Thread.Sleep(retryInterval);

                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }

        public static Task Do(Task task, TimeSpan retryInterval, int maxAttemptCount = 3) => Do(() => task, retryInterval, maxAttemptCount);
        public static async Task<T> Do<T>(Func<Task<T>> action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();
            for (var attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                        await Task.Delay(retryInterval);

                    return await action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}