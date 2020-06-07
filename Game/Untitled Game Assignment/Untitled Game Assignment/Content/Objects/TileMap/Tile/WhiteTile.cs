using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.AssetManagment;
using GeoUtil.HelperCollections.Grids;

public class WhiteTile : DrawableTile
{
    public WhiteTile( Vector2Int tilePosition, bool walkable, int walkCost ) : base( tilePosition, walkable, walkCost, AssetManager.Load<Texture2D>( "Sprites/WhiteSquare" ), Color.White, SpriteEffects.None, null, null )
    {}
}
