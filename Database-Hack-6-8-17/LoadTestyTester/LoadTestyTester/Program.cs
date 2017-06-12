using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace LoadTestyTester
{
    class ApiLoader
    {
        static readonly string urlToRuin = "https://databasehack.azurewebsites.net/api/NewProfile?code=YV5BClf7GkQpqQ4KwHfKq/J8scqGszT8oN2jwJJTadGO3su/KondHw==";
        //GET "https://databasehack.azurewebsites.net/api/TakeIt?code=bk/63rq7WnQWmoqmDgLa9ZiCfpJ3RBJYPXCY62Id/S2BhYjhhh3cjg==";
        static int maxCounter = 10;
        static int counter = 0;

        static Stopwatch watch = new Stopwatch();
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            bool shouldRun = true;
            while (shouldRun)
            {
                try
                {
                    watch.Start();
                    Task.Run(() => RunData()).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
                counter++;
                if (counter == maxCounter)
                    shouldRun = false;
            }
            watch.Stop();
            writeTime(maxCounter * counter);
            Console.ReadLine();
        }
        public static async Task RunData()
        {
            List<Task<string>> tasks = new List<Task<string>>();
            for (var x = 0; x < 1000; x++)
                tasks.Add(callHttp(x));
            await Task.WhenAll(tasks);
        }
        static public async Task<string> callHttp(int dataNumber)
        {
            string astr = await client.GetStringAsync(urlToRuin);
            writeTime(dataNumber);
            return astr;
        }
        static void writeTime(int dataNumber)
        {
            TimeSpan ts = watch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine($"Run {dataNumber + (counter * 1000)} completed at: " + elapsedTime);
        }
    }
}