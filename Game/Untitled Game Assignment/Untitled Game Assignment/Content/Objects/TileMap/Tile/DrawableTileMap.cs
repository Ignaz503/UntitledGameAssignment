using Microsoft.Xna.Framework;
using Util.SortingLayers;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Util.CustomDebug;

public abstract class DrawableTileMap<T> : TileMap<T>, IDraw
    where T : DrawableTile
{
    public SortingLayer Layer { get; protected set; }
    
    public DrawableTileMap( int width, int height, Vector2 tileSpriteSize, SortingLayer layer, GameObject obj ) : base( width, height, tileSpriteSize, obj )
    {
        Layer = layer;
    }

    public void Draw()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var tile = tiles[x,y];
                tile.Draw( TilePositionToWorld( x, y ), Transform.Rotation, Transform.Scale, Layer );
            }
        }
    }
} 
