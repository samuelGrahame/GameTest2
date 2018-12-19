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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine3D
{
    public struct Mesh
    {
        public List<Triangle> Tris;

        public bool LoadFromObjectFile(string sFilename)
        {
            Tris = new List<Triangle>();
            try
            {
                using (FileStream f = new FileStream(sFilename, FileMode.Open, FileAccess.Read))
                using(StreamReader sr = new StreamReader(f))
                {
                    var vectors = new List<Vector3D>();
                    while(sr.Peek() != -1)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        if(line[0] == 'v')
                        {
                            var vectorData = line.Split(' ');
                            vectors.Add(new Vector3D(
                                    float.Parse(vectorData[1]),
                                    float.Parse(vectorData[2]),
                                    float.Parse(vectorData[3])
                                ));
                        }
                        if(line[0] == 'f')
                        {
                            var indexData = line.Split(' ');
                            Tris.Add(new Triangle(
                                    vectors[int.Parse(indexData[1]) - 1],
                                    vectors[int.Parse(indexData[2]) - 1],
                                    vectors[int.Parse(indexData[3]) - 1]
                                ));
                        }

                    }                    
                }
            }
            catch (Exception)
            {
                return false;
            }            

            return true;
        }
    }
}
