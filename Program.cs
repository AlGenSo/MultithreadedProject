using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

class Program
{
    static void Main()
    {
        int[] sizes = [100000, 1000000, 10000000];

        foreach (int size in sizes)
        {
            int[] array = GenerateArray(size);

            Console.WriteLine($"Array size: {size}");

            // Последовательное вычисление
            Stopwatch sw = Stopwatch.StartNew();
            long sequentialSum = SequentialSum(array);
            sw.Stop();
            Console.WriteLine($"Sequential Sum: {sequentialSum}, Time: {sw.ElapsedMilliseconds} ms");

            // Параллельное вычисление с использованием Thread
            sw.Restart();
            long parallelSum = ParallelSumWithThreads(array);
            sw.Stop();
            Console.WriteLine($"Parallel Sum (Threads): {parallelSum}, Time: {sw.ElapsedMilliseconds} ms");

            // Параллельное вычисление с использованием LINQ
            sw.Restart();
            long parallelSumLinq = ParallelSumWithLinq(array);
            sw.Stop();
            Console.WriteLine($"Parallel Sum (LINQ): {parallelSumLinq}, Time: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine();
        }

        Console.WriteLine("Environment:");
        Console.WriteLine($"OS: {Environment.OSVersion}");
        Console.WriteLine($"Processor Count: {Environment.ProcessorCount}");
        Console.WriteLine($"Machine Name: {Environment.MachineName}");
    }

    static int[] GenerateArray(int size)
    {
        int[] array = new int[size];
        Random rand = new Random();
        for (int i = 0; i < size; i++)
        {
            array[i] = rand.Next(1, 100);
        }
        return array;
    }

    static long SequentialSum(int[] array)
    {
        long sum = 0;
        foreach (int num in array)
        {
            checked
            {
                sum += num;
            }
        }
        return sum;
    }

    static long ParallelSumWithThreads(int[] array)
    {
        int numThreads = Environment.ProcessorCount;
        int chunkSize = (int)Math.Ceiling((double)array.Length / numThreads);
        long[] sums = new long[numThreads];
        Thread[] threads = new Thread[numThreads];

        for (int i = 0; i < numThreads; i++)
        {
            int threadIndex = i;
            threads[i] = new Thread(() =>
            {
                int start = threadIndex * chunkSize;
                int end = Math.Min(start + chunkSize, array.Length);
                long localSum = 0;
                for (int j = start; j < end; j++)
                {
                    checked
                    {
                        localSum += array[j];
                    }
                }
                sums[threadIndex] = localSum;
            });
            threads[i].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        long totalSum = 0;
        foreach (long sum in sums)
        {
            checked
            {
                totalSum += sum;
            }
        }
        return totalSum;
    }

    static long ParallelSumWithLinq(int[] array)
    {
        long sum = array.AsParallel().Aggregate<int, long>(0, (currentSum, num) =>
        {
            checked
            {
                return currentSum + num;
            }
        });
        return sum;
    }
}
