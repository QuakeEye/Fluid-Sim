using OpenTK.Mathematics;

namespace Fluid_Sim {

    public static class Utils {

        // Convert a vector3 into integer colour, to use for surfaces
        public static Func<Vector3, int> mixColour = 
            (Vector3 c) => ((int)c.X<<16) + ((int)c.Y<<8) + (int)c.Z;


        /// <summary>
        /// This function is here to test the surface class
        /// </summary>
        public static void SetRandomColour(Surface surf) {

            // Assign a random pixel in surface with a random colour
            int x = Globals.random.Next(0, surf.Width);
            int y = Globals.random.Next(0, surf.Height);

            Vector3 randomColour = new Vector3(
                (float) Globals.random.NextDouble() * 255f,
                (float) Globals.random.NextDouble() * 255f,
                (float) Globals.random.NextDouble() * 255f
            );

            surf.Plot(x, y, mixColour(randomColour));
        }
    }
}