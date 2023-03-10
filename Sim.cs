using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
            //Utils.Populate2DVector2Array(velField, .6f, .7f);
            /*Utils.BoxFillArray( ref velField, new Vector2i(1), 
                                new Vector2i(simSpaceSize.X / 2, simSpaceSize.Y / 2),
                                new Vector2(0.5f, -0.5f), -0.5f, 0.5f, true);
            Utils.BoxFillArray( ref velField, new Vector2i(simSpaceSize.X / 2, 1), 
                                new Vector2i(simSpaceSize.X - 1, simSpaceSize.Y - 1),
                                new Vector2(0.5f, 0.5f), -0.5f, 0.5f, true);
            Utils.BoxFillArray( ref velField, new Vector2i(1, simSpaceSize.Y / 2), 
                                new Vector2i(simSpaceSize.X / 2, simSpaceSize.Y - 1),
                                new Vector2(-0.5f, -0.5f), -0.5f, 0.5f, true);
            Utils.BoxFillArray( ref velField, new Vector2i(simSpaceSize.X / 2, simSpaceSize.Y / 2), 
                                new Vector2i(simSpaceSize.X - 1, simSpaceSize.Y - 1),
                                new Vector2(-0.5f, 0.5f), -0.5f, 0.5f, true);
            
            Utils.Populate2DArrayRandom(densField, 0.2f, 1f);*/
            // this still needs to be (properly) done
            // boundaries should also be set here, and should be initialised above
        }


        // Function that will move a time step through the simulation, 
        //  should be called every frame
        public void Update() {

            // Before doing anything, check input to manage the interactive
            //  part of this fluid simulation
            HandleInput();

            // Make sure not to update when the simulation is paused
            if (!Globals.isRunning)
                return;

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
            //Console.WriteLine(densField[10, 10]);
        }


        /// <summary>
        /// This function will handle the input, so the world can be interacted
        ///  with at runtime
        /// </summary>
        void HandleInput() {

            // Check to see if spacebar is pressed so we can continue simulation
            if(InputHelper.CurrentKeyboardState.IsKeyPressed(Keys.Space))
                Globals.isRunning = !Globals.isRunning;

            // Check to see if enter is pressed so we can slow or unslow the simulation
            if(InputHelper.CurrentKeyboardState.IsKeyPressed(Keys.Enter))
                Globals.isSlowed = !Globals.isSlowed;


            // Reset velocity field when pressing some defined button
            if(InputHelper.CurrentKeyboardState.IsKeyPressed(Globals.velFieldReset))
                velField = new Vector2[simSpaceSize.X, simSpaceSize.Y];

            // Reset density field when pressing some defined button
            if(InputHelper.CurrentKeyboardState.IsKeyPressed(Globals.densFieldReset))
                densField = new float[simSpaceSize.X, simSpaceSize.Y];


            // If the mouse butting isn't currently being pressed, we should skip this step
            if(InputHelper.CurrentMouseState.IsButtonDown(MouseButton.Left)) {

                // To make our simulation interactive, we first need to translate the mouse
                //  position on screen to mouse position in our world
                // We can do this using a similar scaling method like we do in rendering
                Vector2 mousePos = InputHelper.CurrentMouseState.Position;
                mousePos.X = mousePos.X / renderTarget.Width * simSpaceSize.X;
                mousePos.Y = mousePos.Y / renderTarget.Height * simSpaceSize.Y;

                // Now, determine what direction we want to set the velocities
                Vector2 dirSet = InputHelper.MouseDeltaPos;

                // Set the velocity at the mouse position to the direction we want
                if(mousePos.X >= 0 && mousePos.X < simSpaceSize.X &&
                mousePos.Y >= 0 && mousePos.Y < simSpaceSize.Y) {
                    
                    int minY = Math.Max((int) mousePos.Y - Globals.mouseInputLeftSize, 1);
                    int minX = Math.Max((int) mousePos.X - Globals.mouseInputLeftSize, 1);

                    int maxY = Math.Min((int) mousePos.Y + Globals.mouseInputLeftSize, 
                                        simSpaceSize.Y - 1);
                    int maxX = Math.Min((int) mousePos.X + Globals.mouseInputLeftSize, 
                                        simSpaceSize.X - 1);

                    for (int y = minY; y <= maxY; y++) {
                        for (int x = minX; x <= maxX; x++) {
                            velField[x, y] = dirSet;
                        }
                    }
                }
            }


            // Next, I would like to be able to set the density at the mouse position
            if(InputHelper.CurrentMouseState.IsButtonDown(MouseButton.Right)) {

                // First, simply get the mouse position
                Vector2 mousePos = InputHelper.CurrentMouseState.Position;
                
                // and scale it from screen to world
                mousePos.X = mousePos.X / renderTarget.Width * simSpaceSize.X;
                mousePos.Y = mousePos.Y / renderTarget.Height * simSpaceSize.Y;

                // Now, just set density surrounding the mouse
                if(mousePos.X >= 0 && mousePos.X < simSpaceSize.X &&
                mousePos.Y >= 0 && mousePos.Y < simSpaceSize.Y) {
                    
                    int minY = Math.Max((int) mousePos.Y - Globals.mouseInputRightSize, 1);
                    int minX = Math.Max((int) mousePos.X - Globals.mouseInputRightSize, 1);

                    int maxY = Math.Min((int) mousePos.Y + Globals.mouseInputRightSize, 
                                        simSpaceSize.Y - 1);
                    int maxX = Math.Min((int) mousePos.X + Globals.mouseInputRightSize, 
                                        simSpaceSize.X - 1);

                    for (int y = minY; y <= maxY; y++)
                        for (int x = minX; x <= maxX; x++)
                            densField[x, y] = Globals.mouseDensitySetValue;
                }
            }
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


            // There shouldn't be a loop here anymore, it is now included in the solver
            densField = Solvers.GaussSeidelSolveDiffuse(densField, simSpaceSize, Globals.gsIters);

            /*
            for (int x = 0; x < simSpaceSize.X; x++) {
                for (int y = 0; y < simSpaceSize.Y; y++) {

                    // First, set up the values needed for the Gauss-Seidel method
                    // Density field is densField
                    Vector2i currentCell = new Vector2i(x, y);

                    float surrNext = Solvers.GaussSeidelSolve(  densField, 
                                                                simSpaceSize, 
                                                                Globals.gsIters);

                    // This code below now seems to be included in the gauss seidel solver

                    // After this, we can use this converged value to calculate the next density using
                    //  the equation above
                    //float densNext = (densField[x, y] + Globals.k * surrNext) / (1 + Globals.k);

                    // And finally, we can set the next density to this newly calculated density
                    //densField[x, y] = densNext;
                }
            }*/
        }


        private void Advect() {

            // For advection, it is impossible to calculate forwards in time
            //  because vectors almost never exactly line up with another square's center,
            //  so we would have to interpolate between 4 squares, which is kinda difficult
            // So, what we do once again, is trace back in time, and interpolate between the 4 squares
            //  that the vector would have been pointing to at that time

            // This is an algorithm we have to perform for each position

            for (int x = 1; x < simSpaceSize.X - 1; x++) {
                for (int y = 1; y < simSpaceSize.Y - 1; y++) {

                    // First, get the floating point value of the position 
                    //  we want to trace back to
                    Vector2 traceBackPos = new Vector2(x, y) - velField[x, y];

                    // We now need to get the position of the cell and the position within the 4 cells
                    // We do this by splitting the value in a floor and a decimal part
                    Vector2 floorPos = new Vector2((int) traceBackPos.X, (int) traceBackPos.Y);
                    Vector2 decimalPos = traceBackPos - floorPos;

                    // First, before doing anything, we should check if these are within
                    //  the bounds of the world
                    if( floorPos.X < 1 || floorPos.X >= simSpaceSize.X - 1 || 
                        floorPos.Y < 1 || floorPos.Y >= simSpaceSize.Y - 1)
                        continue;

                    // Now, we need to interpolate the density values of these 4 squares
                    // We do this by first interpolating the 2 top squares and the 2 bottom squares
                    //  then interpolating between these 2 values using the y-coordinate
                    float topInter = Utils.Lerp(    densField[(int) floorPos.X, (int) floorPos.Y], 
                                                    densField[(int) floorPos.X + 1, (int) floorPos.Y], 
                                                    decimalPos.X);
                    float bottomInter = Utils.Lerp( densField[(int) floorPos.X, (int) floorPos.Y + 1], 
                                                    densField[(int) floorPos.X + 1, (int) floorPos.Y + 1], 
                                                    decimalPos.X);

                    // Now, as promised, we will interpolate these using the y value :)
                    float newDensity = Utils.Lerp(topInter, bottomInter, decimalPos.Y);

                    // And finally, set this newly calculated density value in the density field
                    densField[x, y] = newDensity;
                }
            }
        }


        private void ClearDivergence() {

            // It is impossible to calculate a divergence-free velocity field
            //  at least, I am not aware of any method to do this with my
            //  limited research thus far
            
            // However, any velocity field is simply a sum of its divergence-free field
            //  and its curl-free field, curl is not something we want to remove, but it
            //  IS something we CAN remove, then we can subtract that from the original
            //  field and thus obtain a divergence-free field
            // (Velfield = Curlfree + Divergencefree) =>
            // Divergencefree = Velfield - Curlfree
            // This is known as the Holmheltz Decomposition
            //  and this theorem of addition is known as Holmheltz's Theorem

            // First, we need to calculate the divergence at each point in the field, which
            //  includes calculating difference in x and in y velocities
            //  which are then divided by their distance 2
            // Positive divergence means mass flowing out, negative divergence means mass 
            //  flowing in
            float[,] divergenceField = new float[simSpaceSize.X, simSpaceSize.Y];

            for (int x = 1; x < simSpaceSize.X - 1; x++) {
                for (int y = 1; y < simSpaceSize.Y - 1; y++) {

                    float xVelDiff = velField[x + 1, y].X - velField[x - 1, y].X;
                    float yVelDiff = velField[x, y + 1].Y - velField[x, y - 1].Y;
                    float divergence = (xVelDiff + yVelDiff) / 2;

                    divergenceField[x, y] = divergence;
                }
            }

            // Using gauss seidel, we will now approximate the project values
            //  at every point in the field
            float[,] projVals = Solvers.GaussSeidelSolveDivergence( divergenceField,
                                                                    simSpaceSize,
                                                                    Globals.gsIters);
            
            // Now that we have these values, we can construct a vector field that will
            //  be free of curl
            Vector2[,] curlFreeField = new Vector2[simSpaceSize.X, simSpaceSize.Y];

            for(int x = 1; x < simSpaceSize.X - 1; x++) {
                for(int y = 1; y < simSpaceSize.Y - 1; y++) {

                    float xComponent = (projVals[x + 1, y] - projVals[x - 1, y]) / 2;
                    float yComponent = (projVals[x, y + 1] - projVals[x, y - 1]) / 2;
                    Vector2 curlFreeVec = new Vector2(xComponent, yComponent);

                    curlFreeField[x, y] = curlFreeVec;
                }
            }

            // Now we need to subtract this curl-free field from the actual field to
            //  obtain a divergence-free field
            for(int x = 1; x < simSpaceSize.X - 1; x++) {
                for(int y = 1; y < simSpaceSize.Y - 1; y++) {

                    velField[x, y] -= curlFreeField[x, y];
                }
            }
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

                            float colourComponent = 
                                densField[x / widthScaler, y / heightScaler] * 255f;

                            // Let's draw the density field out to the screen for visual purposes
                            renderTarget.Plot(  x + i, 
                                                y + j, 
                                                Utils.MixColour(
                                                    colourComponent * 
                                                        (Globals.renderColour / 255f)
                                                ));
                        }
                    }
                }
            }
        }
    }
}