using Microsoft.Xna.Framework;

/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil.Linear
{
    public struct Line
    {
        public Vector2 v0 { get; private set; }
        public Vector2 v1 { get; private set; }

        public Vector3 HomogenousLineCoords => GeometryUtility.CalcHomogeneousLine(this);

        public Line(Vector2 sPoint, Vector2 ePoint) : this()
        {
            v0 = sPoint;
            v1 = ePoint;
        }
    }

}
