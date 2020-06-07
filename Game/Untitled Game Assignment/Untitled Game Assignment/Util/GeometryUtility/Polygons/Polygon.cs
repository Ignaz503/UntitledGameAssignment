using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GeoUtil.Vertex;
using Util.CustomDebug;
/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil.Polygons
{
    public class Polygon : IPolygon
    {
        protected Vector2[] vertices;

        public Bounds2D Bounds { get; protected set; }

        public int VertexCount => vertices.Length;

        public Vector2 Centroid { get; protected set; }

        public VertexWinding VertexWinding { get; protected set; }

        public bool HasSelfIntersection => GeometryUtility.CheckPolygonSelfIntersection(this);

        public bool IsConvex => GeometryUtility.CheckPolygonConvex(this);

        public Polygon(Vector2[] vertices)
        {
            this.vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));

            CalculateNonSerializedData();
        }

        public Polygon(IList<Vector2> vert)
        {
            vertices = new Vector2[vert.Count];
            for (int i = 0; i < vert.Count; i++)
            {
                vertices[i] = new Vector2(vert[i].X, vert[i].Y);
            }
            CalculateNonSerializedData();
        }

        protected Polygon()
        { }

        public Polygon(IPolygon src)
        {
            CopyVertices(src);
            //CalculateBounds();
            //CalculateCentroid();
            Bounds = src.Bounds;
            Centroid = GeometryUtility.CalculateCentroid(this);
            VertexWinding = src.VertexWinding;
        }

        protected Polygon Pilfer(Polygon src, bool nullSrc = false)
        {
            Polygon p = new Polygon();
            p.vertices = src.vertices;
            p.Bounds = src.Bounds;
            p.Centroid = src.Centroid;
            p.VertexWinding = src.VertexWinding;
            return p;
        }

        void CopyVertices(IPolygon src)
        {
            this.vertices = new Vector2[src.VertexCount];
            for (int i = 0; i < VertexCount; i++)
            {
                this.vertices[i] = src[i];
            }
        }

        public Vector2 this[int i]
        {
            get { return vertices[i]; }
        }

        public void CalculateBounds()
        {
            Bounds = GeometryUtility.CalculateBounds(this);
        }

        public void CalculateWinding()
        {
            VertexWinding = GeometryUtility.GetWinding(this);
        }

        protected void CalculateCentroid()
        {
            Centroid = GeometryUtility.CalculateCentroid(this);
        }

        protected void CalculateNonSerializedData()
        {
            CalculateBounds();
            CalculateCentroid();
            CalculateWinding();
        }

        public void Log(ILogger l)
        {
            Stringify(l.LogMessage);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            Stringify((s)=> b.Append(s),addNewLine:true);
            return b.ToString();
        }

        private void Stringify(Action<string> stringAction,bool addNewLine = false)
        {
            string nl = addNewLine ? "\n" : "";
            stringAction("Polygon:"+ nl);
            stringAction($"Winding: {VertexWinding}" + nl);
            stringAction($"Centroid: {Centroid}" + nl);
            stringAction($"Bounds:\n\tCenter: {Bounds.Center}\n\tExtends: {Bounds.Extents}" + nl);
            stringAction("Vertices:" + nl);
            for (int i = 0; i < VertexCount; i++)
            {
                stringAction($"{i}: {vertices[i]}" + nl);
            }
        }

    }

    public class TexturedPolygon : Polygon 
    {
        Vector2[] uvs;

        public TexturedPolygon( Vector2[] vertices, Vector2[] uvs ) : base( vertices )
        {
            this.uvs = uvs;
        }

        public Vector2 GetUVForVertex( int i )
        {
            return uvs[i];
        }
    }

}


