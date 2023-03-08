using OpenTK.Mathematics;

namespace Fluid_Sim {

    public static class Solvers {

        /// <summary>
        /// This function is a solver that will converge a system of equations
        ///  using the Gauss-Seidel method
        /// </summary>
        public static float GaussSeidelSolve(   Vector2i currCell, 
                                                float[,] valField, 
                                                Vector2i simSize, 
                                                float iterations) {

            // First, we need to iterate 'iterations' amount of times
            for(int iter = 0; iter < iterations; iter++) {

                
            }

            return 0.005f;
        }
    }
}