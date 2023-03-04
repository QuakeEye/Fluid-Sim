using OpenTK.Mathematics;

namespace Fluid_Sim {

    public static class Utils {

        // Convert a vector3 into integer colour, to use for surfaces
        public static Func<Vector3, int> mixColour = 
            (Vector3 c) => ((int)c.X<<16) + ((int)c.Y<<8) + (int)c.Z;


        /// <summary>
        /// This function is here to test the surface class
        /// It will generate a random colour and plot it on a random
        ///  position on the target surface
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


        /// <summary>
        /// Overload for the function which does the exact same as above,
        ///  but for a 2dimensional array
        /// </summary>
        public static void SetRandomColour(int[,] arr) {

            // Assign a random pixel in array with a random colour
            int x = Globals.random.Next(0, arr.GetLength(0));
            int y = Globals.random.Next(0, arr.GetLength(1));

            Vector3 randomColour = new Vector3(
                (float) Globals.random.NextDouble() * 255f,
                (float) Globals.random.NextDouble() * 255f,
                (float) Globals.random.NextDouble() * 255f
            );

            arr[x, y] = mixColour(randomColour);
        }


        /// <summary>
        /// Helper function to populate a 2d array with some value
        /// <summary>
        public static T[,] Populate2DArray<T>(T[,] array, T value) {

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i, j] = value;

            return array;
        }
    }
}