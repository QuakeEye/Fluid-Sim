using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fluid_Sim {

    public static class Globals {

        // Globals data members
        public static Random random;

        
        // Render settings
        public static Vector3 renderColour = new Vector3(212f, 241f, 249f);


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

        public const Keys velFieldReset = Keys.Q;
        public const Keys densFieldReset = Keys.W;
    }
}