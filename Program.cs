using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fluid_Sim {
    
    public class Program {

        static void Main(string[] args) {
            
            // Create a new window
            using (Window window = new Window(new Vector2i(800, 600)))
                window.Run();
        }
    }


    /// <summary>
    /// This class has been adapted from a template from the INFOGR course at UU
    /// </summary>
    public class Window : GameWindow {

        // Keep track of wanting to stop the program
        bool terminated = false;    // True if app is terminated ==> close app


        // Texture and ID for opengl per-pixel drawing
        Surface screen;
        int screenID;


        // OpenGL stuff (gotten from template)
        public int vertexArrayObject;
        public int vertexBufferObject;
        public int programID;
        // All the data for the vertices interleaved in one array:
        // - XYZ in normalized device coordinates
        // - UV
        readonly float[] vertices =
        { //  X      Y     Z     U     V
            -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, // bottom-left  2-----3 triangles:
             1.0f, -1.0f, 0.0f, 1.0f, 1.0f, // bottom-right | \   |     012
            -1.0f,  1.0f, 0.0f, 0.0f, 0.0f, // top-left     |   \ |     123
             1.0f,  1.0f, 0.0f, 1.0f, 0.0f, // top-right    0-----1
        };


        public Window(Vector2i size) 
            : base (GameWindowSettings.Default, new NativeWindowSettings() 
            {
                Size = size,
                Title = "Fluid Simulation",

                Profile = ContextProfile.Core,
                Flags = ContextFlags.ForwardCompatible
            }) { }

        
        protected override void OnLoad() {
            
            base.OnLoad();
            
            // Application being initialised
            GL.ClearColor(0, 0, 0, 0);
            GL.Disable(EnableCap.DepthTest);

            screen = new Surface(Size.X, Size.Y);
            
            LoadOpenGL(screen);
        }


        void LoadOpenGL(Surface screen) {

            #region OpenGL
            screenID = screen.GenTexture();
            // Setting up a Modern OpenGL pipeline takes a lot of code
            // Vertex Array Object: will store the meaning of the data in the buffer
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);


            // Vertex Buffer Object: a buffer of raw data
            vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);


            // Vertex Shader
            string shaderSource = File.ReadAllText("shaders/Screen/screen.vert");
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSource);
            GL.CompileShader(vertexShader);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vStatus);

            if (vStatus != (int)All.True) {

                string log = GL.GetShaderInfoLog(vertexShader);
                throw new Exception($"Error occurred whilst compiling vertex shader ({vertexShader}):\n{log}");
            }


            // Fragment Shader
            shaderSource = File.ReadAllText("shaders/Screen/screen.frag");
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fStatus);

            if (fStatus != (int)All.True) {

                string log = GL.GetShaderInfoLog(fragmentShader);
                throw new Exception($"Error occurred whilst compiling fragment shader ({fragmentShader}):\n{log}");
            }


            // Program: a set of shaders to be used together in a pipeline
            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShader);
            GL.AttachShader(programID, fragmentShader);
            GL.LinkProgram(programID);
            GL.GetProgram(programID, GetProgramParameterName.LinkStatus, out int pStatus);

            if (pStatus != (int)All.True) {

                string log = GL.GetProgramInfoLog(programID);
                throw new Exception($"Error occurred whilst linking program ({programID}):\n{log}");
            }

            // The program pipeline has been set up, we can now delete the shaders
            GL.DetachShader(programID, vertexShader);
            GL.DetachShader(programID, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // All the draw calls should now be directed through this pipeline
            GL.UseProgram(programID);

            // Tell the VAO which part of the VBO data should go to each shader input
            int vPosLocation = GL.GetAttribLocation(programID, "vPosition");
            GL.EnableVertexAttribArray(vPosLocation);
            GL.VertexAttribPointer(vPosLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int vUVlocation = GL.GetAttribLocation(programID, "vUV");
            GL.EnableVertexAttribArray(vUVlocation);
            GL.VertexAttribPointer(vUVlocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            // Connect the texture to the shader uniform variable
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, screenID);
            GL.Uniform1(GL.GetUniformLocation(programID, "pixels"), 0); // Retrieves a location in the fragment shader

            Console.WriteLine("OpenGL was initialised without errors");

            #endregion OpenGL


            // Lastly, set up time itself for the simulation of reality
            Time.Setup();
        }


        protected override void OnUnload() {

            base.OnUnload();

            // Called upon app close
            GL.DeleteTextures(1, ref screenID);

            // Close the program
            GL.DeleteProgram(programID);

            // Close the windows and terminate the app
            Environment.Exit(0);
        }


        protected override void OnResize(ResizeEventArgs e) {

            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);   // Set the viewport to the new window size
        }


        protected override void OnUpdateFrame(FrameEventArgs e) {

            base.OnUpdateFrame(e);

            // Quit the program once escape key has been pressed
            var keyboard = KeyboardState;
            if (keyboard[Keys.Escape]) terminated = true;

            SetRandomColour();

            Time.Update();
        }

        void SetRandomColour() {

            Func<Vector3, int> mixColour = (Vector3 c) => ((int)c.X<<16) + ((int)c.Y<<8) + (int)c.Z;

            // Assign a random pixel in surface with a random colour
            Random random = new Random();

            int x = random.Next(0, Size.X);
            int y = random.Next(0, Size.Y);

            Vector3 randomColour = new Vector3(
                (float) random.NextDouble() * 255f,
                (float) random.NextDouble() * 255f,
                (float) random.NextDouble() * 255f
            );

            screen.Plot(x, y, mixColour(randomColour));
            Console.WriteLine($"Plotted colour {randomColour} at X:{x} Y:{y}");
        }


        protected override void OnRenderFrame(FrameEventArgs e) {
            
            base.OnRenderFrame(e);

            if (terminated) { Close(); return; }
            
            // Convert MyApplication.screen to OpenGL texture
            GL.BindTexture(TextureTarget.Texture2D, screenID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                            screen.Width, screen.Height, 0,
                            PixelFormat.Bgra,
                            PixelType.UnsignedByte, screen.Pixels
                            );

            // Draw screen filling quad
            GL.BindVertexArray(vertexArrayObject);
            GL.UseProgram(programID);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            // Tell OpenTK we're done rendering
            SwapBuffers();
        }
    }
}