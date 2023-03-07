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
                                                float tol) {

            // TODO: implement gauss seidel
            return 0.003f;
        }
    }
}