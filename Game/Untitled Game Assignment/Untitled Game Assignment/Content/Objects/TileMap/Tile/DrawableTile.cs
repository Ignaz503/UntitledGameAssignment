using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.Rendering;
using Util.SortingLayers;
using GeoUtil.HelperCollections.Grids;

public abstract class DrawableTile : Tile
{

    public Texture2D TileSprite { get; protected set; }
    public Color Tint { get; set; }

    public SpriteEffects Effects { get; protected set; }

    public Effect Shader { get; protected set; }

    Action<Effect> shaderSetupFunc;

    public bool HasShader => Shader != null;

    Vector2 origin => new Vector2( (float)TileSprite.Width / (float)2, (float)TileSprite.Height / (float)2 );

    public int TileSpriteWidth => TileSprite.Width;
    public int TileSpriteHeight => TileSprite.Height;

    public DrawableTile( Vector2Int tilePosition, bool walkable, int walkCost, Texture2D tileSprite, Color tint, SpriteEffects effects, Effect shader, Action<Effect> shaderSetupFunc ):base( tilePosition, walkable, walkCost)
    {
        TileSprite = tileSprite ?? throw new ArgumentNullException( nameof( TileSprite ) );
        Tint = tint;
        Effects = effects;
        Shader = shader;
        this.shaderSetupFunc = shaderSetupFunc;
    }

    public void Draw( Vector2 position, float rotation, Vector2 scale, SortingLayer layer )
    {
        if (HasShader)
        {
            shaderSetupFunc?.Invoke( Shader );
            //BatchRenderer.Draw( TileSprite, position, null, Tint, rotation, origin, scale, Effects, (float)layer, Shader );
            SortedBatchRenderer.Draw( TileSprite, position, null, Tint, rotation, origin, scale, Effects, layer, Shader );
        } else
        {
            //BatchRenderer.Draw( TileSprite, position, null, Tint, rotation, origin, scale, Effects, (float)layer );
            SortedBatchRenderer.Draw( TileSprite, position, null, Tint, rotation, origin, scale, Effects, layer );
        }
    }

    public void SetShader( Effect effect, Action<Effect> shaderSetup )
    {
        Shader = effect;
        shaderSetupFunc = shaderSetup;
    }
}