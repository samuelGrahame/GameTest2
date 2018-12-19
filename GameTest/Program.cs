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
using PixelEngine3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PixelEngine3D.Extensions.Extension3D;

namespace GameTest
{
    public class GameTest : Game
    {
        Mesh meshCube;
        Matrix4x4 matProj; // Matrix that converts from view space to screen space
        Vector3D vCamera = Vector3D.Empty;  // Location of camera in world space
        Vector3D vLookDir; // Direction vector along the direction camera points
        float fYaw;     // FPS Camera rotation in XZ plane
        float fTheta;	// Spins World transform
        Pixel ClearColor = Pixel.Presets.Black;

        static void Main(string[] args)
        {
            // Create an instance
            GameTest rp = new GameTest();

            rp.Construct(1200, 800, 1, 1);

            // Start and show a window
            rp.Start();
        }

        public override void OnCreate()
        {
            meshCube.LoadFromObjectFile("mountains.obj");
            
            matProj = Matrix_MakeProjection(90.0f, (float)ScreenHeight / (float)ScreenWidth, 0.1f, 1000.0f);

            base.OnCreate();
        }

        // Called once per frame
        public override void OnUpdate(float elapsed)
        {
            if (GetKey(Key.Up).Down)
                vCamera.y += 8.0f * elapsed;   // Travel Upwards

            if (GetKey(Key.Down).Down)
                vCamera.y -= 8.0f * elapsed;   // Travel Downwards


            // Dont use these two in FPS mode, it is confusing :P
            if (GetKey(Key.Left).Down)
                vCamera.x -= 8.0f * elapsed;   // Travel Along X-Axis

            if (GetKey(Key.Right).Down)
                vCamera.x += 8.0f * elapsed;   // Travel Along X-Axis
                                               ///////
            var vForward = Vector_Mul(ref vLookDir, 8.0f * elapsed);

            // Standard FPS Control scheme, but turn instead of strafe
            if (GetKey(Key.W).Down)
                vCamera = Vector_Add(ref vCamera, ref vForward);

            if (GetKey(Key.S).Down)
                vCamera = Vector_Sub(ref vCamera, ref vForward);

            if (GetKey(Key.A).Down)
                fYaw -= 2.0f * elapsed;

            if (GetKey(Key.D).Down)
                fYaw += 2.0f * elapsed;

            // Set up "World Tranmsform" though not updating theta 
            // makes this a bit redundant
            Matrix4x4 matRotZ, matRotX;
            //fTheta += 1.0f * fElapsedTime; // Uncomment to spin me right round baby right round
            matRotZ = Matrix_MakeRotationZ(fTheta * 0.5f);
            matRotX = Matrix_MakeRotationX(fTheta);

            Matrix4x4 matTrans;
            matTrans = Matrix_MakeTranslation(0.0f, 0.0f, 5.0f);

            Matrix4x4 matWorld;
            matWorld = Matrix_MakeIdentity();   // Form World Matrix
            matWorld = Matrix_MultiplyMatrix(ref matRotZ, ref matRotX); // Transform by rotation
            matWorld = Matrix_MultiplyMatrix(ref matWorld, ref matTrans); // Transform by translation

            // Create "Point At" Matrix for camera
            Vector3D vUp = new Vector3D ( 0, 1, 0 );
            Vector3D vTarget = new Vector3D(0, 0, 1 );
            Matrix4x4 matCameraRot = Matrix_MakeRotationY(fYaw);
            vLookDir = Matrix_MultiplyVector(ref matCameraRot, ref vTarget);
            vTarget = Vector_Add(ref vCamera, ref vLookDir);
            Matrix4x4 matCamera = Matrix_PointAt(ref vCamera, ref vTarget, ref vUp);

            // Make view matrix from camera
            Matrix4x4 matView = Matrix_QuickInverse(ref matCamera);

            // Store triagles for rastering later
            List<Triangle> vecTrianglesToRaster = new List<Triangle>();

            foreach (var tri in meshCube.Tris)
            {
                Triangle triProjected, triTransformed = Triangle.Empty, triViewed;

                // World Matrix Transform
                triTransformed.p[0] = Matrix_MultiplyVector(ref matWorld, ref tri.p[0]);
                triTransformed.p[1] = Matrix_MultiplyVector(ref matWorld, ref tri.p[1]);
                triTransformed.p[2] = Matrix_MultiplyVector(ref matWorld, ref tri.p[2]);

                // Calculate triangle Normal
                Vector3D normal, line1, line2;

                // Get lines either side of triangle
                line1 = Vector_Sub(ref triTransformed.p[1], ref triTransformed.p[0]);
                line2 = Vector_Sub(ref triTransformed.p[2], ref triTransformed.p[0]);

                // Take cross product of lines to get normal to triangle surface
                normal = Vector_CrossProduct(ref line1, ref line2);

                // You normally need to normalise a normal!
                normal = Vector_Normalise(ref normal);

                // Get Ray from triangle to camera
                Vector3D vCameraRay = Vector_Sub(ref triTransformed.p[0], ref vCamera);

                // If ray is aligned with normal, then triangle is visible
                if (Vector_DotProduct(ref normal, ref vCameraRay) < 0.0f)
                {
                    // Illumination
                    Vector3D light_direction = new Vector3D( 0.0f, 1.0f, -1.0f );
                    light_direction = Vector_Normalise(ref light_direction);

                    // How "aligned" are light direction and triangle surface normal?
                    float dp =  Math.Max(0.1f, Vector_DotProduct(ref light_direction, ref normal));

                    // Choose console colours as required (much easier with RGB)

                    var c = GetColor(triTransformed.Color, dp);
                    triTransformed.Color = c;

                    triViewed = Triangle.Empty;

                    // Convert World Space --> View Space
                    triViewed.p[0] = Matrix_MultiplyVector(ref matView, ref triTransformed.p[0]);
                    triViewed.p[1] = Matrix_MultiplyVector(ref matView, ref triTransformed.p[1]);
                    triViewed.p[2] = Matrix_MultiplyVector(ref matView, ref triTransformed.p[2]);

                    triViewed.Color = triTransformed.Color;
                    
                    // Clip Viewed Triangle against near plane, this could form two additional
                    // additional triangles. 
                    int nClippedTriangles = 0;
                    Triangle[] clipped = Triangle.CreateArray(2);
                    
                    nClippedTriangles = Triangle_ClipAgainstPlane(new Vector3D(0.0f, 0.0f, 0.1f), new Vector3D(0.0f, 0.0f, 1.0f), ref triViewed, ref clipped[0], ref clipped[1]);

                    // We may end up with multiple triangles form the clip, so project as
                    // required
                    triProjected = Triangle.Empty;

                    for (int n = 0; n < nClippedTriangles; n++)
                    {
                        // Project triangles from 3D --> 2D
                        triProjected.p[0] = Matrix_MultiplyVector(ref matProj, ref clipped[n].p[0]);
                        triProjected.p[1] = Matrix_MultiplyVector(ref matProj, ref clipped[n].p[1]);
                        triProjected.p[2] = Matrix_MultiplyVector(ref matProj, ref clipped[n].p[2]);

                        triProjected.Color = clipped[n].Color;                        

                        // Scale into view, we moved the normalising into cartesian space
                        // out of the matrix.vector function from the previous videos, so
                        // do this manually
                        triProjected.p[0] = Vector_Div(ref triProjected.p[0], triProjected.p[0].w);
                        triProjected.p[1] = Vector_Div(ref triProjected.p[1], triProjected.p[1].w);
                        triProjected.p[2] = Vector_Div(ref triProjected.p[2], triProjected.p[2].w);

                        // X/Y are inverted so put them back
                        triProjected.p[0].x *= -1.0f;
                        triProjected.p[1].x *= -1.0f;
                        triProjected.p[2].x *= -1.0f;
                        triProjected.p[0].y *= -1.0f;
                        triProjected.p[1].y *= -1.0f;
                        triProjected.p[2].y *= -1.0f;

                        // Offset verts into visible normalised space
                        Vector3D vOffsetView = new Vector3D( 1, 1, 0 );
                        triProjected.p[0] = Vector_Add(ref triProjected.p[0], ref vOffsetView);
                        triProjected.p[1] = Vector_Add(ref triProjected.p[1], ref vOffsetView);
                        triProjected.p[2] = Vector_Add(ref triProjected.p[2], ref vOffsetView);
                        triProjected.p[0].x *= 0.5f * (float)ScreenWidth;
                        triProjected.p[0].y *= 0.5f * (float)ScreenHeight;
                        triProjected.p[1].x *= 0.5f * (float)ScreenWidth;
                        triProjected.p[1].y *= 0.5f * (float)ScreenHeight;
                        triProjected.p[2].x *= 0.5f * (float)ScreenWidth;
                        triProjected.p[2].y *= 0.5f * (float)ScreenHeight;

                        // Store triangle for sorting
                        vecTrianglesToRaster.Add(triProjected);
                    }
                }
            }
            
            vecTrianglesToRaster.Sort((t1, t2) =>
            {
                float z1 = (t1.p[0].z + t1.p[1].z + t1.p[2].z) / 3.0f;
                float z2 = (t2.p[0].z + t2.p[1].z + t2.p[2].z) / 3.0f;

                var compareResult = z2.CompareTo(z1);
                // If the first items are equal (have a CompareTo result of 0) then compare on the second item.
                if (compareResult == 0)
                {
                    return z1.CompareTo(z2);
                }
                // Return the result of the first CompareTo.
                return compareResult;
            });

            Clear(ClearColor);

            foreach (var triToRaster in vecTrianglesToRaster)
            {
                // Clip triangles against all four screen edges, this could yield
                // a bunch of triangles, so create a queue that we traverse to 
                //  ensure we only test new triangles generated against planes
                
                LinkedList<Triangle> listTriangles = new LinkedList<Triangle>();

                // Add initial triangle
                listTriangles.AddFirst(triToRaster);
                var clipped = new Triangle[2];
                int nNewTriangles = 1;

                for (int p = 0; p < 4; p++)
                {
                    int nTrisToAdd = 0;
                    while (nNewTriangles > 0)
                    {
                        clipped[0] = Triangle.Empty;
                        clipped[1] = Triangle.Empty;

                        // Take triangle from front of queue
                        Triangle test = listTriangles.First();
                        listTriangles.RemoveFirst();
                        nNewTriangles--;

                        // Clip it against a plane. We only need to test each 
                        // subsequent plane, against subsequent new triangles
                        // as all triangles after a plane clip are guaranteed
                        // to lie on the inside of the plane. I like how this
                        // comment is almost completely and utterly justified
                        switch (p)
                        {
                            case 0:
                                nTrisToAdd = Triangle_ClipAgainstPlane(new Vector3D( 0.0f, 0.0f, 0.0f ), new Vector3D(0.0f, 1.0f, 0.0f), ref test, ref clipped[0], ref clipped[1]);
                                break;
					        case 1:
                                nTrisToAdd = Triangle_ClipAgainstPlane(new Vector3D(0.0f, (float)ScreenHeight - 1, 0.0f ), new Vector3D(0.0f, -1.0f, 0.0f ), ref test, ref clipped[0], ref clipped[1]);
                                break;
				            case 2:
                                nTrisToAdd = Triangle_ClipAgainstPlane(new Vector3D(0.0f, 0.0f, 0.0f ), new Vector3D(1.0f, 0.0f, 0.0f ), ref test, ref clipped[0], ref clipped[1]);
                                break;
					        case 3:
                                nTrisToAdd = Triangle_ClipAgainstPlane(new Vector3D((float)ScreenWidth - 1, 0.0f, 0.0f), new Vector3D(-1.0f, 0.0f, 0.0f), ref test, ref clipped[0], ref clipped[1]);
                                break;
                        }


                        // Clipping may yield a variable number of triangles, so
                        // add these new ones to the back of the queue for subsequent
                        // clipping against next planes
                        for (int w = 0; w < nTrisToAdd; w++)
                        {
                            var clpw = clipped[w];
                            listTriangles.AddLast(new Triangle(clpw.Color, clpw.p));
                        }
                            
                    }

                    nNewTriangles = listTriangles.Count;
                }

                // Draw the transformed, viewed, clipped, projected, sorted, clipped triangles
                foreach (var t in listTriangles)
                {                    
                    FillTriangle(t.p[0].x, t.p[0].y, t.p[1].x, t.p[1].y, t.p[2].x, t.p[2].y, t.Color);
                    //DrawTriangle(t.p[0].x, t.p[0].y, t.p[1].x, t.p[1].y, t.p[2].x, t.p[2].y, PIXEL_SOLID, FG_BLACK);
                }
            }
        }
    }
}
