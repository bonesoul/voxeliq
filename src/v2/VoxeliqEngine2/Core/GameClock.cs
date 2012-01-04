using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace VolumetricStudios.VoxeliqEngine.Core
{
    internal class GameClock
    {
        private long baseRealTime;
        private TimeSpan currentTimeBase;
        private TimeSpan currentTimeOffset;
        private TimeSpan elapsedAdjustedTime;
        private TimeSpan elapsedTime;
        private long lastRealTime;
        private bool lastRealTimeValid;
        private int suspendCount;
        private long suspendStartTime;
        private long timeLostToSuspension;

        internal static long Counter { get { return Stopwatch.GetTimestamp(); } }
        internal TimeSpan CurrentTime { get { return (this.currentTimeBase + this.currentTimeOffset); } }
        internal TimeSpan ElapsedAdjustedTime { get { return this.elapsedAdjustedTime; } }
        internal TimeSpan ElapsedTime { get { return this.elapsedTime; } }
        internal static long Frequency { get { return Stopwatch.Frequency; } }

        public GameClock()
        {
            this.Reset();
        }

        internal void Reset()
        {
            this.currentTimeBase = TimeSpan.Zero;
            this.currentTimeOffset = TimeSpan.Zero;
            this.baseRealTime = Counter;
            this.lastRealTimeValid = false;
        }

        internal void Resume()
        {
            this.suspendCount--;
            if (this.suspendCount <= 0)
            {
                long counter = Counter;
                this.timeLostToSuspension += counter - this.suspendStartTime;
                this.suspendStartTime = 0L;
            }
        }

        internal void Step()
        {
            long counter = Counter;
            if (!this.lastRealTimeValid)
            {
                this.lastRealTime = counter;
                this.lastRealTimeValid = true;
            }
            try
            {
                this.currentTimeOffset = CounterToTimeSpan(counter - this.baseRealTime);
            }
            catch (OverflowException)
            {
                this.currentTimeBase += this.currentTimeOffset;
                this.baseRealTime = this.lastRealTime;
                try
                {
                    this.currentTimeOffset = CounterToTimeSpan(counter - this.baseRealTime);
                }
                catch (OverflowException)
                {
                    this.baseRealTime = counter;
                    this.currentTimeOffset = TimeSpan.Zero;
                }
            }
            try
            {
                this.elapsedTime = CounterToTimeSpan(counter - this.lastRealTime);
            }
            catch (OverflowException)
            {
                this.elapsedTime = TimeSpan.Zero;
            }
            try
            {
                long num2 = this.lastRealTime + this.timeLostToSuspension;
                this.elapsedAdjustedTime = CounterToTimeSpan(counter - num2);
                this.timeLostToSuspension = 0L;
            }
            catch (OverflowException)
            {
                this.elapsedAdjustedTime = TimeSpan.Zero;
            }
            this.lastRealTime = counter;
        }

        internal void Suspend()
        {
            this.suspendCount++;
            if (this.suspendCount == 1)
            {
                this.suspendStartTime = Counter;
            }
        }

        static TimeSpan CounterToTimeSpan(long delta)
        {
            return TimeSpan.FromTicks((delta * 10000000) / Frequency);
        }
    }
}
