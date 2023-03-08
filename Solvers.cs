using OpenTK.Mathematics;

namespace Fluid_Sim {

    public static class Solvers {

        /// <summary>
        /// This function is a solver that will converge a system of equations
        ///  using the Gauss-Seidel method
        /// </summary>
        public static float[,] GaussSeidelSolve(float[,] currValField, 
                                                Vector2i simSize, 
                                                float iterations) {

            float a = Time.DeltaTime * Globals.k * simSize.X * simSize.Y;
            float[,] nextValField = currValField;

            // First, we need to iterate 'iterations' amount of times
            for(int iter = 0; iter < iterations; iter++) {
                for(int x = 1; x < simSize.X - 1; x++) {
                    for(int y = 1; y < simSize.Y - 1; y++) {

                        // This is the iterative function, we take some from the current field
                        //  but as you can see, we continuously expand iteratively on our next field
                        nextValField[x, y] = 
                            (currValField[x, y] + 
                                a * (nextValField[x - 1, y] + nextValField[x + 1, y] + 
                                    nextValField[x, y - 1] + nextValField[x, y + 1])) 
                            / (1 + 4 * a);
                    }
                }
            }

            return nextValField;
        }
    }
}