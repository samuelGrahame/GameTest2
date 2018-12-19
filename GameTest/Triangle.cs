/*
 * Acknowledgments
Check out the OneLoneCoder.com @Javidx9 - 3D Graphics Part for C++ and its creator, Javidx9, his website OneLoneCoder, and the OLC-3 License in the original project.

License
~~~~~~~
One Lone Coder Console Game Engine  Copyright (C) 2018  Javidx9
This program comes with ABSOLUTELY NO WARRANTY.
This is free software, and you are welcome to redistribute it
under certain conditions; See license for details.
Original works located at:
https://www.github.com/onelonecoder
https://www.onelonecoder.com
https://www.youtube.com/javidx9
GNU GPLv3
https://github.com/OneLoneCoder/videos/blob/master/LICENSE
*/

using PixelEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PixelEngine.Pixel;

namespace PixelEngine3D
{
    public struct Triangle
    {        
        public Vector3D[] p;
        public Pixel Color;

        public static Triangle Empty => new Triangle() { p = Vector3D.CreateArray(3), Color = new Pixel(255, 255, 255) };

        public static Triangle[] CreateArray(int length)
        {
            var array = new Triangle[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = Empty;
            }
            return array;
        }

        public Triangle(params Vector3D[] vectors)
        {
            if (vectors == null || vectors.Length != 3)
                p = Vector3D.CreateArray(3);
            else
                p = vectors;
            Color = new Pixel(255, 255, 255);
        }

        public Triangle(Pixel pixel, params Vector3D[] vectors)
        {
            if (vectors == null || vectors.Length != 3)
                p = Vector3D.CreateArray(3);
            else
                p = vectors;
            Color = pixel;
        }
    }
}
