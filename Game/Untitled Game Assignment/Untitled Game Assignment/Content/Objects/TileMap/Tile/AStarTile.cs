using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeoUtil.HelperCollections.Grids;
using Util.DataStructure;

public class AStarTile : Tile, IHeapItem<AStarTile>
{
    public int globalCost;
    public int heuristicCost;
    public int fullCost 
    {
        get { return globalCost + heuristicCost; }
    }

    public AStarTile Parent;//for path retracing

    int hIdx;
    public int HeapIndex { get => hIdx; set => hIdx = value; }

    public AStarTile(Vector2Int tilePosition, bool walkable, int walkCost):base(tilePosition,walkable,walkCost)
    {}

    public int CompareTo( AStarTile other )
    {
        int comp = fullCost.CompareTo(other.fullCost);
        if (comp == 0)
        {
            comp = heuristicCost.CompareTo( other.heuristicCost );
        }
        return -comp;
    }
}