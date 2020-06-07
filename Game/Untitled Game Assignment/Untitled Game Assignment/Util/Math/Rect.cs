using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Util.CustomMath
{
    public struct Rect 
    {
        public Vector2 Position;
        public Vector2 Size;

        public float X 
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return Position.X; }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { Position.X = value; }
        } 
        
        public float Y 
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return Position.Y; }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { Position.Y = value; }
        } 
        
        public float Width 
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return Size.X; }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { Size.X = value; }
        } 
        
        public float Height 
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return Size.Y; }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { Size.Y = value; }
        }

        public float Left => X;
        public float Right => X + Width;

        public float Top => Y;
        public float Bottom => Y + Height;

        public Vector2 Center => Position + (Size * 0.5f);

        public Vector2 Extents => Size * 0.5f;

        public Vector2 TopLeft => new Vector2( Left, Top );

        public Vector2 TopRight => new Vector2( Right, Top);

        public Vector2 BottomLeft => new Vector2( Left, Bottom );

        public Vector2 BottomRight => new Vector2( Right, Bottom );

        public Rect( Vector2 position, Vector2 size )
        {
            Position = position;
            Size = size;
        }

        public Rect( float xMin, float yMin, float xMax, float yMax ) 
        {
            Position = new Vector2( xMin, yMin );
            Size = new Vector2( xMax - xMin, yMax - yMin );
        }

        public bool Contains( Vector2 p ) 
        {
            return p.X >= Left && p.X <= Right && p.Y >= Top && p.Y <= Bottom;
        }

    }
}
