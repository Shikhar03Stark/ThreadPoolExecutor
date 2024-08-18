namespace ThreadPoolExecutor;

public interface IConcurrentTaskExecutor: IDisposable
{
    void QueueTask(Action task);
    void WaitAll();
}
