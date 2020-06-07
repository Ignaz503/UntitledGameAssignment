using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Util.CustomMath;
namespace GeoUtil.HelperCollections.Grids
{
    public abstract class Vector2HelperGrid<C, R> : HelperGrid<C, Vector2, R>
        where C : Cell<Vector2, R>
    {
        protected Vector2HelperGrid(float resolution) : base(resolution)
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual Vector2Int GetCellPosition(Vector2 v)
        {
            return (Vector2Int)VectorMath.Floor(v / resolution);
        }
    }
}