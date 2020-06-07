using GeoUtil;
using GeoUtil.Polygons;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using Util.CustomMath;
using VoronoiLib;
using VoronoiLib.Structures;

namespace UntitledGameAssignment.Util.Vornoi
{
    public static class Voronoi
    {
        public static List<IPolygon> Shatter( IList<Vector2> shatterPoints, Rect bounds )  
        {
            var sites = new List<FortuneSite>();

            float tlDist = float.MaxValue;
            float trDist = float.MaxValue;
            float blDist = float.MaxValue;
            float brDist = float.MaxValue;

            var closest = new ClosestsSites();

            for (int i = 0; i < shatterPoints.Count; i++)
            {
                var current =  new FortuneSite( shatterPoints[i].X, shatterPoints[i].Y ) ;
                sites.Add( current );

                var dist = Vector2.DistanceSquared(shatterPoints[i],bounds.TopLeft);
                if (dist < tlDist)
                {
                    tlDist = dist;
                    closest.TopLeft = current;
                }

                dist = Vector2.DistanceSquared( shatterPoints[i], bounds.TopRight );
                if(dist < trDist)
                {
                    trDist = dist;
                    closest.TopRight = current;
                }

                dist = Vector2.DistanceSquared( shatterPoints[i], bounds.BottomLeft );
                if (dist < blDist)
                {
                    blDist = dist;
                    closest.BottomLeft = current;
                }

                dist = Vector2.DistanceSquared( shatterPoints[i], bounds.BottomRight );
                if(dist < brDist)
                {
                    brDist = dist;
                    closest.BottomRight = current;
                }

            }

            var edges = FortunesAlgorithm.Run( sites, bounds.Left,bounds.Top,bounds.Right,bounds.Bottom);

            var polygons = new List<IPolygon>();

            for (int i = 0; i < sites.Count; i++)
            {
                var current = sites[i];
                var p = MakePolygon(edges,current,closest,bounds );
                polygons.Add( new Polygon( p ) );
            }

            return polygons;
        }

        static IList<Vector2> MakePolygon( LinkedList<VEdge> voronoiEdges, FortuneSite point, ClosestsSites closest, Rect r )
        {
            var polyEdges = new List<VEdge>();
            foreach (var edge in voronoiEdges)
            {
                if (edge.Left == point || edge.Right == point)
                {
                    polyEdges.Add( edge );
                }
            }

            List<VPoint> pointCloudTemp = new List<VPoint>();
            List<Vector2> pointCloud = new List<Vector2>();

            for (int i = 0; i < polyEdges.Count; i++)
            {
                var edge = polyEdges[i];
                if (!pointCloudTemp.Contains( edge.Start ))
                { 
                    pointCloudTemp.Add( edge.Start) ;
                    pointCloud.Add( edge.Start.ToVector2() );
                }
                if (!pointCloudTemp.Contains( edge.End ))
                {
                    pointCloudTemp.Add( edge.End );
                    pointCloud.Add( edge.End.ToVector2() );
                }
            }

            if (closest.TopLeft == point)
                pointCloud.Add( r.TopLeft );

            if (closest.TopRight == point)
                pointCloud.Add( r.TopRight );

            if (closest.BottomLeft == point)
                pointCloud.Add( r.BottomLeft );

            if (closest.BottomRight == point)
                pointCloud.Add( r.BottomRight );

            return GeometryUtility.ComputeConvexHull( pointCloud );
        }

        private struct ClosestsSites 
        {
            public FortuneSite TopLeft;
            public FortuneSite TopRight;
            public FortuneSite BottomLeft;
            public FortuneSite BottomRight;
        }
    }
}
