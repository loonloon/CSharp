using System;
using System.Diagnostics;
using System.Linq;

namespace LoopUnrolling
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            var watch = new Stopwatch();
            var array = new int[500000000];

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = 1;
            }

            //initialize
            {
                watch.Restart();
                var sum = 0;

                for (var i = 0; i < array.Length; i++)
                {
                    sum += array[i];
                }
            }

            //example 1
            {
                watch.Restart();
                var sum = 0;

                for (var i = 0; i < array.Length; i++)
                {
                    sum += array[i];
                }

                Console.WriteLine($"for loop:{watch.ElapsedMilliseconds}ms, result:{sum}");
            }

            //example 2
            {
                watch.Restart();
                var sum = 0;
                var length = array.Length;

                for (var i = 0; i < length; i++)
                {
                    sum += array[i];
                }

                Console.WriteLine($"cached property:{watch.ElapsedMilliseconds}ms, result:{sum}");
            }

            //example 3
            {
                watch.Restart();
                var sum = array.Sum();
                Console.WriteLine($"linq:{watch.ElapsedMilliseconds}ms, result:{sum}");
            }

            //example 4
            {
                watch.Restart();
                var sum1 = 0;
                var sum2 = 0;
                var sum3 = 0;
                var sum4 = 0;
                var length = array.Length;

                for (var i = 0; i < length; i += 4)
                {
                    sum1 += array[i + 0];
                    sum2 += array[i + 1];
                    sum3 += array[i + 2];
                    sum4 += array[i + 3];
                }

                Console.WriteLine($"loop unrolling:{watch.ElapsedMilliseconds}ms, result:{(sum1 + sum2 + sum3 + sum4)}");
            }

            //example 5
            {
                watch.Restart();
                var sum = 0;

                foreach (var item in array)
                {
                    sum += item;
                }

                Console.WriteLine($"foreach loop:{watch.ElapsedMilliseconds}ms, result:{sum}");
            }

            Console.ReadKey();
        }
    }
}
