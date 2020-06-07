using GeoUtil.HelperCollections.Grids;
using Microsoft.Xna.Framework;
/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil.Polygons
{
    public struct PolygonVertexFinder
    {
        IPolygon p;
        PolygonVertexIDXHelperGrid helperGrid;

        public PolygonVertexFinder(IPolygon p,float resolution=.5f)
        {
            this.p = p;
            helperGrid = new PolygonVertexIDXHelperGrid(p, resolution);
        }

        public int this[Vector2 v]
        {
            get
            {
                return Find(v);
            }
        }

        private int Find(Vector2 v)
        {
            return helperGrid.GetValue(v);
        }

    }
}
