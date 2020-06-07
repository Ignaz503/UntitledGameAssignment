using System;
using System.Collections.Generic;
using System.Linq;
using GeoUtil.Vertex;
using Microsoft.Xna.Framework;

/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil.Polygons
{
    public class MutablePolygon : IPolygon
    {
        List<Vector2> vertices;

        public Bounds2D Bounds => GeometryUtility.CalculateBounds(this);

        public int VertexCount => vertices.Count;

        public VertexWinding VertexWinding { get; protected set; }

        public MutablePolygon(Vector2[] vertices) 
        {
            this.vertices = vertices.ToList();
            VertexWinding = GeometryUtility.GetWinding(this);
        }

        public MutablePolygon(List<Vector2> vertices)
        {
            this.vertices = vertices;
            VertexWinding = GeometryUtility.GetWinding(this);
        }

        public MutablePolygon(int i)
        {
            vertices = new List<Vector2>(i);
        }

        public MutablePolygon(IPolygon src)
        {
            vertices = new List<Vector2>();
            for (int i = 0; i < src.VertexCount; i++)
            {
                vertices.Add(src[i]);
            }
            VertexWinding = GeometryUtility.GetWinding(this);
        }

        public Vector2 this[int i]
        {
            get => vertices[i];
            set
            {
                if (i > vertices.Count || i < 0)
                    throw new IndexOutOfRangeException();

                if (i == vertices.Count)
                    vertices.Add(value);
                else
                    vertices[i] = value;
            }
        }

        public void Reverse()
        {
            vertices.Reverse();
        }

        public void RemoveRange(int beginIdx, int count,bool recalculateBounds=true)
        {
            var vert = vertices.ToList();
            vert.RemoveRange(beginIdx, count);
        }

        public Polygon MakeUnmutable(bool updateNonSerializedData = true)
        {
            return new Polygon(this);
        }

        public void RemoveAt(int idx)
        {
            vertices.RemoveAt(idx);
        }

        public void RecalculateVertexWinding()
        {
            VertexWinding = GeometryUtility.GetWinding(this);
        }
    }
}
