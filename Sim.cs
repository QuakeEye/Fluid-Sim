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
            simSpaceSize = spaceSize;
        }


        // Function that will move a time step through the simulation, 
        //  should be called every frame
        public void Update() {

            Utils.SetRandomColour(renderTarget);
        }


        // This function will render the simulation world onto the render target
        //  in a scalable manner
        public void Render() {

            // todo
        }
    }
}