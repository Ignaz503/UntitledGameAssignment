using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GeoUtil.Polygons;
using GeoUtil.Linear;
/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil.HelperCollections.Enumerables
{
    public class EdgeEnumeratedPolygon<E>:EnumerablePolygon<E>
         where E : IPolygonEdge
    {

        Func<Vector2, Vector2, int, int, E> edgeGenerator;

        public EdgeEnumeratedPolygon(IPolygon p, Func<Vector2, Vector2, int, int, E> edgeGenerator) : base(p)
        {
            this.edgeGenerator = edgeGenerator;
        }

        public override IEnumerator<E> GetEnumerator()
        {
            for (int i = 0; i < polygon.VertexCount; i++)
            {
                int idxNext = (i + 1) % polygon.VertexCount;
                var vCurrent = polygon[i];
                var vNext = polygon[idxNext];
                yield return edgeGenerator(vCurrent, vNext, i, idxNext);
            }
        }

    }  
}


