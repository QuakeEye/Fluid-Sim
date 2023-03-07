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

            renderTarget = target;

            // Save the world size and set up the integer array
            simSpaceSize = spaceSize;
            simSpace = new int[simSpaceSize.X, simSpaceSize.Y];
            simSpace = Utils.Populate2DArray(simSpace, 0);

            // Set up the world for fluid values
            velField = new Vector2[simSpaceSize.X, simSpaceSize.Y];
            densField = new float[simSpaceSize.X, simSpaceSize.Y];
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
            //Advect();

            // Execute part 3
            //ClearDivergence();
        }


        /// <summary>
        /// This function will diffuse the worlds
        ///  spreading cells values to the surrounding area
        //  Actual algorithm info in the function
        /// </summary>
        void Diffuse() {

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


            // Gauss seidel...
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
                            renderTarget.Plot(  x + i, 
                                                y + j, 
                                                simSpace[x / widthScaler, y / heightScaler]);
                        }
                    }
                }
            }
        }
    }
}