using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LazyConcurrentDictionary
{
    public class Program
    {
        private static readonly ConcurrentDictionary<string, string> ConcurrentDictionary = new ConcurrentDictionary<string, string>();
        private static readonly LazyConcurrentDictionary<string, string> LazyConcurrentDictionary = new LazyConcurrentDictionary<string, string>();

        public static void Main(string[] args)
        {
            //https://blogs.endjin.com/2015/10/using-lazy-and-concurrentdictionary-to-ensure-a-thread-safe-run-once-lazy-loaded-collection/

            Parallel.For(0, 10, i =>
            {
                var value = ConcurrentDictionary.GetOrAdd("key1", k => RunOnceMethod(k, nameof(ConcurrentDictionary)));
                Console.WriteLine(value);
            });

            Parallel.For(0, 2, i =>
            {
                var value = LazyConcurrentDictionary.GetOrAdd("key2", k => RunOnceMethod(k, nameof(LazyConcurrentDictionary)));
                Console.WriteLine(value);
            });
        }

        private static string RunOnceMethod(string key, string caller)
        {
            Console.WriteLine($"[{caller}] I should only run once, because this is add when key not found");
            return $"{key}_{Guid.NewGuid()}";
        }
    }

    public class LazyConcurrentDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _concurrentDictionary;

        public LazyConcurrentDictionary()
        {
            _concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var lazyResult = _concurrentDictionary.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));
            return lazyResult.Value;
        }
    }
}
