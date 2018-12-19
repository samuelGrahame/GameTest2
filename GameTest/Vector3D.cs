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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine3D
{
    public struct Vector3D
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static Vector3D Empty => new Vector3D(0, 0, 0, 1);
        public static Vector3D[] CreateArray(int length)
        {
            var array = new Vector3D[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = Empty;
            }
            return array;
        }

        public Vector3D(float _x = 0.0f, float _y = 0.0f, float _z = 0.0f, float _w = 1.0f)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }
    }
}
