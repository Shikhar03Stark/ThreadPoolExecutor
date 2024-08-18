
using System.Collections.Concurrent;

namespace ThreadPoolExecutor;

public class ConcurrentTaskExecutor : IConcurrentTaskExecutor
{
    private readonly ConcurrentQueue<Action> _tasks = [];
    private readonly List<Thread> _threads = [];
    private bool _disposed = false;

    public ConcurrentTaskExecutor(int maxThreads) {
        for(int t = 0; t<maxThreads; t++) 
        {
            var thread = new Thread(() => 
            {
                while(!_disposed) 
                {
                    if(_tasks.TryDequeue(out var execTask)){
                        execTask();
                    }
                }
            });
            thread.Start();
            _threads.Add(thread);
        }
    }

    public void Dispose()
    {
        if(_disposed) return;

        var disposeThread = new Thread(() => {
            while(!_tasks.IsEmpty);
            _disposed = true;
            foreach (var thread in _threads) 
            {
                thread.Join();
            }
        });
        disposeThread.Start();

    }

    public void QueueTask(Action task)
    {
        _tasks.Enqueue(task);
    }

    public void WaitAll()
    {
        while(!_tasks.IsEmpty);
    }
}
