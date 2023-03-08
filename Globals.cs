namespace Fluid_Sim {

    public static class Globals {

        // Globals data members
        public static Random random;


        // Constant members
        public const float gsIters = 20;
        public const float k = 0.001f;


        // Simulation data members
        public static bool isRunning = false;
        public static bool isSlowed = false;

        public const float slowDownValue = 1000000f;
        public const float dt = 0.1f;


        // Input data members
        public const int mouseInputLeftSize = 3;
        public const int mouseInputRightSize = 3;
        public const float mouseDensitySetValue = 1;
    }
}