using System;

namespace VoxeliqEngine.Universe
{
    /// <summary>
    /// Processes universe time.
    /// </summary>
    public static class Time
    {
        private const float RealTimeDivisor = 24*60*60;
        private const float GameHourInRealMinutes = (float) 2/60;
        private const float GameHourInRealSeconds = GameHourInRealMinutes*60;

        public static float GetRealTimeOfDay()
        {
            return (float) (DateTime.Now.TimeOfDay.TotalSeconds*24)/RealTimeDivisor;
        }

        public static float GetGameTimeOfDay()
        {
            //return (float)((DateTime.Now.TimeOfDay.TotalSeconds / GameHourInRealSeconds) % 24);
            return 12;
        }
    }
}