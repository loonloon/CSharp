using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncWithMapReduce
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            ProcessCmdLineArgs(args, out var infile);
            sw.Restart();

            var reviewsByUser = new Dictionary<int, int>();

            //foreach (string line in File.ReadLines(infile))
            Parallel.ForEach(File.ReadLines(infile),

                // Initializer:  create task-local Dictionary:
                () => new Dictionary<int, int>(),

                // Loop-body: work with TLS which represents a local Dictionary,
                // mapping our results into this local dictionary:
                (line, loopControl, localD) =>
                {
                    // movie id, user id, rating (1..5), date (YYYY-MM-DD)
                    var userid = Parse(line);

                    // first review:
                    if (!localD.ContainsKey(userid))
                    {
                        localD.Add(userid, 1);
                    }
                    else
                    {
                        // another review by same user:
                        localD[userid]++;
                    }

                    // return out so it can be passed back in later:
                    return localD;
                },

                // Finalizer: reduce individual local dictionaries into global dictionary:
                localD =>
                {
                    lock (reviewsByUser)
                    {
                        // merge into global data structure:
                        foreach (var userid in localD.Keys)
                        {
                            var numReviews = localD[userid];

                            // first review:
                            if (!reviewsByUser.ContainsKey(userid))
                            {
                                reviewsByUser.Add(userid, numReviews);
                            }
                            else
                            {
                                // another review by same user:
                                reviewsByUser[userid] += numReviews;
                            }
                        }
                    }
                }

            );

            var sort = from user in reviewsByUser
                       orderby user.Value descending, user.Key
                       select new { Userid = user.Key, NumReviews = user.Value };

            var top10 = sort.Take(10).ToList();
            var timeInMs = sw.ElapsedMilliseconds;

            Console.WriteLine();
            Console.WriteLine("** Top 10 users reviewing movies:");

            foreach (var user in top10)
            {
                Console.WriteLine("{0}: {1}", user.Userid, user.NumReviews);
            }

            // convert milliseconds to secs
            var time = timeInMs / 1000.0;

            Console.WriteLine();
            Console.WriteLine("** Done! Time: {0:0.000} secs", time);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Press a key to exit...");
            Console.ReadKey();
        }


        /// <summary>
        /// Parses one line of the netflix data file, and returns the userid who reviewed the movie.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static int Parse(string line)
        {
            char[] separators = { ',' };
            var tokens = line.Split(separators);
            var movieid = Convert.ToInt32(tokens[0]);
            var userid = Convert.ToInt32(tokens[1]);
            var rating = Convert.ToInt32(tokens[2]);
            var date = Convert.ToDateTime(tokens[3]);
            return userid;
        }


        /// <summary>
        /// Processes any command-line args (currently there are none), returning
        /// the input file to read from.
        /// </summary>
        /// <param name="args">cmd-line args to program</param>
        /// <param name="infile">input file to read from</param>
        ///
        static void ProcessCmdLineArgs(string[] args, out string infile)
        {
            string version, platform;

#if DEBUG
            version = "debug";
#else
            version = "release";
#endif

#if _WIN64
	platform = "64-bit";
#elif _WIN32
	platform = "32-bit";
#else
            platform = "any-cpu";
#endif


            infile = "ratings.txt";

            if (args.Length != 0)
            {
                Console.WriteLine("Usage: top10.exe");
                Console.WriteLine();
                Console.WriteLine();
                Environment.Exit(-1);
            }

            if (!File.Exists(infile))
            {
                Console.WriteLine("** Error: infile '{0}' does not exist.", infile);
                Console.WriteLine();
                Console.WriteLine();
                Environment.Exit(-1);
            }

            var fi = new FileInfo(infile);
            var sizeInMb = fi.Length / 1048576.0;

            Console.WriteLine("** Parallel, MapReduce Top-10 Netflix Data Mining App [{0}, {1}] **", platform, version);
            Console.Write("   Infile:  '{0}' ({1:#,##0.00 MB})", infile, sizeInMb);
            Console.WriteLine();
        }
    }
}
