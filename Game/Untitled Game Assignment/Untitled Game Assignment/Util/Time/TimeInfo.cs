using Microsoft.Xna.Framework;
using System;

namespace Util.FrameTimeInfo
{
    /// <summary>
    /// time class managing time steps and so on 
    /// </summary>
    public static class TimeInfo
    {
        /// <summary>
        /// the time step from frame to frame
        /// </summary>
        public static GameTime timeStep;
        /// <summary>
        /// the time step from frame to frame
        /// </summary>
        internal static GameTime TimeStep
        {
            private get => timeStep;
            set
            {
                timeStep = value;
                CalcFixedUpdateDelta();
            }
        }

        /// <summary>
        /// scales the time (0 pauses game)
        /// </summary>
        public static float TimeScale { get; set; }

        /// <summary>
        /// scaled delta time between update
        /// </summary>
        public static float DeltaTime { get { return UnscaledDeltaTime * TimeScale; } }
        /// <summary>
        /// delta time between frames unscaled
        /// </summary>
        public static float UnscaledDeltaTime { get => (float)TimeStep.ElapsedGameTime.TotalSeconds; }

        /// <summary>
        /// fixed update rate delta time scaled
        /// </summary>
        public static float FixedDeltaTime { get => UnscaledFixedDeltaTime * TimeScale; }

        /// <summary>
        /// fixed update rate delta time 
        /// </summary>
        public static float UnscaledFixedDeltaTime => FixedUpdateRate;

        /// <summary>
        /// the update rate for updates happening at fixed time intervals
        /// </summary>
        public static float FixedUpdateRate { get; set; }

        /// <summary>
        /// the number of fixed updates this update cycle
        /// </summary>
        internal static int NumberFixedUpdates { get; private set; }

        /// <summary>
        /// the remainder between fixed updates carreied to next frame
        /// </summary>
        private static float fixedUpdateTimeCarray;

        /// <summary>
        /// calculates the fixed update rate
        /// </summary>
        private static void CalcFixedUpdateDelta()
        {
            float t = fixedUpdateTimeCarray + UnscaledDeltaTime;

            NumberFixedUpdates = (int)Math.Floor(t / FixedUpdateRate);

            fixedUpdateTimeCarray = t - (NumberFixedUpdates * FixedUpdateRate);

            //UnscaledFixedDeltaTime = FixedUpdateRate;
        }

        internal static void Initialize(float timeScale, float fixedUpdateRate)
        {
            TimeScale = timeScale;
            FixedUpdateRate = fixedUpdateRate;
            fixedUpdateTimeCarray = 0f;
        }

    }
}
