using System.Diagnostics;

namespace ThreadPoolExecutor;

class Program
{
    private static long numberOfPrimes = 0;
    private static readonly long countPrimesTill = 1_000_000_000;
    private static readonly object lockObject = new ();
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        RunConcurrentTaskExecutor();
        Console.WriteLine("Goodbye, World!");
    }

    private static bool isPrime(long number)
    {
        if (number < 2) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;
        for (long i = 3; i*i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }


    private static void RunConcurrentTaskExecutor()
    {
        using IConcurrentTaskExecutor executor = new ConcurrentTaskExecutor(8);
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < countPrimesTill; i++)
        {
            var i1 = i;
            executor.QueueTask(() => {
                if(isPrime(i1)) {
                    lock(lockObject) {
                        numberOfPrimes++;
                    }
                }
                });
        }
        executor.WaitAll();
        sw.Stop();
        Console.WriteLine($"Number of primes till {countPrimesTill} is {numberOfPrimes}");
        Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds} ms or {sw.Elapsed.TotalSeconds} s");
    }
}
