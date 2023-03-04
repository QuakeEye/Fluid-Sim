using System;
using OpenTK.Graphics.OpenGL;

namespace Fluid_Sim {

    /// <summary>
    /// Surface class that is used to 'draw' our picture onto and to be drawn using openGL
    /// It is basically just an array of pixels holding their own color
    ///
    /// This class has been adapted from a template from the INFOGR course at UU
    /// </summary>
    public class Surface {

        public int[] Pixels { get; set; } // for now using an int to represent the color of a pixel, might be better to change this later

        public int Width { get; private set; }

        public int Height { get; private set; }


        /// <summary>
        /// Constructor for the surface class
        /// </summary>
        /// <param name="width">Width of the surface</param>
        /// <param name="height">Height of the surface</param>
        public Surface(int width, int height) {
            
            Width = width;
            Height = height;
            Pixels = new int[width * height];
        }


        // clear the surface, and turn it to the given color
        public void Clear(int c) { for (int s = Width * Height, p = 0; p < s; p++) Pixels[p] = c; }


        // Plot out a single pixel to the surface
        public void Plot(int x, int y, int c) {

            if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height))
                Pixels[x + y * Width] = c;
        }


        // Plot out a single pixel to the surface, with a given index offset
        public void Plot(int x, int y, int c, int offset) {

            if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height))
                Pixels[x + y * Width + offset] = c;
        }


        public int GetPixel(int x, int y) {
            if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height))
                return Pixels[x + y * Width];
            else
                return 0;
        }


        // Print some pixel for debugging purposes
        public void Print(int x, int y) {
            
            if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height))
                Console.WriteLine(Pixels[x + y * Width]);
        }


        // create an OpenGL texture
        public int GenTexture() {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, Pixels);
            return id;
        }
    }
}