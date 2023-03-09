using OpenTK.Mathematics;

namespace Fluid_Sim {

    public static class Solvers {

        // TODO: Maybe the Gauss-Seidel solvers should be generalised and combined

        /// <summary>
        /// This function is a solver that will converge a system of equations
        ///  using the Gauss-Seidel method
        /// This function is catered towards solving diffusion
        /// </summary>
        public static float[,] GaussSeidelSolveDiffuse( float[,] currValField, 
                                                        Vector2i simSize, 
                                                        float iterations) {

            float a = Time.DeltaTime * Globals.k;
            
            float[,] nextValField = currValField;
            
            //float[,] nextValField = new float[simSize.X, simSize.Y];
            //Utils.Populate2DArray(nextValField, 0.0f);

            // When the simulation is slowed, manually lower the a value so you can
            //  more easily see the algorithm at work
            if(Globals.isSlowed)
                a /= Globals.slowDownValue;

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


        /// <summary>
        /// This function is a solver that will converge a system of equations
        ///  using the Gauss-Seidel method
        /// This function is catered towards clearing divergence
        /// </summary>
        public static float[,] GaussSeidelSolveDivergence(  float[,] divField,
                                                            Vector2i simSize,
                                                            float iterations) {

            float[,] projectField = new float[simSize.X, simSize.Y];

            // First, we need to iterate 'iterations' amount of times
            for (int k = 0; k < iterations; k++) {
                for (int x = 1; x < simSize.X - 1; x++) {
                    for (int y = 1; y < simSize.Y - 1; y++) {

                        // Now, we apply the equation for calculating these project values
                        //  in a gauss seidel approach
                        projectField[x, y] =
                            (projectField[x - 1, y] + projectField[x + 1, y] +
                            projectField[x, y - 1] + projectField[x, y + 1] -
                            divField[x, y]) / 4;
                    }
                }
            }

            return projectField;
        }
    }
}