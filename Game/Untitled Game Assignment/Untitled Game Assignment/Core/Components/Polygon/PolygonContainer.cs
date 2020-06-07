using GeoUtil.Polygons;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using GeoUtil;
using GeoUtil.Vertex;

namespace UntitledGameAssignment.Core.Components
{
    public class PolygonContainer : Component
    {
        IPolygon p;
        public IPolygon Polygon
        {
            get => p;
            set { p = value; BuildMesh(); }
        }

        public VertexPositionTexture[] Mesh
        {
            get;
            private set;
        }

        public PolygonContainer( IPolygon p, GameObject obj ) : base( obj )
        {
            this.Polygon = p;
        }

        public override void OnDestroy()
        { }

        private void BuildMesh()
        {
            if (Polygon is TexturedPolygon tPol)
            {
                //we have tex coords given
                FillArray( ( i ) => tPol.GetUVForVertex( i ) );

            } else
            {
                //no tex coords calculate from bounds
                var bounds = Polygon.Bounds;

                FillArray( ( i ) => CalculateUV( Polygon[i], bounds ) );

            }

            void FillArray( Func<int, Vector2> getUV )
            {
                //actually triangulate and use that
                var triangles = GeometryUtility.Triangulate(Polygon);

                Mesh = new VertexPositionTexture[triangles.Count];

                for (int i = 0; i < triangles.Count - 2; i += 3)
                {
                    int idx = triangles[i];
                    int idx1 = triangles[i+1];
                    int idx2 = triangles[i+2];

                    Triangle t = new Triangle(Polygon[idx],Polygon[idx1],Polygon[idx2]);
                    if (t.VertexWinding != VertexWinding.CW)
                    {
                        t = GeometryUtility.ChangeOrientation( t, ( mP ) => new Triangle( mP[0], mP[1], mP[2] ), VertexWinding.CW );
                    }

                    Mesh[i] = new VertexPositionTexture( new Vector3( t[0], 0f ), getUV( idx ) );
                    Mesh[i + 1] = new VertexPositionTexture( new Vector3( t[1], 0f ), getUV( idx1 ) );
                    Mesh[i + 2] = new VertexPositionTexture( new Vector3( t[2], 0f ), getUV( idx2 ) );
                }
            }
        }

        Vector2 CalculateUV( Vector2 vertex, Bounds2D bounds )
        {
            return new Vector2(
                 vertex.X / bounds.MaxX,
                 vertex.Y / bounds.MaxY
                );
        }

    }
}
