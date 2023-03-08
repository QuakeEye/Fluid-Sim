using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Fluid_Sim {
    public static class InputHelper {

        // Save previous and current keyboard states to be able to observe their differences
        public static KeyboardState PreviousKeyboardState;  // Previous keyboard state gets updated after it has been used

        public static KeyboardState CurrentKeyboardState;   // Current keyboard state gets updated before it is used


        // Save previous and current mouse states to be able to observe their differences
        public static MouseState PreviousMouseState;        // Previous mouse state gets updated after it has been used

        public static MouseState CurrentMouseState;         // Current mouse state gets updated before it is used


        public static Vector2 MouseDelta
            => new Vector2(CurrentMouseState.X, CurrentMouseState.Y) - new Vector2(PreviousMouseState.X, PreviousMouseState.Y);


        // Mouse positions
        public static Vector2 LastMousePos = Vector2.Zero;
        public static Vector2 CurrentMousePos = Vector2.Zero;
        public static Vector2 MouseDeltaPos
            => CurrentMousePos - LastMousePos;


        /// <summary>
        /// Method to update the keyboard states which needs to be updated every frame (Called in tick function so that is correct)
        ///  to properly function
        /// </summary>
        /// <param name="keyboardState">Give the currently retrieved keyboard state</param>
        public static void Update(KeyboardState keyboardState, MouseState mouseState) {

            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = keyboardState;

            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = mouseState;

            LastMousePos = CurrentMousePos;
            CurrentMousePos = CurrentMouseState.Position;

            // Write for debugging purposes, should be removed (TODO:)
            //Console.WriteLine($"Mouse delta is {MouseDeltaPos}: {CurrentMousePos} - {LastMousePos}");
        }


        /// <summary>
        /// Method to retrieve whether a button has been pressed, that means it returns true if it was being pressed in the last frame and
        ///  isn't pressed anymore
        /// </summary>
        /// <param name="button">The button that needs to be checked</param>
        public static bool ButtonPressed(Keys button) {

            return PreviousKeyboardState.IsKeyDown(button) && !CurrentKeyboardState.IsKeyDown(button);
        }
    }
}
