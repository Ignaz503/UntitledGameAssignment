using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;
using GeoUtil.HelperCollections.Grids;
using Util.AssetManagment;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.Input;
using Util.CustomDebug;

public class TestTileMap : DrawableTileMap<WhiteTile>
{ 
    public TestTileMap( Vector2 position,int size, GameObject obj ) : base( size, size, AssetManager.Load<Texture2D>( "Sprites/WhiteSquare" ).Bounds.Size.ToVector2(), (SortingLayer)0, obj)
    {
        Transform.Position = position;        
    }

    public override void OnDestroy()
    {}

    protected override void GenerateTileAt( int x, int y )
    {
        tiles[x, y] = new WhiteTile(new Vector2Int(x,y),true,0);
    }
}
