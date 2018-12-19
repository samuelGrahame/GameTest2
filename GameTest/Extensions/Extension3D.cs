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
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine3D.Extensions
{
    public static class Extension3D
    {
        public static Pixel GetColor(Pixel pixel, float shade_factor)
        {
            return new Pixel(
                (byte)(pixel.R * shade_factor),
                (byte)(pixel.G * shade_factor),
                (byte)(pixel.B * shade_factor),
                pixel.A);
        }
        
        public static Matrix4x4 Matrix_MakeIdentity()
        {
            Matrix4x4 matrix = Matrix4x4.Empty;

            matrix[0, 0] = 1.0f;
            matrix[1, 1] = 1.0f;
            matrix[2, 2] = 1.0f;
            matrix[3, 3] = 1.0f;

            return matrix;
        }

        public static Vector3D Matrix_MultiplyVector(ref Matrix4x4 m, ref Vector3D i)
        {
            Vector3D v = Vector3D.Empty;
            v.x = i.x * m[0, 0] + i.y * m[1, 0] + i.z * m[2, 0] + i.w * m[3, 0];
            v.y = i.x * m[0, 1] + i.y * m[1, 1] + i.z * m[2, 1] + i.w * m[3, 1];
            v.z = i.x * m[0, 2] + i.y * m[1, 2] + i.z * m[2, 2] + i.w * m[3, 2];
            v.w = i.x * m[0, 3] + i.y * m[1, 3] + i.z * m[2, 3] + i.w * m[3, 3];
            return v;
        }
       
        public static Matrix4x4 Matrix_MakeRotationX(float fAngleRad)
        {
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = 1.0f;
            matrix[1, 1] = (float)Math.Cos(fAngleRad);
            matrix[1, 2] = (float)Math.Sin(fAngleRad);
            matrix[2, 1] = -(float)Math.Sin(fAngleRad);
            matrix[2, 2] = (float)Math.Cos(fAngleRad);
            matrix[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 Matrix_MakeRotationY(float fAngleRad)
        {
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = (float)Math.Cos(fAngleRad);
            matrix[0, 2] = (float)Math.Sin(fAngleRad);
            matrix[2, 0] = -(float)Math.Sin(fAngleRad);
            matrix[1, 1] = 1.0f;
            matrix[2, 2] = (float)Math.Cos(fAngleRad);
            matrix[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 Matrix_MakeRotationZ(float fAngleRad)
        {
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = (float)Math.Cos(fAngleRad);
            matrix[0, 1] = (float)Math.Sin(fAngleRad);
            matrix[1, 0] = -(float)Math.Sin(fAngleRad);
            matrix[1, 1] = (float)Math.Cos(fAngleRad);
            matrix[2, 2] = 1.0f;
            matrix[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 Matrix_MakeTranslation(float x, float y, float z)
        {
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = 1.0f;
            matrix[1, 1] = 1.0f;
            matrix[2, 2] = 1.0f;
            matrix[3, 3] = 1.0f;
            matrix[3, 0] = x;
            matrix[3, 1] = y;
            matrix[3, 2] = z;
            return matrix;
        }

        public static Matrix4x4 Matrix_MakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = 1.0f / (float)Math.Tan(fFovDegrees * 0.5f / 180.0f * 3.14159f);
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = fAspectRatio * fFovRad;
            matrix[1, 1] = fFovRad;
            matrix[2, 2] = fFar / (fFar - fNear);
            matrix[3, 2] = (-fFar * fNear) / (fFar - fNear);
            matrix[2, 3] = 1.0f;
            matrix[3, 3] = 0.0f;
            return matrix;
        }

        public static Matrix4x4 Matrix_MultiplyMatrix(ref Matrix4x4 m1, ref Matrix4x4 m2)
        {
            Matrix4x4 matrix = Matrix4x4.Empty;
            for (int c = 0; c < 4; c++)
                for (int r = 0; r < 4; r++)
                    matrix[r, c] = m1[r, 0] * m2[0, c] + m1[r, 1] * m2[1, c] + m1[r, 2] * m2[2, c] + m1[r, 3] * m2[3, c];
            return matrix;
        }

        public static Matrix4x4 Matrix_PointAt(ref Vector3D pos, ref Vector3D target, ref Vector3D up)
        {
            // Calculate new forward direction
            Vector3D newForward = Vector_Sub(ref target, ref pos);
            newForward = Vector_Normalise(ref newForward);

            // Calculate new Up direction
            Vector3D a = Vector_Mul(ref newForward, Vector_DotProduct(ref up, ref newForward));
            Vector3D newUp = Vector_Sub(ref up, ref a);
            newUp = Vector_Normalise(ref newUp);

            // New Right direction is easy, its just cross product
            Vector3D newRight = Vector_CrossProduct(ref newUp, ref newForward);

            // Construct Dimensioning and Translation Matrix	
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = newRight.x; matrix[0, 1] = newRight.y; matrix[0, 2] = newRight.z; matrix[0, 3] = 0.0f;
            matrix[1, 0] = newUp.x; matrix[1, 1] = newUp.y; matrix[1, 2] = newUp.z; matrix[1, 3] = 0.0f;
            matrix[2, 0] = newForward.x; matrix[2, 1] = newForward.y; matrix[2, 2] = newForward.z; matrix[2, 3] = 0.0f;
            matrix[3, 0] = pos.x; matrix[3, 1] = pos.y; matrix[3, 2] = pos.z; matrix[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4x4 Matrix_QuickInverse(ref Matrix4x4 m) // Only for Rotation/Translation Matrices
        {
            Matrix4x4 matrix = Matrix4x4.Empty;
            matrix[0, 0] = m[0, 0]; matrix[0, 1] = m[1, 0]; matrix[0, 2] = m[2, 0]; matrix[0, 3] = 0.0f;
            matrix[1, 0] = m[0, 1]; matrix[1, 1] = m[1, 1]; matrix[1, 2] = m[2, 1]; matrix[1, 3] = 0.0f;
            matrix[2, 0] = m[0, 2]; matrix[2, 1] = m[1, 2]; matrix[2, 2] = m[2, 2]; matrix[2, 3] = 0.0f;
            matrix[3, 0] = -(m[3, 0] * matrix[0, 0] + m[3, 1] * matrix[1, 0] + m[3, 2] * matrix[2, 0]);
            matrix[3, 1] = -(m[3, 0] * matrix[0, 1] + m[3, 1] * matrix[1, 1] + m[3, 2] * matrix[2, 1]);
            matrix[3, 2] = -(m[3, 0] * matrix[0, 2] + m[3, 1] * matrix[1, 2] + m[3, 2] * matrix[2, 2]);
            matrix[3, 3] = 1.0f;
            return matrix;
        }

        public static Vector3D Vector_Add(ref Vector3D v1, ref Vector3D v2)
        {
            return new Vector3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3D Vector_Sub(ref Vector3D v1, ref Vector3D v2)
        {
            return new Vector3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z );
        }

        public static Vector3D Vector_Mul(ref Vector3D v1, float k)
        {
            return new Vector3D(v1.x * k, v1.y * k, v1.z * k );
        }

        public static Vector3D Vector_Div(ref Vector3D v1, float k)
        {
            return new Vector3D ( v1.x / k, v1.y / k, v1.z / k );
        }

        public static float Vector_DotProduct(ref Vector3D v1, ref Vector3D v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static float Vector_Length(ref Vector3D v)
        {
            return (float)Math.Sqrt(Vector_DotProduct(ref v, ref v));
        }

        public static Vector3D Vector_Normalise(ref Vector3D v)
        {
            float l = Vector_Length(ref v);
            return new Vector3D( v.x / l, v.y / l, v.z / l );
        }

        public static Vector3D Vector_CrossProduct(ref Vector3D v1, ref Vector3D v2)
        {
            Vector3D v = new Vector3D
            {
                x = v1.y * v2.z - v1.z * v2.y,
                y = v1.z * v2.x - v1.x * v2.z,
                z = v1.x * v2.y - v1.y * v2.x
            };
            return v;
        }

        public static Vector3D Vector_IntersectPlane(ref Vector3D plane_p, ref Vector3D plane_n, ref Vector3D lineStart, ref Vector3D lineEnd)
        {
            plane_n = Vector_Normalise(ref plane_n);
            float plane_d = -Vector_DotProduct(ref plane_n, ref plane_p);
            float ad = Vector_DotProduct(ref lineStart, ref plane_n);
            float bd = Vector_DotProduct(ref lineEnd, ref plane_n);
            float t = (-plane_d - ad) / (bd - ad);
            Vector3D lineStartToEnd = Vector_Sub(ref lineEnd, ref lineStart);
            Vector3D lineToIntersect = Vector_Mul(ref lineStartToEnd, t);
            return Vector_Add(ref lineStart, ref lineToIntersect);
        }

        public static Triangle GetNonRef(Triangle tri)
        {
            return tri;
        }

        public static int Triangle_ClipAgainstPlane(Vector3D plane_p, Vector3D plane_n, ref Triangle in_tri, ref Triangle out_tri1, ref Triangle out_tri2)
        {
            // Make sure plane normal is indeed normal
            plane_n = Vector_Normalise(ref plane_n);

            // Return signed shortest distance from point to plane, plane normal must be normalised

            float dist(ref Vector3D p)
            {
                Vector3D n = Vector_Normalise(ref p);
                return (plane_n.x * p.x + plane_n.y * p.y + plane_n.z * p.z - Vector_DotProduct(ref plane_n, ref plane_p));
            }

            // Create two temporary storage arrays to classify points either side of plane
            // If distance sign is positive, point lies on "inside" of plane
            var inside_points = Vector3D.CreateArray(3); int nInsidePointCount = 0;
            var outside_points = Vector3D.CreateArray(3); int nOutsidePointCount = 0;

            // Get signed distance of each point in triangle to plane
            float d0 = dist(ref in_tri.p[0]);
            float d1 = dist(ref in_tri.p[1]);
            float d2 = dist(ref in_tri.p[2]);

            var in_triN = GetNonRef(in_tri);

            if (d0 >= 0.0f) { inside_points[nInsidePointCount++] = in_triN.p[0]; }
            else { outside_points[nOutsidePointCount++] = in_triN.p[0]; }
            if (d1 >= 0.0f) { inside_points[nInsidePointCount++] = in_triN.p[1]; }
            else { outside_points[nOutsidePointCount++] = in_triN.p[1]; }
            if (d2 >= 0.0f) { inside_points[nInsidePointCount++] = in_triN.p[2]; }
            else { outside_points[nOutsidePointCount++] = in_triN.p[2]; }

            // Now classify triangle points, and break the input triangle into 
            // smaller output triangles if required. There are four possible
            // outcomes...

            if (nInsidePointCount == 0)
            {
                // All points lie on the outside of plane, so clip whole triangle
                // It ceases to exist

                return 0; // No returned triangles are valid
            }

            if (nInsidePointCount == 3)
            {
                // All points lie on the inside of plane, so do nothing
                // and allow the triangle to simply pass through
                out_tri1 = in_triN; // = new Triangle(in_tri.Color, in_tri.p);

                return 1; // Just the one returned original triangle is valid
            }

            if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                // Triangle should be clipped. As two points lie outside
                // the plane, the triangle simply becomes a smaller triangle

                // Copy appearance info to new triangle
                out_tri1.Color = in_triN.Color;                

                // The inside point is valid, so keep that...
                out_tri1.p[0] = inside_points[0];

                // but the two new points are at the locations where the 
                // original sides of the triangle (lines) intersect with the plane
                out_tri1.p[1] = Vector_IntersectPlane(ref plane_p, ref plane_n, ref inside_points[0], ref outside_points[0]);
                out_tri1.p[2] = Vector_IntersectPlane(ref plane_p, ref plane_n, ref inside_points[0], ref outside_points[1]);

                return 1; // Return the newly formed single triangle
            }

            if (nInsidePointCount == 2 && nOutsidePointCount == 1)
            {
                // Triangle should be clipped. As two points lie inside the plane,
                // the clipped triangle becomes a "quad". Fortunately, we can
                // represent a quad with two new triangles

                // Copy appearance info to new triangles
                out_tri1.Color = in_triN.Color;
                out_tri2.Color = in_triN.Color;
                
                // The first triangle consists of the two inside points and a new
                // point determined by the location where one side of the triangle
                // intersects with the plane
                out_tri1.p[0] = inside_points[0];
                out_tri1.p[1] = inside_points[1];
                out_tri1.p[2] = Vector_IntersectPlane(ref plane_p, ref plane_n, ref inside_points[0], ref outside_points[0]);

                // The second triangle is composed of one of he inside points, a
                // new point determined by the intersection of the other side of the 
                // triangle and the plane, and the newly created point above
                out_tri2.p[0] = inside_points[1];
                out_tri2.p[1] = out_tri1.p[2];
                out_tri2.p[2] = Vector_IntersectPlane(ref plane_p, ref plane_n, ref inside_points[1], ref outside_points[0]);

                return 2; // Return two newly formed triangles which form a quad
            }

            return 0; // ?
        }
    }
}
