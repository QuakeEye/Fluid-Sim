using System;
using System.Diagnostics;

namespace Fluid_Sim {

    public static class Time {

        public static float DeltaTime = 0;      // Delta time for amount of ms per frame
        public static float ElapsedTime = 0;    // Total elapsed time in ms

        static Stopwatch deltaCounter = new Stopwatch();    // Stopwatch that counts time


        /// <summary>
        /// Setup function that starts the stopwatch for the first time
        /// </summary>
        public static void Setup() {

            deltaCounter.Start();

            Console.WriteLine("Time has been set up");
        }


        /// <summary>
        /// Update Time function that stops stop watch, read and save its values and then starts the stopwatch for the next Tick
        /// </summary>
        public static void Update() {

            deltaCounter.Stop();
            
            DeltaTime = deltaCounter.ElapsedMilliseconds;
            ElapsedTime += DeltaTime;

            deltaCounter.Restart();
        }
    }
}