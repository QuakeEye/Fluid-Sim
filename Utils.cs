using OpenTK.Mathematics;

namespace Fluid_Sim {

    public static class Utils {

        // Convert a vector3 into integer colour, to use for surfaces
        public static Func<Vector3, int> MixColour = 
            (Vector3 c) => ((int)c.X<<16) + ((int)c.Y<<8) + (int)c.Z;


        // Generate a float between a min and a max value
        public static Func<float, float, float> RandomFloat =
            (float min, float max) => (float) Globals.random.NextDouble() * (max - min) + min;


        // Linearly interpolate as discussed in the referenced paper
        public static Func<float, float, float, float> Lerp =
            (float a, float b, float k) => a + k * (b - a);


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

            surf.Plot(x, y, MixColour(randomColour));
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

            arr[x, y] = MixColour(randomColour);
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


        /// <summary>
        /// Helper function to populate a 2d array with random values
        /// The only values that are supported lie between the range 0 and max
        /// This function only supports float types
        /// <summary>
        public static float[,] Populate2DArrayRandom(float[,] array, float min, float max) {

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i, j] = RandomFloat(min, max);

            return array;
        }


        /// <summary>
        /// Helper function to populate a 2d array with random vector 2 values
        /// <summary>
        public static Vector2[,] Populate2DVector2Array(Vector2[,] array, float min, float max) {

            for (int i = 0; i < array.GetLength(0); i++)
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i, j] = new Vector2(
                        RandomFloat(min, max), RandomFloat(min, max)
                    );

            return array;
        }


        /// <summary>
        /// Function that will fill a set of coordinates in a 2d array with a value
        /// <summary>
        public static void BoxFillArray<T>(ref T[,] array, Vector2i start, Vector2i end, T value) {

            for (int i = start.X; i < end.X; i++)
                for (int j = start.Y; j < end.Y; j++)
                    array[i, j] = value;
        }


        /// <summary>
        /// Overload function that will instead assign random values within min and max
        /// Only supports vector 2 values
        /// </summary>
        public static void BoxFillArray(ref Vector2[,] array, Vector2i start, Vector2i end, 
                                        Vector2 value, float min, float max,
                                        bool randomFill = false, float randomFillChance = 0.9f) {

            if(randomFill) {

                float randomFillDecision = RandomFloat(0f, 1f);

                if(randomFillDecision > randomFillChance)
                    return;
            }

            for(int i = start.X; i < end.X; i++)
                for(int j = start.Y; j < end.Y; j++)
                    array[i, j] = value + new Vector2(RandomFloat(min, max), RandomFloat(min, max));
        }
    }
}