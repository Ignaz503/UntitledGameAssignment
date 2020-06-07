using System;
using Microsoft.Xna.Framework;
/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil
{
    public struct Bounds2D
    {
        public float MinX { get; private set; }
        public float MaxX{ get; private set; } 
        
        public float MinY { get; private set; }
        public float MaxY{ get; private set; }

        public Vector2 Min => new Vector2(MinX, MinY);

        public Vector2 Max => new Vector2(MaxX, MaxY);

        public Vector2 Size { get; private set; }

        public Vector2 Center { get; private set; }

        public Vector2 Extents { get; private set; }

        public Bounds2D(float minX, float maxX, float minY, float maxY) : this()
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            Center = new Vector2(MathHelper.Lerp(minX, maxX, 0.5f), MathHelper.Lerp(minY, maxY, 0.5f));

            Size = new Vector2(MaxX - MinX, MaxY - MinY);
            Extents = Size * 0.5f;
        }

        public Bounds2D(Vector2 min, Vector2 max)
            :this(min.X,max.X,min.Y,max.Y)
        {}

        public bool Contains(Vector2 p)
        {
            return p.X >= MinX && p.Y >= MinY && p.X <= MaxX && p.Y <= MaxY;
        }

        public bool Overlap(Bounds2D other)
        {
            return Contains(other.Min) || Contains(other.Max) || Contains(other.Center)
                || Contains(new Vector2(other.Min.X, other.Max.Y)) || Contains(new Vector2(other.Max.Y, other.Min.Y)) ||
                other.Contains(Min) || other.Contains(Max) || other.Contains(Center)
                || other.Contains(new Vector2(Min.X, Max.Y)) || other.Contains(new Vector2(Max.X, Min.Y));
        }

        public Bounds2D Scale(float scale)
        {
            return new Bounds2D(Min * scale, Max * scale);
        }
    }    
}
