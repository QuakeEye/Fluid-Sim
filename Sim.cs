using OpenTK.Mathematics;

namespace Fluid_Sim {

    public class Sim {

        // Data members
        Surface renderTarget;   // Surface to which rendered pixels should be drawn, they will be
                                //  displayed by the template
        int[,] simSpace;        // 2 dimensional array which represents our fluid simulation world

        Vector2i simSpaceSize;  // Defined size of the simulation world
        
        
        // Constructor that will set up our world
        public Sim(Surface target, Vector2i spaceSize) {

            renderTarget = target;

            // Save the world size and set up the integer array
            simSpaceSize = spaceSize;
            simSpace = new int[simSpaceSize.X, simSpaceSize.Y];
            simSpace = Utils.Populate2DArray(simSpace, int.MaxValue);
        }


        // Function that will move a time step through the simulation, 
        //  should be called every frame
        public void Update() {

            Utils.SetRandomColour(simSpace);
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