namespace Barrier;

class Program
{
    private readonly static int numberOfThreads = 16;
    private static int Counter = 0;
    private readonly static SemaphoreSlim waitForCounter = new(1);
    private readonly static SemaphoreSlim waitForOthers = new (0);
    static void Main(string[] args)
    {
        Task
            .WaitAll(GenerateParallelTasks(numberOfThreads)
                .Select(Task.Run).ToArray());
    }

    private static IEnumerable<Action> GenerateParallelTasks(int count) {
        for (int i = 0; i < count; i++) {
            yield return () => {
                SomeParallelTask();
                // wait for all to complete
                waitForCounter.Wait();
                Counter++;
                waitForCounter.Release();
                if (Counter == numberOfThreads) {
                    waitForOthers.Release();
                }
                waitForOthers.Wait();
                waitForOthers.Release();
                // then execute the next task
                AfterBarrier();
            };
        }
    }

    private static void SomeParallelTask() {
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} is running");
        Thread.Sleep(1000 + Thread.CurrentThread.ManagedThreadId * 100);
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} is done");
    }

    private static void AfterBarrier() {
        Console.WriteLine("All threads are done, now me");
    }
}
