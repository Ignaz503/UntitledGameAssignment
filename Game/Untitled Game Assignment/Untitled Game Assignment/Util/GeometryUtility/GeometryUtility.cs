using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using GeoUtil.Exceptions;
using GeoUtil.HelperCollections.Enumerables;
using GeoUtil.Polygons;
using GeoUtil.Linear;
using GeoUtil.Vertex;
using Util.CustomMath;
using Loyc.Collections;
/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil
{
    public static class GeometryUtility
    {
        /// <summary>
        /// calculate point p is left right or on a line formed by lp0 to lp1
        /// </summary>
        /// <param name="lP0">the first point forming the line</param>
        /// <param name="lP1">the second point of the line</param>
        /// <param name="p">the point where whe want to know the relative position to the line of</param>
        /// <returns>the line position</returns>
        public static LinePosition CalculateLinePosition(Vector2 lP0, Vector2 lP1, Vector2 p)
        {
            float val = ((lP1.X - lP0.X) * (p.Y - lP0.Y) -
                            (p.X - lP0.X) * (lP1.Y - lP0.Y));

            return ((int)(Math.Sign(val) * Math.Ceiling(Math.Abs(val)))).ToLinePosition();
        }

        /// calculate point p is left right or on a line formed by lp0 to lp1
        /// </summary>
        /// <param name="l">the line we check against</param>
        /// <param name="p">the point where whe want to know the relative position to the line of</param>
        /// <returns>the line position</returns>
        public static LinePosition CalculateLinePosition(Line l, Vector2 p)
        {
            return CalculateLinePosition(l.v0, l.v1, p);
        }

        /// <summary>
        /// calculate if the points form a counter clockwise angle
        /// </summary>
        /// <param name="p0">the first point forming the line</param>
        /// <param name="p1">the second point of the line</param>
        /// <param name="p3">the point where whe want to know the relative position to the line of</param>
        /// <returns>1 if ccw, 0 colinear, -1 if cw</returns>
        public static int CalculateCCW(Vector2 p0, Vector2 p1, Vector2 p3)
        {
            return CalculateLinePosition(p0, p1, p3).ToInt();
        }

        /// <summary>
        /// check if edge pair intersects
        /// </summary>
        /// <param name="p">the edge pair to check</param>
        /// <returns>true if intersect</returns>
        //https://algs4.cs.princeton.edu/91primitives/
        public static bool Intersects<T>(EdgePair<T> p) where T : IPolygonEdge
        {
            return Intersects(p.e0, p.e1);
        }

        /// <summary>
        /// check if line containers intersects
        /// </summary>
        /// <param name="p">the edge pair to check</param>
        /// <returns>true if intersect</returns>
        //https://algs4.cs.princeton.edu/91primitives/
        public static bool Intersects<T>(T l0, T l1) where T : ILineContainer
        {
            return Intersects(l0.Line, l1.Line);
        }

        /// <summary>
        /// check if edge pair intersects
        /// </summary>
        /// <param name="p">the edge pair to check</param>
        /// <returns>true if intersect</returns>
        //https://algs4.cs.princeton.edu/91primitives/
        public static bool Intersects(Line l0, Line l1)
        {
            if (CalculateCCW(l0.v0, l0.v1, l1.v0) * CalculateCCW(l0.v0, l0.v1, l1.v1) > 0) return false;
            if (CalculateCCW(l1.v0, l1.v1, l0.v0) * CalculateCCW(l1.v0, l1.v1, l0.v1) > 0) return false;
            return true;
        }

        /// <summary>
        /// calculates the centroid for a polygon
        /// </summary>
        /// <param name="polygon">polygon the centroid is calculated of</param>
        /// <returns>the centroid of a polygon</returns>
        public static Vector2 CalculateCentroid(in IPolygon polygon)
        {
            Vector2 centroid = Vector2.Zero;
            for (int i = 0; i < polygon.VertexCount; i++)
            {
                centroid += polygon[i];
            }
            centroid /= (float)polygon.VertexCount;
            return centroid;
        }

        /// <summary>
        /// returns the instersection point of to line segments
        /// </summary>
        /// <param name="l0">first line segment</param>
        /// <param name="l1">second line segment</param>
        /// <returns>the intersection point</returns>
        public static Vector2 IntersectUnknownIntersection(Line l0, Line l1)
        {
            if (Intersects(l0, l1))
            {
                var res = IntersectHomogenouseCoords(l0, l1);
                return new Vector2(res.X/res.Z,res.Y/res.Z);
            }
            throw new DivideByZeroException();
        }

        /// <summary>
        /// returns the instersection point of to line segments
        /// </summary>
        /// <param name="l0">first line segment</param>
        /// <param name="l1">second line segment</param>
        /// <returns>the intersection point</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 IntersectKnownIntersection(Line l0, Line l1)
        {
            var res = IntersectHomogenouseCoords(l0, l1);
            return new Vector2( res.X / res.Z, res.Y / res.Z );
        }

        /// <summary>
        /// calculates lineintersection in homogneous coords from given line segments
        /// </summary>
        /// <param name="l0">first line segment</param>
        /// <param name="l1">second line segment</param>
        /// <returns>intersection point in hom coords</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 IntersectHomogenouseCoords(Line l0, Line l1)
        {
            return Vector3.Cross(l0.HomogenousLineCoords, l1.HomogenousLineCoords);
        }

        /// <summary>
        /// calculates the homogenoues cordinates line for a line segment
        /// </summary>
        /// <param name="l">the line segments</param>
        /// <returns>the line coords in porjective space</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 CalcHomogeneousLine(Line l)
        {
            return Vector3.Cross(new Vector3( l.v0, 1), new Vector3( l.v1, 1));
        }

        /// <summary>
        /// calculate polygon orientation
        ///  works with self intersect (returns most common winding)
        /// </summary>
        /// <param name="poly">the polygon we want the orientation of</param>
        /// <returns>the vertex orientation</returns>
        public static VertexWinding GetWinding(in IPolygon poly)
        {
            EdgeEnumeratedPolygon<Edge> p = new EdgeEnumeratedPolygon<Edge>(poly, Edge.Create);
            float sum = 0;
            foreach (var e in p)
            {
                sum += (e.EPoint.X - e.SPoint.X) * (e.EPoint.Y + e.SPoint.Y);
            }
            if (sum >= 0)
                return VertexWinding.CW;
            return VertexWinding.CCW;
        }
                /// <summary>
        /// gets the index of the first(if multiple had the same min y value) vertex with the minimum y value
        /// </summary>
        /// <param name="poly">the polygon</param>
        /// <returns>the index of the min y value vertex</returns>
        private static int GetMinYVertexIndex(in IPolygon poly)
        {
            float min = float.MaxValue;
            int idx = -1;
            for (int i = 0; i < poly.VertexCount; i++)
            {
                if (poly[i].Y < min)
                {
                    min = poly[i].Y;
                    idx = i;
                }
            }
            return idx;
        }

        /// <summary>
        /// calculates the next vertex index
        /// </summary>
        /// <param name="poly">polygon we are interessted in</param>
        /// <param name="curIdx">current vertex index</param>
        /// <returns>the next index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetNextVertexIdx(in IPolygon poly, int curIdx)
        {
            return (curIdx + 1) % poly.VertexCount;
        }

        /// <summary>
        /// calculates the next vertex index
        /// </summary>
        /// <param name="poly">polygon we are interessted in</param>
        /// <param name="curIdx">current vertex index</param>
        /// <returns>the next index</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetPrevVertexIdx(in IPolygon poly, int curIdx)
        {
            return curIdx - 1 < 0 ? poly.VertexCount - 1 : curIdx - 1;
        }

        /// <summary>
        /// checks if a point is in a certain relational position to a line
        /// </summary>
        /// <param name="lP0">line point 1</param>
        /// <param name="lP1">line pont 2</param>
        /// <param name="p">point to check</param>
        /// <param name="posToCheckAgains">the line position we want to check against</param>
        /// <returns>true if point hast that position</returns>
        private static bool CheckPointPositionAgainstLine(Vector2 lP0, Vector2 lP1, Vector2 p, LinePosition posToCheckAgains)
        {
            return CalculateLinePosition(lP0, lP1, p) == posToCheckAgains;
        }


        /// <summary>
        /// checks if a polygon is malformed in any way
        /// </summary>
        /// <param name="p">the polygon to check</param>
        /// <param name="malformedType">what malformation type(out param)</param>
        /// <returns>true if malformed</returns>
        public static bool IsMalformed(in IPolygon p, out string malformedType)
        {
            malformedType = "";
            //TODO are there more malformation types?
            bool malformed = CheckPolygonSelfIntersection(p);
            if (malformed)
                malformedType = "SelfIntersection";
            return malformed;
        }

        /// <summary>
        /// orients the vertces in a polygon to a certain winding
        /// </summary>
        /// <param name="p">The polygon p</param>
        /// <param name="orientation">the winding wanted</param>
        /// <returns>a NEW polygon (if the old one had a different winding) with the new winding</returns>
        public static T ChangeOrientation<T>(in T p,Func<MutablePolygon,T> polygonConverter, VertexWinding orientation = VertexWinding.CW)
            where T : IPolygon
        {
            if (orientation == p.VertexWinding)
                return p;

            int startIdx = GetMinYVertexIndex(p);
            var muteP = new MutablePolygon(p.VertexCount);

            muteP[0] = p[startIdx];

            int currentIdx = startIdx;
            int i = 1;
            //TODO
            do
            {
                currentIdx = GetPrevVertexIdx(p, currentIdx);
                muteP[i] = p[currentIdx];
                i++;

            } while (startIdx != GetPrevVertexIdx(p, currentIdx));


            return polygonConverter(muteP);

        }

        /// <summary>
        /// check if p is left of line
        /// </summary>
        /// <param name="lP0">line point 1</param>
        /// <param name="lP1">line pont 2</param>
        /// <param name="p">point to check</param>
        /// <returns>true if left of line</returns>
        public static bool IsLeftOfLine(Vector2 lP0, Vector2 lP1, Vector2 p)
        {
            return CheckPointPositionAgainstLine(lP0, lP1, p, LinePosition.left);
        }


        /// <summary>
        /// check if p is left of line
        /// </summary>
        ///<param name="l">The line segement container</param>
        /// <param name="p">point to check</param>
        /// <returns>true if left of line</returns>
        public static bool IsLeftOfLine(ILineContainer l, Vector2 p)
        {
            return IsLeftOfLine(l.Line, p);
        }

        /// <summary>
        /// check if p is left of line
        /// </summary>
        ///<param name="l">The line segement</param>
        /// <param name="p">point to check</param>
        /// <returns>true if left of line</returns>
        public static bool IsLeftOfLine(Line l, Vector2 p)
        {
            return IsLeftOfLine(l.v0, l.v1, p);
        }

        /// <summary>
        /// check if p is right of line
        /// </summary>
        /// <param name="lP0">line point 1</param>
        /// <param name="lP1">line pont 2</param>
        /// <param name="p">point to check</param>
        /// <returns>true if right of line</returns>
        public static bool IsRightOfLine(Vector2 lP0, Vector2 lP1, Vector2 p)
        {
            return CheckPointPositionAgainstLine(lP0, lP1, p, LinePosition.right);
        }

        /// <summary>
        /// check if p is right of line
        /// </summary>
        ///<param name="l">The line segement container</param>
        /// <param name="p">point to check</param>
        /// <returns>true if right of line</returns>
        public static bool IsRightOfLine(ILineContainer l, Vector2 p)
        {
            return IsRightOfLine(l.Line.v0, l.Line.v1, p);
        }

        /// <summary>
        /// check if p is right of line
        /// </summary>
        ///<param name="l">The line segement</param>
        /// <param name="p">point to check</param>
        /// <returns>true if right of line</returns>
        public static bool IsRightOfLine(Line l, Vector2 p)
        {
            return IsRightOfLine(l.v0, l.v1, p);
        }

        /// <summary>
        /// check if p is on line
        /// </summary>
        /// <param name="lP0">line point 1</param>
        /// <param name="lP1">line pont 2</param>
        /// <param name="p">point to check</param>
        /// <returns>true if on line</returns>
        public static bool IsOnLine(Vector2 lP0, Vector2 lP1, Vector2 p)
        {
            return CheckPointPositionAgainstLine(lP0, lP1, p, LinePosition.on);
        }

        /// <summary>
        /// check if p is on line
        /// </summary>
        ///<param name="l">The line segement container</param>
        /// <param name="p">point to check</param>
        /// <returns>true if on line</returns>
        public static bool IsOnLine(ILineContainer l, Vector2 p)
        {
            return IsOnLine(l.Line.v0, l.Line.v1, p);
        }

        /// <summary>
        /// check if p is on line
        /// </summary>
        ///<param name="l">The line segement</param>
        /// <param name="p">point to check</param>
        /// <returns>true if left of line</returns>
        public static bool IsOnLine(Line l, Vector2 p)
        {
            return IsOnLine(l.v0, l.v1, p);
        }

        /// <summary>
        /// winding number point to polygon
        /// </summary>
        /// <param name="p">point to test</param>
        /// <param name="polygon">polygon to test against</param>
        /// <returns> == 0 iff outside</returns>
        public static int WindingNumberPointInPolygon(Vector2 p, in IPolygon polygon)
        {
            int windingNumber = 0;
            var pol = new EdgeEnumeratedPolygon<Edge>(polygon, Edge.Create);
            foreach (Edge edge in pol)
            {
                if (edge.SPoint.Y <= p.Y && edge.EPoint.Y > p.Y)
                {
                    //upward crossing
                    if (IsLeftOfLine(edge.SPoint, edge.EPoint, p))
                        windingNumber++;

                }
                else if (edge.EPoint.Y <= p.Y)
                {
                    //downward crossing
                    if (IsRightOfLine(edge.SPoint, edge.EPoint, p))
                        windingNumber--;
                }
            }
            return windingNumber;
        }

        /// <summary>
        /// calculates the intersection point of ray and line, if they have one
        /// </summary>
        /// <param name="v0">line defining point 1</param>
        /// <param name="v1">line defining point 2</param>
        /// <param name="rayOrigin">origin of ray</param>
        /// <param name="rayDirection">direction of ray</param>
        /// <param name="intersect">out intersection point</param>
        /// <returns>true if intersection false otherwise</returns>
        public static bool LineRayIntersect(Vector2 v0, Vector2 v1, Vector2 rayOrigin, Vector2 rayDirection, out Vector2 intersect)
        {
            float dist;
            if (LineRayIntersectDist(v0, v1, rayOrigin, rayDirection, out dist))
            {
                intersect = rayOrigin + dist * rayDirection;
                return true;
            }
            intersect = Vector2.Zero;
            return false;
        }

        /// <summary>
        /// calculates the disstance from ray origin to line intersection
        /// </summary>
        /// <param name="v0">line defining point 1</param>
        /// <param name="v1">line defining point 2</param>
        /// <param name="rayOrigin">origin of ray</param>
        /// <param name="rayDirection">direction of ray</param>
        /// <param name="intersect">out intersection point</param>
        /// <returns>true if intersection false otherwise</returns>
        public static bool LineRayIntersectDist(Vector2 v0, Vector2 v1, Vector2 rayOrigin, Vector2 rayDirection, out float dist)
        {
            dist = float.NaN;
            Vector2 originV0 = rayOrigin - v0;
            Vector2 v1v0 = v1 - v0;
            Vector2 rayNormal = new Vector2(-rayDirection.Y, rayDirection.X);

            float dot = Vector2.Dot(v1v0, rayNormal);
            if (Math.Abs(dot) < 0.000001f)
                return false;

            float t1 = (v1v0.X * originV0.Y - v1v0.Y * originV0.X) / dot;
            float t2 = Vector2.Dot(originV0, rayNormal) / dot;

            if (t1 >= 0.0f && (t2 >= 0.0f && t2 <= 1f))
            {
                dist = t1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// calculates all hits of a ray and a polygon edge
        /// </summary>
        /// <param name="r">the ray</param>
        /// <param name="p">the polygon</param>
        /// <param name="hits">all the hits</param>
        /// <returns>bool if ray and polygon intersect</returns>
        public static bool PolygonRayIntersection(Vector2 rOrigin, Vector2 rDirection, in IPolygon p, out List<Vector2> hits)
        {
            hits = new List<Vector2>();

            var edgePoly = new EdgeEnumeratedPolygon<Edge>(p, Edge.Create);

            foreach (var edge in edgePoly)
            {
                Vector2 intersect;
                if (LineRayIntersect(edge.SPoint, edge.EPoint, rOrigin, rDirection, out intersect))
                {
                    hits.Add(intersect);
                }
            }

            if (hits.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// calculates the closest ray polygon intersection point
        /// </summary>
        /// <param name="rOrigin">ray origin</param>
        /// <param name="rDirection">ray direction</param>
        /// <param name="p">the polygon</param>
        /// <param name="closestIntersection">the intersection point</param>
        /// <returns>true if intersection</returns>
        public static bool PolygonRayClosestIntersection(Vector2 rOrigin, Vector2 rDirection, in IPolygon p, out Vector2 closestIntersection)
        {
            bool hasIntersection = false;
            var edgePoly = new EdgeEnumeratedPolygon<Edge>(p, Edge.Create);

            float minDist = float.NaN;
            foreach (var edge in edgePoly)
            {
                float dist;
                if (LineRayIntersectDist(edge.SPoint, edge.EPoint, rOrigin, rDirection, out dist))
                {
                    hasIntersection = true;
                    if (dist < minDist)
                        minDist = dist;
                }
            }

            if (hasIntersection)
                closestIntersection = rOrigin + minDist * rDirection;
            else
                closestIntersection = Vector2.Zero;

            return hasIntersection;
        }

        /// <summary>
        /// checks if a polygon contains a point
        /// </summary>
        /// <param name="p">the point</param>
        /// <param name="polygon">the polygon</param>
        /// <returns>true if contained</returns>
        public static bool PolygonContains(Vector2 p, in IPolygon polygon)
        {
            return polygon.Bounds.Contains(p) && WindingNumberPointInPolygon(p, polygon) != 0;
        }

        struct MinMaxTracker 
        {
            float _min;
            float _max;
            public float min 
            {
                get { return _min; }
                set 
                {
                    if (value < _min)
                        _min = value;
                }
            } 
            public float max 
            {
                get { return _max; }
                set 
                {
                    if (value > _max)
                        _max = value;
                }
            }

            public MinMaxTracker(float min = float.MaxValue, float max = float.MinValue)
            {
                this._min = min;
                this._max = max;
            }
        }

        /// <summary>
        /// calculates bounds of polygon
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Bounds2D CalculateBounds(in IPolygon p)
        {
            var xBounds = new MinMaxTracker(float.MaxValue,float.MinValue);
            var yBounds = new MinMaxTracker(float.MaxValue,float.MinValue);

            for (int i = 0; i < p.VertexCount; i++)
            {
                var v = p[i];

                minMaxUpdate(v.X, ref xBounds);
                minMaxUpdate(v.Y, ref yBounds);

            }

            return new Bounds2D( xBounds.min, xBounds.max, yBounds.min, yBounds.max );

            void minMaxUpdate(float value, ref MinMaxTracker tracker)
            {
                tracker.min = value;
                tracker.max = value;
            }
        }

        /// <summary>
        /// checks a polygon for self intersections
        /// </summary>
        /// <param name="p">the poolygon to check</param>
        /// <returns></returns>
        public static bool CheckPolygonSelfIntersection(in IPolygon p)
        {
            return SelfIntersection(p) > 0 ? true : false;
        }

        /// <summary>
        /// triangulate via ear clipping
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static List<int> Triangulate(IPolygon p)
        {
            var triIdxs = new List<int>();
            MutablePolygon muteablePoly = new MutablePolygon(p);
            PolygonVertexFinder finder = new PolygonVertexFinder(p);
            while (muteablePoly.VertexCount > 3)
            {
                var ear = FindEar(muteablePoly);

                int[] actualEar = new int[3];

                actualEar[0] = finder[muteablePoly[ear[0]]];
                actualEar[1] = finder[muteablePoly[ear[1]]];
                actualEar[2] = finder[muteablePoly[ear[2]]];

                triIdxs.AddRange(actualEar);
                RemoveEar(muteablePoly, ear);
            }

            triIdxs.Add(finder[muteablePoly[GetPrevVertexIdx(muteablePoly, 0)]]);
            triIdxs.Add(finder[muteablePoly[0]]);
            triIdxs.Add(finder[muteablePoly[GetNextVertexIdx(muteablePoly, 0)]]);

            return triIdxs;
        }

        /// <summary>
        /// removes an ear from a mutable polygon
        /// </summary>
        /// <param name="poly">the polygon we remove the ear from</param>
        /// <param name="ear">the ear indices we want to remove</param>
        public static void RemoveEar(MutablePolygon poly, int[] ear)
        {
            poly.RemoveAt(ear[1]);
        }

        /// <summary>
        /// finds an ear of a polygon
        /// </summary>
        /// <param name="poly"></param>
        /// <returns>a list of indices of vertixces taht form an ear</returns>
        public static int[] FindEar(MutablePolygon poly)
        {
            int i = 0;
            int[] indices = default;
            bool earNotFound = true;
            while (earNotFound)
            {
                indices = l_CalcIndex(i);

                if (IsConvexVertex(poly,indices[0], indices[1], indices[2]))
                {
                    int prevOneOver = indices[0] - 1 < 0 ? poly.VertexCount - 1 : indices[0] - 1;
                    int nextOneOver = (indices[2] + 1) % poly.VertexCount;

                    //if (!IsConcavVertex(poly,prevOneOver, indices[0], indices[1]) &&
                    //   !IsConcavVertex(poly,indices[0], indices[1], indices[2]) &&
                    //   !IsConcavVertex(poly,indices[1], indices[2], nextOneOver))
                    //{
                    //    earNotFound = false;
                    //}
                    if (!PotentialTriangleContainsOtherPolygonVertex(poly, indices))
                        earNotFound = false;
                }
                if (earNotFound)
                    i = GetNextVertexIdx(poly, i);

            }
            return indices;
            int[] l_CalcIndex(int idx)
            {
                int prev = GetPrevVertexIdx(poly, idx);
                int next = GetNextVertexIdx(poly, idx);
                return new int[] { prev, idx, next };
            }
        }

        /// <summary>
        /// checks if the potenial found ear contains any other points of the polygon
        /// </summary>
        /// <param name="p"></param>
        /// <param name="earExclude"></param>
        /// <returns></returns>
        private static bool PotentialTriangleContainsOtherPolygonVertex(in IPolygon p, int[] earExclude)
        {
            var triangle = new Triangle(
                p[earExclude[0]],
                p[earExclude[1]],
                p[earExclude[2]]
                );

            for (int i = 0; i < p.VertexCount; i++)
            {
                if (i != earExclude[0] &&
                        i != earExclude[1] &&
                        i != earExclude[2])
                {//i is not an ear candidate
                    Vector2 point = p[i];

                    if (PolygonContains(point, triangle))
                        return true;

                }
            }
            return false;
        }

        /// <summary>
        /// returns true if a vertex is a concave vertex of a polygon
        /// </summary>
        /// <param name="p">the polygon</param>
        /// <param name="prevVIdx">the previous vertex</param>
        /// <param name="curVIdx">vertex interessted in</param>
        /// <param name="nextVIdx">next vertex</param>
        /// <returns>true if concave vertex</returns>
        public static bool IsConcavVertex(in IPolygon p, int prevVIdx, int curVIdx, int nextVIdx)
        {
            return GetVertexTurnType(p, prevVIdx, curVIdx, nextVIdx) == VertexTurnType.concave;
        }

        /// <summary>
        /// returns true if a vertex is a convex vertex of a polygon
        /// </summary>
        /// <param name="p">the polygon</param>
        /// <param name="prevVIdx">the previous vertex</param>
        /// <param name="curVIdx">vertex interessted in</param>
        /// <param name="nextVIdx">next vertex</param>
        /// <returns>true if convex vertex</returns>
        public static bool IsConvexVertex(in IPolygon p, int prevVIdx, int curVIdx, int nextVIdx)
        {
            return GetVertexTurnType(p, prevVIdx, curVIdx, nextVIdx) == VertexTurnType.convex;
        }

        /// <summary>
        /// returns true if a vertex is a colinear vertex of a polygon
        /// </summary>
        /// <param name="p">the polygon</param>
        /// <param name="prevVIdx">the previous vertex</param>
        /// <param name="curVIdx">vertex interessted in</param>
        /// <param name="nextVIdx">next vertex</param>
        /// <returns>true if colinear vertex</returns>
        public static bool IsColinearVertex(in IPolygon p, int prevVIdx, int curVIdx, int nextVIdx)
        {
            return GetVertexTurnType(p, prevVIdx, curVIdx, nextVIdx) == VertexTurnType.colinear;
        }

        /// <summary>
        /// returns the turn type a vertex makes in a polygon, useful to find out if concave or convex vertex
        /// </summary>
        /// <param name="poly">the polygon the vertices are from</param>
        /// <param name="prevVertIdx">previous vertex index</param>
        /// <param name="curVertIdx">vertex we are interessted in</param>
        /// <param name="nextVertIdx">next vertex index</param>
        /// <param name="cwComparison">turn type comparison when cw vertex winding</param>
        /// <param name="ccwComparison">turn type comparison when cw vertex winding</param>
        /// <returns></returns>
        public static VertexTurnType GetVertexTurnType(in IPolygon poly, int prevVertIdx, int curVertIdx, int nextVertIdx)
        {
            //Debug.Log($"prev:{prevVertIdx}, cur: {curVertIdx}, next: {nextVertIdx}, total: {poly.VertexCount}");
            LinePosition p = CalculateLinePosition(poly[prevVertIdx], poly[nextVertIdx], poly[curVertIdx]);
            if (poly.VertexWinding == VertexWinding.CW)
            {
                if (p == LinePosition.right)
                    return VertexTurnType.concave;
                else if (p == LinePosition.left)
                    return VertexTurnType.convex;
                else
                    return VertexTurnType.colinear;
            }
            else
            {//ccw poligon
                if (p == LinePosition.left)
                    return VertexTurnType.concave;
                else if (p == LinePosition.right)
                    return VertexTurnType.convex;
                else
                    return VertexTurnType.colinear;
            }
        }

        /// <summary>
        /// checks if polygon is convex
        /// </summary>
        /// <param name="polygon">the polygon to check</param>
        /// <returns>true if convex</returns>
        public static bool CheckPolygonConvex(in Polygon polygon)
        {
            return IsConvex(polygon) > 0;
        }

        #region adapted from https://gist.github.com/KvanTTT/3855122
        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        /// <param name="polygon">the polygon to check</param>
        /// <returns>-1 if no intersect 1 otherwise</returns>
        static int SelfIntersection(in IPolygon polygon)
        {
            if (polygon.VertexCount < 3)
                return 0;
            int High = polygon.VertexCount - 1;
            Vector2 O = new Vector2();
            int i;
            for (i = 0; i < High; i++)
            {
                for (int j = i + 2; j < High; j++)
                {
                    if (LineIntersect(polygon[i], polygon[i + 1],
                                      polygon[j], polygon[j + 1], ref O) == 1)
                        return 1;
                }
            }
            for (i = 1; i < High - 1; i++)
                if (LineIntersect(polygon[i], polygon[i + 1], polygon[High], polygon[0], ref O) == 1)
                    return 1;
            return -1;
        }

        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        static float Square(in IPolygon polygon)
        {
            float S = 0;
            if (polygon.VertexCount >= 3)
            {
                for (int i = 0; i < polygon.VertexCount - 1; i++)
                    S += PMSquare(polygon[i], polygon[i + 1]);
                S += PMSquare(polygon[polygon.VertexCount - 1], polygon[0]);
            }
            return S;
        }

        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        static int IsConvex(in IPolygon Polygon)
        {
            if (Polygon.VertexCount >= 3)
            {
                if (Square(Polygon) > 0)
                {
                    for (int i = 0; i < Polygon.VertexCount - 2; i++)
                        if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) < 0)
                            return -1;
                    if (PMSquare(Polygon[Polygon.VertexCount - 2], Polygon[Polygon.VertexCount - 1], Polygon[0]) < 0)
                        return -1;
                    if (PMSquare(Polygon[Polygon.VertexCount - 1], Polygon[0], Polygon[1]) < 0)
                        return -1;
                }
                else
                {
                    for (int i = 0; i < Polygon.VertexCount - 2; i++)
                        if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) > 0)
                            return -1;
                    if (PMSquare(Polygon[Polygon.VertexCount - 2], Polygon[Polygon.VertexCount - 1], Polygon[0]) > 0)
                        return -1;
                    if (PMSquare(Polygon[Polygon.VertexCount - 1], Polygon[0], Polygon[1]) > 0)
                        return -1;
                }
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        static bool Intersect(in IPolygon polygon, int vertex1Ind, int vertex2Ind, int vertex3Ind)
        {
            float s1, s2, s3;
            for (int i = 0; i < polygon.VertexCount; i++)
            {
                if ((i == vertex1Ind) || (i == vertex2Ind) || (i == vertex3Ind))
                    continue;
                s1 = PMSquare(polygon[vertex1Ind], polygon[vertex2Ind], polygon[i]);
                s2 = PMSquare(polygon[vertex2Ind], polygon[vertex3Ind], polygon[i]);
                if (((s1 < 0) && (s2 > 0)) || ((s1 > 0) && (s2 < 0)))
                    continue;
                s3 = PMSquare(polygon[vertex3Ind], polygon[vertex1Ind], polygon[i]);
                if (((s3 >= 0) && (s2 >= 0)) || ((s3 <= 0) && (s2 <= 0)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        static float PMSquare(Vector2 p1,Vector2 p2)
        {
            return (p2.X * p1.Y - p1.X * p2.Y);
        }

        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        static float PMSquare(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p3.X - p1.X) * (p2.Y - p1.Y) - (p2.X - p1.X) * (p3.Y - p1.Y);
        }

        /// <summary>
        /// adapted from https://gist.github.com/KvanTTT/3855122
        /// </summary>
        static int LineIntersect(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, ref Vector2 O)
        {
            float a1 = A2.Y - A1.Y;
            float b1 = A1.X - A2.X;
            float d1 = -a1 * A1.X - b1 * A1.Y;
            float a2 = B2.Y - B1.Y;
            float b2 = B1.X - B2.X;
            float d2 = -a2 * B1.X - b2 * B1.Y;
            float t = a2 * b1 - a1 * b2;

            if (t == 0)
                return -1;

            O.Y = (a1 * d2 - a2 * d1) / t;
            O.X = (b2 * d1 - b1 * d2) / t;

            if (A1.X > A2.X)
            {
                if ((O.X < A2.X) || (O.X > A1.X))
                    return 0;
            }
            else
            {
                if ((O.X < A1.X) || (O.X > A2.X))
                    return 0;
            }

            if (A1.Y > A2.Y)
            {
                if ((O.Y < A2.Y) || (O.Y > A1.Y))
                    return 0;
            }
            else
            {
                if ((O.Y < A1.Y) || (O.Y > A2.Y))
                    return 0;
            }

            if (B1.X > B2.X)
            {
                if ((O.X < B2.X) || (O.X > B1.X))
                    return 0;
            }
            else
            {
                if ((O.X < B1.X) || (O.X > B2.X))
                    return 0;
            }

            if (B1.Y > B2.Y)
            {
                if ((O.Y < B2.Y) || (O.Y > B1.Y))
                    return 0;
            }
            else
            {
                if ((O.Y < B1.Y) || (O.Y > B2.Y))
                    return 0;
            }

            return 1;
        }
        #endregion

        #region PointCloud

        /// <summary>
        /// http://vcg.isti.cnr.it/publications/papers/dewall.pdf
        /// </summary>
        /// <param name="points">point cloud</param>
        /// <returns>triangulation of point cloud</returns>
        public static List<int> DelauneyTriangulation(Vector2[] points)
        {
            List<int> triangles = new List<int>();

            //TODO
            

            return triangles;
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Delaunay_triangulation
        /// sssection Algorithms start
        /// </summary>
        /// <param name="t">the triangle</param>
        /// <param name="d">point we want to test if in circumcircle</param>
        /// <returns></returns>
        public static bool TriangleCircumferanceTest(Triangle t, Vector2 d)
        {
            if (t.VertexWinding != VertexWinding.CCW)
                t = ChangeOrientation(t, (mP) => new Triangle(mP[0], mP[1], mP[2]),VertexWinding.CCW);

            Vector3 c1_1 = new Vector3(t.A.X, t.B.X, t.C.X);
            Vector3 c2_1 = new Vector3(t.A.Y, t.B.Y, t.C.Y);

            Vector3 dX = new Vector3(d.X);
            Vector3 dY = new Vector3(d.Y);

            Vector3 X = c1_1 - dX;
            Vector3 Y = c2_1 - dY;

            var c3 = X * X +Y * Y;

            Matrix mat = new Matrix( new Vector4( X, 1 ), new Vector4( Y, 1 ), new Vector4( c3, 1 ), Vector4.One );

            return mat.Determinant() > 0f;
        }


        /// <summary> 
        /// Uses the Monotone Chain algorithm
        /// </summary>
        /// <param name="points"></param>
        /// <returns>convex hull</returns>
        public static IList<Vector2> ComputeConvexHull( IList<Vector2> points)
        {

            points.Sort( ( a, b ) =>
                 a.X == b.X ? a.Y.CompareTo( b.Y ) : (a.X > b.X ? 1 : -1) );

            DList<Vector2> hull = new DList<Vector2>();

            int L = 0, U = 0;


            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector2 p = points[i], p1;

                //build lower hull
                while (L >= 2 && Cross( (p1 = hull.Last) - hull[hull.Count - 2], p - p1 ) >= 0)
                {
                    hull.RemoveAt( hull.Count - 1 );
                    L--;
                }
                hull.PushLast( p );
                L++;

                //upper hull
                while (U >= 2 && Cross( (p1 = hull.First) - hull[1], p - p1 ) <= 0)
                {
                    hull.RemoveAt( 0 );
                    U--;
                }

                if (U != 0)
                    hull.PushFirst( p );
                U++;
            }
            hull.RemoveAt( hull.Count - 1 );
            return hull;

            float Cross( Vector2 a, Vector2 b )
            {
                return a.X * b.Y - a.Y * b.X;
            }
        }

        /// <summary> 
        /// Uses the Monotone Chain algorithm
        /// </summary>
        /// <param name="points"></param>
        /// <returns>convex hull</returns>
        public static IPolygon ComputeConvexHull( IList<Vector2> points, Func<IList<Vector2>, IPolygon> polyGen ) 
        {
            return polyGen( ComputeConvexHull( points ) );
        }


        #endregion
    }
}
