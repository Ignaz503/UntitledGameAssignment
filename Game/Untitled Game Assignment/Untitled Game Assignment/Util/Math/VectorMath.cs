using System;
using System.Runtime.CompilerServices;
using GeoUtil.HelperCollections.Grids;
using Microsoft.Xna.Framework;
namespace Util.CustomMath
{
    public static class VectorMath
    {
        /// <summary>
        /// rotate vector
        /// </summary>
        /// <param name="vec">the vector to rotate</param>
        /// <param name="radians">the radians of the rotation</param>
        /// <returns>a rotated vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Rotate( Vector2 vec, float radians )
        {
            float cos = (float)System.Math.Cos(radians);
            float sin = (float)System.Math.Cos(radians);

            Vector2 p = Vector2.Zero;
            p.X = (vec.X * cos) - (vec.Y * sin);
            p.Y = (vec.X * sin) + (vec.Y * cos);
            return p;
        }

        /// <summary>
        /// rotate vector by ref
        /// </summary>
        /// <param name="vec">ref of vector</param>
        /// <param name="radians">radians</param>
        public static void Rotate( ref Vector2 vec, float radians ) 
        {
            float cos = (float)System.Math.Cos(radians);
            float sin = (float)System.Math.Cos(radians);

            Vector2 p = vec;
            vec.X = (p.X * cos) - (p.Y * sin);
            vec.Y = (p.X * sin) + (p.Y * cos);
        }

        /// <summary>
        /// takes abs of both vector elements
        /// </summary>
        /// <param name="vec">the vector to absolute</param>
        /// <returns>a vector with absolute values</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs( Vector2 vec ) 
        {
            return new Vector2( Math.Abs( vec.X ), Math.Abs( vec.Y ) );
        }
        
        /// <summary>
        /// takes abs of both vector elements
        /// </summary>
        /// <param name="vec">the vector to absolute</param>
        /// <returns>a vector with absolute values</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs( ref Vector2 vec ) 
        {
            vec.X = Math.Abs( vec.X );
            vec.Y = Math.Abs( vec.Y );
        }

        /// <summary>
        /// floors elements of vector
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector2 Floor( Vector2 v )
        {
            return new Vector2( (float)Math.Floor( v.X ), (float)Math.Floor( v.Y ) );
        }

        /// <summary>
        /// floors elements of vector
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Floor(ref Vector2 v)
        {
            v.X = (float)Math.Floor(v.X);
            v.Y = (float)Math.Floor(v.Y);
        }

        /// <summary>
        /// returns angle based on input vector, correctly rotated based on quadrant
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(float x, float y)
        {
            if (x == 0.0f)
                if (y > 0.0f)
                    return 90.0f;
                else
                    if (y == 0.0f)
                    return 0.0f;
                else
                    return 270.0f;
            else if (y == 0)
                if (x >= 0)
                    return 0.0f;
                else
                    return 180.0f;

            float ret = (float)Math.Atan(y / x) * 180.0f / (float)Math.PI;

            if (x < 0.0f && y < 0.0f) // quadrant 3
                ret = 180 + ret;
            else if (x < 0.0f) // quadrant 2
                ret = 180 + ret;
            else if (y < 0.0f) // quadrant 4
                ret = 270.0f + (90.0f + ret);

            return ret;
        }

        /// <summary>
        /// catumull rom interpolation
        /// </summary>
        /// <param name="p_1">p-1</param>
        /// <param name="p0">p0</param>
        /// <param name="p1">p1</param>
        /// <param name="p2">p2</param>
        /// <param name="t">time</param>
        /// <param name="tau">tau</param>
        /// <returns>a point on the catmul rom spline</returns>
        public static Vector2 CatmullRom( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t, float tau = 0.5f )
        {
            float xRes = p2.X + (t * ((p1.X * -tau) + (p3.X * tau))) + ((t*t)*(p1.X*2*tau + (tau-3)*p2.X + (3-2*tau)*p3.X + (p4.X*-tau))) + ((t*t*t)*((p1.X*-tau)+(2-tau)*p2.X+(tau-2)*p3.X+tau*p4.X));

            float yRes = p2.Y + (t * ((p1.Y * -tau) + (p3.Y * tau))) + ((t*t)*(p1.Y*2*tau + (tau-3)*p2.Y + (3-2*tau)*p3.Y + (p4.Y*-tau))) + ((t*t*t)*((p1.Y*-tau)+(2-tau)*p2.Y+(tau-2)*p3.Y+tau*p4.Y));

            return new Vector2( xRes, yRes );
        }

    }

    public static class MonoGameVectorExtensions
    {
        public static float MaxElement( this Vector2 v ) 
        {
            return System.Math.Max( v.X, v.Y );
        }

        public static float MinElement( this Vector2 v )
        {
            return System.Math.Min( v.X, v.Y );
        }

        public static Vector2 ToVec2( this Vector3 vec, bool projectiveGeometry=false ) 
        {
            if (projectiveGeometry)
                return new Vector2( vec.X / vec.Z, vec.Y / vec.Z );
            return new Vector2( vec.X, vec.Y );
        }

        public static Vector2Int RoundToInt( this Vector2 v ) 
        {
            return new Vector2Int( (int)Math.Round( v.X ), (int)Math.Round( v.Y ) );
        }

        public static Vector2Int FloorToInt( this Vector2 v ) 
        {
            return new Vector2Int( (int)Math.Floor( v.X ), (int)Math.Floor( v.Y ) );
        } 
        
        public static Vector2Int CeilToInt( this Vector2 v ) 
        {
            return new Vector2Int( (int)Math.Ceiling( v.X ), (int)Math.Ceiling( v.Y ) );
        }
    }

}
