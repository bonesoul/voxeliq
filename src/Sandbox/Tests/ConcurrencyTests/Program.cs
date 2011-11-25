/*    
 * Copyright (C) 2011, Hüseyin Uslu - shalafiraistlin@gmail.com
 *  
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU General 
 * Public License as published by the Free Software Foundation, either version 3 of the License, or (at your 
 * option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the 
 * implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License 
 * for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program.  If not, see 
 * <http://www.gnu.org/licenses/>. 
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrencyTests
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] feeds = {
                                 "http://www.teamliquid.net/rss/news.xml",
                                 "http://www.blizztv.com/rss/ccs/1-blizztvcom/",
                                 "http://feeds.feedburner.com/blizzplanetcom",
                                 "http://www.tehgladiators.com/rss.xml",
                                 "http://www.worldofraids.com/rss/forums/1-world-of-raids-news/",
                                 "http://wow.joystiq.com/rss.xml",
                                 "http://www.wowhead.com/blog&rss",
                                 "http://www.tentonhammer.com/wow/all/feed",
                                 "http://feeds.feedburner.com/LookingForGroup",
                                 "http://www.mmo-champion.com/external.php?do=rss&type=newcontent&sectionid=1&days=120&count=10"
                             };

            const int iterationCount = 10;
            var timeSpanSequentials = new TimeSpan[iterationCount];
            var timeSpanParallelForEachs = new TimeSpan[iterationCount];
            var timeSpanTPLs = new TimeSpan[iterationCount];
            var timeSpanThreadPools = new TimeSpan[iterationCount];

            PrintCPUInfo();
            Console.WriteLine("Will be parsing a total of {0} feeds.", feeds.Length);
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine("Itr.\tSeq.\tPrlEx\tTPL\tTPool");
            Console.WriteLine("________________________________________________________________________________");

            for (int i = 0; i < iterationCount; i++)
            {
                Console.Write("#{0}\t", i + 1);

                TimeSpan timeSpanSequential = MeasureSequential(feeds);
                timeSpanSequentials[i] = timeSpanSequential;
                Console.Write(String.Format("{0:00}.{1:00}s\t",timeSpanSequential.Seconds, timeSpanSequential.Milliseconds / 10));

                TimeSpan timeSpanParallelForeach = MeasureParallelForeach(feeds);
                timeSpanParallelForEachs[i] = timeSpanParallelForeach;
                Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanParallelForeach.Seconds, timeSpanParallelForeach.Milliseconds / 10));

                TimeSpan timeSpanTPL = MeasureTPL(feeds);
                timeSpanTPLs[i] = timeSpanTPL;
                Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanTPL.Seconds, timeSpanTPL.Milliseconds / 10));

                TimeSpan timeSpanThreadPool = MeasureNativeThreadPool(feeds);
                timeSpanThreadPools[i] = timeSpanThreadPool;
                Console.Write(String.Format("{0:00}.{1:00}s\t\n", timeSpanThreadPool.Seconds, timeSpanThreadPool.Milliseconds / 10));
            }

            
            // print the averages also
            Console.WriteLine("\n________________________________________________________________________________");
            Console.Write("Avg.\t");

            TimeSpan timeSpanSequentialAverage = CalculateAverageTimeSpan(timeSpanSequentials);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanSequentialAverage.Seconds, timeSpanSequentialAverage.Milliseconds / 10));

            TimeSpan timeSpanParallelForeachAverage = CalculateAverageTimeSpan(timeSpanParallelForEachs);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanParallelForeachAverage.Seconds, timeSpanParallelForeachAverage.Milliseconds / 10));

            TimeSpan timeSpanTPLAverage = CalculateAverageTimeSpan(timeSpanTPLs);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanTPLAverage.Seconds, timeSpanTPLAverage.Milliseconds / 10));

            TimeSpan timeSpanThreadPoolAvarage = CalculateAverageTimeSpan(timeSpanThreadPools);
            Console.Write(String.Format("{0:00}.{1:00}s\t", timeSpanThreadPoolAvarage.Seconds, timeSpanThreadPoolAvarage.Milliseconds / 10));

            Console.WriteLine("\n________________________________________________________________________________");
            Console.ReadLine();
        }

        static TimeSpan MeasureSequential(IEnumerable<string> feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var f in feedSources.Select(feedSource => new FeedParser(feedSource))) { }

            stopwatch.Stop();
            return stopwatch.Elapsed;            
        }

        static TimeSpan MeasureParallelForeach(IEnumerable<string> feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.ForEach(feedSources, feedSource =>
                                              {
                                                  var f = new FeedParser(feedSource);
                                              }
            );

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        static TimeSpan MeasureTPL(IList<string> feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tasks = new Task[feedSources.Count];
            for (int i = 0; i < feedSources.Count;i++ )
            {
                string source = feedSources[i]; /* work-around modified closures */
                tasks[i] = Task.Factory.StartNew(() => TPLFeedParserTask(source));
            }

            Task.WaitAll(tasks);

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        static void TPLFeedParserTask(string feedSource)
        {
            var f = new FeedParser(feedSource);
        }

        static TimeSpan MeasureNativeThreadPool(string[] feedSources)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var threadPoolDoneEvents = new ManualResetEvent[feedSources.Length];

            for (int i = 0; i < feedSources.Length; i++)
            {
                threadPoolDoneEvents[i] = new ManualResetEvent(false);
                var wrapper = new ThreadPoolDataWrapper(threadPoolDoneEvents[i], feedSources[i]);
                ThreadPool.QueueUserWorkItem(ThreadPoolTask, wrapper);
            }

            WaitHandle.WaitAll(threadPoolDoneEvents);

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        static void ThreadPoolTask(object data)
        {
            var f = new FeedParser(((ThreadPoolDataWrapper) data).FeedSource);
            (data as ThreadPoolDataWrapper).ResetEvent.Set();
        }

        static TimeSpan CalculateAverageTimeSpan(TimeSpan[] timeSpans)
        {
            double miliseconds = timeSpans.Sum(t => t.TotalMilliseconds) / timeSpans.Length;
            return TimeSpan.FromMilliseconds(miliseconds);
        }

        static void PrintCPUInfo()
        {
            /* code taken from: http://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c/2670568#2670568 */

            int physicalCpuCount = 0;
            int coreCount = 0;
            bool coreCountSupported;
            int logicalCpuCount = 0;
            bool logicalCpuCountSupported;

            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                physicalCpuCount = Int32.Parse(item["NumberOfProcessors"].ToString());
            }

            try
            {
                foreach (var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
                }
                coreCountSupported=true;
            }
            catch(Exception) { coreCountSupported = false; }

            try
            {
                foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
                {
                    logicalCpuCount = Int32.Parse(item["NumberOfLogicalProcessors"].ToString());
                }
                logicalCpuCountSupported = true;
            }
            catch(Exception) { logicalCpuCountSupported = false; }

            Console.WriteLine("Test Environment: {0} physical cpus, {1} cores, {2} logical cpus.", physicalCpuCount, coreCountSupported ? coreCount.ToString() : "NotSupported", logicalCpuCountSupported ? logicalCpuCount.ToString() : "NotSupported");
        }
    }

    class ThreadPoolDataWrapper
    {
        public ManualResetEvent ResetEvent { get; set; }
        public string FeedSource { get; private set; }

        public ThreadPoolDataWrapper(ManualResetEvent @event, string feedSource)
        {
            this.ResetEvent = @event;
            this.FeedSource = feedSource;
        }
    }
}
