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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine3D
{
    public struct Matrix4x4
    {
        public float[,] m;
        public static Matrix4x4 Empty => new Matrix4x4 { m = new float[4, 4] };        

        public float this[int x, int y]    // Indexer declaration  
        {
            get 
            {                
                return m[x, y];
            }
            set {
                m[x, y] = value;
            }
        }
    }
}
