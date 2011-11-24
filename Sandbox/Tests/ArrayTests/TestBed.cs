 /*
 * Hüseyin Uslu, shalafiraistlin@gmail.com
 * This code is provided as is.
 * Original code by: http://techcraft.codeplex.com/discussions/247791
 */

using System;
using System.Linq;
using System.Management;
using arraytests.Tests;

namespace arraytests
{
    public class TestBed
    {
        public TestBed() { }
       
        public void Run()
        {
            PrintCPUInfo();

            const int dimensionSize = 256; // the dimension size for arrays.
            const int iterationCount = 10; // how many iterations should we go for?

            // load the tests 
            var multidimensionalArray = new Multidimensional(dimensionSize);
            var jaggedArray = new Jagged(dimensionSize);
            var flattenArray = new Flatten(dimensionSize);

            // timespans for measuring averages
            var timeSpanMultiSequentals = new TimeSpan[iterationCount];
            var timeSpanJaggedSeqentals = new TimeSpan[iterationCount];
            var timeSpanFlattenSequentals = new TimeSpan[iterationCount];
            var timeSpanMultiRandoms = new TimeSpan[iterationCount];
            var timeSpanJaggedRandoms = new TimeSpan[iterationCount];
            var timeSpanFlattenRandoms = new TimeSpan[iterationCount];

            Console.WriteLine("Array size: {0}*{0}*{0}",dimensionSize);

            // test the sequental access
            Console.WriteLine("________________________________________________________________________________");
            Console.WriteLine("Itr.\tMulti.\tJagged\tFlatten\t(Sequental)");
            Console.WriteLine("________________________________________________________________________________");

            for (int i = 0; i < iterationCount; i++)
            {
                Console.Write("#{0}\t", i + 1);

                TimeSpan multidimensionalSequental = multidimensionalArray.AccessSequental();
                timeSpanMultiSequentals[i] = multidimensionalSequental;
                Console.Write(String.Format("{0:00}.{1:000}s\t", multidimensionalSequental.Seconds, multidimensionalSequental.Milliseconds));

                TimeSpan jaggedSequental = jaggedArray.AccessSequental();
                timeSpanJaggedSeqentals[i] = jaggedSequental;
                Console.Write(String.Format("{0:00}.{1:000}s\t", jaggedSequental.Seconds, jaggedSequental.Milliseconds));

                TimeSpan flattenSequental = flattenArray.AccessSequental();
                timeSpanFlattenSequentals[i] = flattenSequental;
                Console.Write(String.Format("{0:00}.{1:000}s\t", flattenSequental.Seconds, flattenSequental.Milliseconds));

                Console.WriteLine();
            }

            // averages for sequentals
            Console.Write("~Avg\t");

            TimeSpan multidimensionalSequentalAvg = CalculateAverageTimeSpan(timeSpanMultiSequentals);
            Console.Write(String.Format("{0:00}.{1:000}s\t", multidimensionalSequentalAvg.Seconds, multidimensionalSequentalAvg.Milliseconds));

            TimeSpan timeSpanJaggedSeqentalsAvg = CalculateAverageTimeSpan(timeSpanJaggedSeqentals);
            Console.Write(String.Format("{0:00}.{1:000}s\t", timeSpanJaggedSeqentalsAvg.Seconds, timeSpanJaggedSeqentalsAvg.Milliseconds));

            TimeSpan timeSpanFlattenSequentalsAvg = CalculateAverageTimeSpan(timeSpanFlattenSequentals);
            Console.Write(String.Format("{0:00}.{1:000}s\t", timeSpanFlattenSequentalsAvg.Seconds, timeSpanFlattenSequentalsAvg.Milliseconds));

            // test the random access

            Console.WriteLine("\n________________________________________________________________________________");
            Console.WriteLine("Itr.\tMulti.\tJagged\tFlatten\t(Random)");
            Console.WriteLine("________________________________________________________________________________");

            for (int i = 0; i < iterationCount; i++)
            {
                Console.Write("#{0}\t", i + 1);

                TimeSpan multidimensionalRandom = multidimensionalArray.AccessRandom();
                timeSpanMultiRandoms[i] = multidimensionalRandom;
                Console.Write(String.Format("{0:00}.{1:000}s\t", multidimensionalRandom.Seconds, multidimensionalRandom.Milliseconds));

                TimeSpan jaggedRandom = jaggedArray.AccessRandom();
                timeSpanJaggedRandoms[i] = jaggedRandom;
                Console.Write(String.Format("{0:00}.{1:000}s\t", jaggedRandom.Seconds, jaggedRandom.Milliseconds));

                TimeSpan flattenRandom = flattenArray.AccessRandom();
                timeSpanFlattenRandoms[i] = flattenRandom;
                Console.Write(String.Format("{0:00}.{1:000}s\t", flattenRandom.Seconds, flattenRandom.Milliseconds));

                Console.WriteLine();
            }

            // averages for sequentals
            Console.Write("~Avg\t");

            TimeSpan multidimensionalRandomAvg = CalculateAverageTimeSpan(timeSpanMultiRandoms);
            Console.Write(String.Format("{0:00}.{1:000}s\t", multidimensionalRandomAvg.Seconds, multidimensionalRandomAvg.Milliseconds));

            TimeSpan jaggedRandomAvg = CalculateAverageTimeSpan(timeSpanJaggedRandoms);
            Console.Write(String.Format("{0:00}.{1:000}s\t", jaggedRandomAvg.Seconds, jaggedRandomAvg.Milliseconds));

            TimeSpan flattenRandomAvg = CalculateAverageTimeSpan(timeSpanFlattenRandoms);
            Console.Write(String.Format("{0:00}.{1:000}s\t", flattenRandomAvg.Seconds, flattenRandomAvg.Milliseconds));
            
            Console.ReadLine();
        }

        static TimeSpan CalculateAverageTimeSpan(TimeSpan[] timeSpans)
        {
            double miliseconds = timeSpans.Sum(t => t.TotalMilliseconds) / timeSpans.Length;
            return TimeSpan.FromMilliseconds(miliseconds);
        }

        private void PrintCPUInfo()
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
                coreCountSupported = true;
            }
            catch (Exception) { coreCountSupported = false; }

            try
            {
                foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
                {
                    logicalCpuCount = Int32.Parse(item["NumberOfLogicalProcessors"].ToString());
                }
                logicalCpuCountSupported = true;
            }
            catch (Exception) { logicalCpuCountSupported = false; }

            Console.WriteLine("Test Environment: {0} physical cpus, {1} cores, {2} logical cpus.", physicalCpuCount, coreCountSupported ? coreCount.ToString() : "NotSupported", logicalCpuCountSupported ? logicalCpuCount.ToString() : "NotSupported");
            Console.WriteLine("________________________________________________________________________________");
        }
    }
}
