using Microsoft.Xna.Framework;
using GeoUtil.HelperCollections.Grids;
using UntitledGameAssignment.Core.GameObjects;

public class AStarTileMap : TileMap<AStarTile>
{
    public AStarTileMap( int width, int height, Vector2 tileSize, GameObject obj, Neighborhood neighborCalc = Neighborhood.Cross ) : base( width, height, tileSize, obj, neighborCalc )
    { }

    public AStarTileMap( GameObject j ) : base( 20, 20, Vector2.One * 50f, j )
    { }

    public override void OnDestroy()
    {
        tiles = null;
    }

    protected override void GenerateTileAt( int x, int y )
    {
        //TODO load from map or something
        tiles[x, y] = new AStarTile( new Vector2Int( x, y ), true, 1 );
    }

    public static AStarTileMap CreateAStarMap<T>(TileMap<T> map, GameObject obj) where T: Tile
    {
        return obj.AddComponent( j => new AStarTileMap( map.Width, map.Height,map.TileSize, j, map.NeighborhoodCalculationType ) );
    }    
    
    public static AStarTileMap CreateAStarMap<T>(TileMap<T> map) where T: Tile
    {
        return map.GameObject.AddComponent( j => new AStarTileMap( map.Width, map.Height,map.TileSize, j, map.NeighborhoodCalculationType ) );
    }

}
