using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GeoUtil.Polygons;

namespace GeoUtil.HelperCollections.Grids
{
    public struct IndexedVertex 
    {
        public Vector2 v;
        public int idx;

        public IndexedVertex( Vector2 v, int idx )
        {
            this.v = v;
            this.idx = idx;
        }
    }

    public class PolygonVertexIDXCell : Cell<Vector2, int>
    {
        const int ERROR_IDX = -1;
        public const float DistErrorRad = 0.0001f;
        List<IndexedVertex> verticesInCell;
        float distanceErrorRadius;

        public PolygonVertexIDXCell(List<IndexedVertex> verticesInCell, float distanceErrorRadius)
        {
            this.verticesInCell = verticesInCell ?? throw new ArgumentNullException(nameof(verticesInCell));
            this.distanceErrorRadius = distanceErrorRadius;
        }

        public PolygonVertexIDXCell(float distanceErrorRadius = DistErrorRad)
        {
            verticesInCell = new List<IndexedVertex>();
            this.distanceErrorRadius = distanceErrorRadius;
        }

        public PolygonVertexIDXCell(Vector2 firstVert, int firstIDX, float distanceErrorRadius = DistErrorRad) : this(distanceErrorRadius)
        {
            Add(firstVert, firstIDX);
        }

        public override int GetValue(Vector2 _in)
        {
            return FindIDX(_in);
        }

        int FindIDX(Vector2 v)
        {
            for (int i = 0; i < verticesInCell.Count; i++)
            {
                var item = verticesInCell[i];
                if (Vector2.DistanceSquared(item.v, v) <= distanceErrorRadius)
                    return item.idx;
            }
            return ERROR_IDX;
        }

        public void Add(Vector2 vert, int idx)
        {
            verticesInCell.Add(new IndexedVertex(vert, idx));
        }
    }

    public class PolygonVertexIDXHelperGrid : RebasingHelperGrid<PolygonVertexIDXCell, int>
    {
        IPolygon poly;

        Dictionary<Vector2Int, PolygonVertexIDXCell> cells;

        public PolygonVertexIDXHelperGrid(IPolygon p, float resolution) : base(p.Bounds.Min,resolution)
        {
            poly = p;
            cells = new Dictionary<Vector2Int, PolygonVertexIDXCell>();
            BuildGrid();
        }

        private void BuildGrid()
        {
            for (int i = 0; i < poly.VertexCount; i++)
            {
                var vert = poly[i];
                var gridIDX = GetCellPosition(vert);
                if (cells.ContainsKey(gridIDX))
                {
                    cells[gridIDX].Add(vert, i);
                }
                else
                {
                    cells.Add(gridIDX, new PolygonVertexIDXCell(vert, i));
                }
            }
        }

        public override PolygonVertexIDXCell GetCell(Vector2 _in)
        {
            return cells[GetCellPosition(_in)];
        }

        public override int GetValue(Vector2 _in)
        {
            return GetCell(_in).GetValue(_in);
        }

    }
}
