using Microsoft.Xna.Framework;

namespace GeoUtil.HelperCollections.Grids
{
    public struct Vector2Int 
    {
        public int X,Y;

        public Vector2Int( int x, int y )
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals( object obj )
        {
            if (obj is Vector2Int other)
                return other.X == X && other.Y == Y;
            return false;
        }

        public override int GetHashCode()
        {
            return ((Vector2)this).GetHashCode();
        }

        public static explicit operator Vector2Int( Vector2 v ) 
        {
            return new Vector2Int( (int)v.X, (int)v.Y );
        }

        public static implicit operator Vector2( Vector2Int v ) 
        {
            return new Vector2( v.X, v.Y );
        }
    }

    public abstract class RebasingHelperGrid<C, R> : Vector2HelperGrid<C, R>
    where C : Cell<Vector2, R>
    {
        protected Vector2 origin;

        public RebasingHelperGrid(Vector2 origin, float resolution) : base(resolution)
        {
            this.origin = origin;
        }

        protected override Vector2Int GetCellPosition(Vector2 v)
        {
            return base.GetCellPosition(v - origin);
        }
    }
}