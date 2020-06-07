using GeoUtil.HelperCollections.Grids;

public class Tile
{
    public Vector2Int TilePosition { get; private set; }

    public bool Walkable { get; private set; }

    public int WalkCost { get; private set; }

    public Tile( Vector2Int tilePosition, bool walkable, int walkCost )
    {
        TilePosition = tilePosition;
        Walkable = walkable;
        WalkCost = walkCost;
    }

    public void UpdateWalkCost( int t ) 
    {
        WalkCost = t;
    }

}
