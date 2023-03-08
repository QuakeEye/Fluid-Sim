using OpenTK.Mathematics;

namespace Fluid_Sim {

    public class Sim {

        // Data members
        Surface renderTarget;   // Surface to which rendered pixels should be drawn, they will be
                                //  displayed by the template
        int[,] simSpace;        // 2 dimensional array which represents our fluid simulation world


        // Fluid value grids
        Vector2[,] velField;    // Velocity vector field
        float[,] densField;     // Density field


        Vector2i simSpaceSize;  // Defined size of the simulation world
        
        
        // Constructor that will set up our world
        public Sim(Surface target, Vector2i spaceSize) {

            // Set up the render target, this represents the actual screen
            renderTarget = target;


            // Save the world size and set up the integer array
            simSpaceSize = spaceSize;
            simSpace = new int[simSpaceSize.X, simSpaceSize.Y];
            simSpace = Utils.Populate2DArray(simSpace, 0);


            // Set up the world for fluid values
            velField = new Vector2[simSpaceSize.X, simSpaceSize.Y];
            densField = new float[simSpaceSize.X, simSpaceSize.Y];
            // initialise boundaries here


            // Fill these fields with initial values
            Utils.Populate2DArray(velField, new Vector2(0f, 0f));
            Utils.Populate2DArray(densField, 1f);
            // this still needs to be (properly) done
            // boundaries should also be set here, and should be initialised above
        }


        // Function that will move a time step through the simulation, 
        //  should be called every frame
        public void Update() {

            // Updating is made up of 3 main parts in this basic fluid sim:
            //  1. Diffuse the world, aka spread values through the world
            //  2. Advect the world, aka move values through the world
            //  3. Clear divergence, remove divergence from the world, as we are not god

            // Exexcute part 1
            Diffuse();

            // Execute part 2
            Advect();

            // Execute part 3
            ClearDivergence();


            // For debugging purposes, print some position in the field's density
            //  this should be removed later
            Console.WriteLine(densField[10, 10]);
        }


        /// <summary>
        /// This function will diffuse the worlds
        ///  spreading cells values to the surrounding area
        //  Actual algorithm info in the function
        /// </summary>
        private void Diffuse() {

            // Part 1: Diffusion
            // Even though velocities are the most important bit in fluid simulation,
            //  the first step, diffusion, is independent of the velocity field,
            //  as we are simply diffusing values throughout the world space
            // Simply said: diffusion is about attributes in a cell in the 
            //  fluid spreading to the surrounding area
            // Our goal is to have cells have values that gradually become the
            //  average of it and its surrounding cells

            // First, let's work on the diffusion of density
            //  we need to define some conceptual things:
            //  - d(x, y) = density at cell (x, y)
            //  - s(x, y) = average density of surrounding cells (just the surrounding cells!)
            //      --> (d(x + 1, y) + d(x - 1, y) + d(x, y + 1) + d(x, y - 1))  /  4
            //  - k = diffusion constant, how fast the diffusion should occur
            //      aka, k is the amount of change we will have in this diffusion per timestep
            
            // So, to reiterate with these definitions, we want d to gradually
            //  become s
            // dn = dc + k * (sc - dc) would be an approach, but this will
            //  will result into a linear interpolation approach, which is not ideal, as
            //  we could easily overshoot the s value, which would have unstable results
            //  in the simulation

            // To solve this, we will swap the next and current values to trace back, then
            //  rewrite this to obtain the following equation
            // dn = (dc + k.sn) / (1 + k)
            // This is a hyperbolic relation, instead of a linear one, so we will never overshoot
            //  and rather converge to our value

            // The only problem with this solution, is that we need the surrouding average density
            //  of the next time step, which we don't have yet
            // We can fill in what that sn really is, and we will see a system of equatins appear
            // We will solve this system by using the Gauss Seidel method (more info in notion)


            // The Gauss-Seidel method basically uses random values, and keeps pluggin them into
            //  the system of equations, until it converges to a solution
            // A requirement for this method is that the system of equations is diagonally dominant,
            //  but in this case that is *always* the case, info in notion

            // So, using the Gauss-Seidel method we will converge the sn (next step surrounding density)
            //  to the correct value, and then we can use that to calculate the next step density

            for (int x = 0; x < simSpaceSize.X; x++) {
                for (int y = 0; y < simSpaceSize.Y; y++) {

                    // First, set up the values needed for the Gauss-Seidel method
                    // Density field is densField
                    Vector2i currentCell = new Vector2i(x, y);

                    float surrNext = Solvers.GaussSeidelSolve(  currentCell, 
                                                                densField, 
                                                                simSpaceSize, 
                                                                Globals.gsIters);

                    // After this, we can use this converged value to calculate the next density using
                    //  the equation above
                    float densNext = (densField[x, y] + Globals.k * surrNext) / (1 + Globals.k);

                    // And finally, we can set the next density to this newly calculated density
                    densField[x, y] = densNext;
                }
            }
        }


        private void Advect() {

            
        }


        private void ClearDivergence() {


        }


        // This function will render the simulation world onto the render target
        //  in a scalable manner
        public void Render() {

            // Big warning: widths and heights of render target should be some multiplication
            //  of the world size, as information might be lost if this is not the case

            // Slight to-do: this scaling code should propably occur once, and on window resize

            // This scaling should be a multiple idea is enhanced by the fact that the scales
            //  are stored as integers, so watch out!
            int widthScaler = renderTarget.Width / simSpaceSize.X;
            int heightScaler = renderTarget.Height / simSpaceSize.Y;

            // Render out the world into the target in *some* way
            for (int x = 0; x < simSpaceSize.X * widthScaler; x += widthScaler) {
                for (int y = 0; y < simSpaceSize.Y * heightScaler; y += heightScaler) {

                    // Fill out this kind of box in the render target that is the world scale size
                    for (int i = 0; i < widthScaler; i++) {
                        for (int j = 0; j < heightScaler; j++) {

                            // For now, just plot this arbitrary pixel colour directly
                            /*renderTarget.Plot(  x + i, 
                                                y + j, 
                                                simSpace[x / widthScaler, y / heightScaler]);*/

                            // Let's draw the density field out to the screen for visual purposes
                            renderTarget.Plot(  x + i, 
                                                y + j, 
                                                Utils.mixColour(
                                                    new Vector3(
                                                        densField[x / widthScaler, y / heightScaler] * 255f
                                                )));
                        }
                    }
                }
            }
        }
    }
}