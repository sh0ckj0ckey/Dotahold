using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotahold.Utils
{
    public class SerialTaskQueue
    {
        private readonly SemaphoreSlim _semaphore = new(1);

        public async Task EnqueueAsync(Func<Task> taskFunc)
        {
            await _semaphore.WaitAsync();
            try
            {
                await taskFunc();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
