using System;
using System.Runtime.CompilerServices;
namespace Util.CustomMath
{
    public static class Mathf
    {
        public const float PI = (float)System.Math.PI;
        public const float Deg2Rad = PI / 180f;
        public const float Rad2Deg= 180f / PI;

        /// <summary>
        /// Inverts an rotation given in radians
        /// </summary>
        /// <param name="radians">the rotation in rad we want the iverse of</param>
        /// <returns>the inverse of an rotation in radians</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float InverseRadians(float radians)
        {
            return (radians + (float)Math.PI) % (2f * (float)Math.PI);
        }

        /// <summary>
        /// inverts the rotaion given in degrees
        /// </summary>
        /// <param name="deg">rotaion in degrees</param>
        /// <returns>the inverse rotation</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float InverseDegree( float deg ) 
        {
            return InverseRadians( deg * Deg2Rad ) * Rad2Deg;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float Floor( float val )
        {
            return (float)Math.Floor( val );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float Ceil( float val )
        {
            return (float)Math.Ceiling( val );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float Round( float val )
        {
            return (float)Math.Round( val );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int FloorToInt( float val )
        {
            return (int)Math.Floor( val );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int CeilToInt( float val )
        {
            return (int)Math.Ceiling( val );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int RoundToInt( float val )
        {
            return (int)Math.Round( val );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal static float Abs( float v )
        {
            return Math.Abs( v );
        }

        /// <summary>
        /// returns decimal places
        /// </summary>
        /// <param name="v">the number the decimal places are wanted of</param>
        /// <returns>a number with 0 at the start and the decimal places of v</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float Dec( float v ) 
        {
            return v - (float)Math.Truncate( v );
        }

    }

}
