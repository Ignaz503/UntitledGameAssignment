﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GeoUtil.Vertex;
/// <summary>
/// some basic geometry utitltiy
/// polygon inclusion testing from: http://geomalgorithms.com/a03-_inclusion.html
/// triangulation https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
/// </summary>
namespace GeoUtil.Polygons
{
    public interface IPolygon
    {
        Vector2 this[int i]
        {
            get;
        }

        int VertexCount { get; }

        Bounds2D Bounds { get; }

        VertexWinding VertexWinding { get; }
    }
}


