using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotahold.Utils
{
    public class SerialTaskQueue(int initialCount = 1)
    {
        private readonly SemaphoreSlim _semaphore = new(initialCount);

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
