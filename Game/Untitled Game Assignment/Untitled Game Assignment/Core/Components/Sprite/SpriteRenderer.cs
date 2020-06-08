using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;
using UntitledGameAssignment.Core.GameObjects;
using Util.Rendering;
using Util.AssetManagment;
using Util.CustomDebug;

namespace UntitledGameAssignment.Core.Components
{
    public class SpriteRenderer : Component, IDraw
    {
        /// <summary>
        /// the sorting layer of this sprite drawer
        /// </summary>
        public SortingLayer Layer;
        /// <summary>
        /// the sprite to draw
        /// </summary>
        public Texture2D Sprite { get; set; }
        /// <summary>
        /// the tint of the sprite
        /// </summary>
        public Color Tint { get; set; }

        public SpriteEffects Effects { get; set; }

        public Effect Shader { get; set; }

        Vector2 origin => new Vector2( Sprite.Width / 2.0f, Sprite.Height / 2.0f );

        public SpriteRenderer(Texture2D sprite, Color tint, SortingLayer layer, GameObject obj):base(obj)
        {
            this.Layer = layer;
            this.Sprite = sprite;
            this.Tint = tint;
            Effects = SpriteEffects.None;
            Shader = null;
        }

        public SpriteRenderer( string spritePath, Color tint, SortingLayer layer, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = AssetManager.Load<Texture2D>( spritePath );
            this.Tint = tint;
            Effects = SpriteEffects.None;
            Shader = null;
        }

        public SpriteRenderer( Texture2D sprite, Color tint, SortingLayer layer, SpriteEffects effect, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = sprite;
            this.Tint = tint;
            Effects = effect;
            Shader = null;
        }

        public SpriteRenderer( string spritePath, Color tint, SortingLayer layer, SpriteEffects effects, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = AssetManager.Load<Texture2D>( spritePath );
            this.Tint = tint;
            this.Effects = effects;
            this.Shader = null;
        }

        public SpriteRenderer( Texture2D sprite, Color tint, SortingLayer layer, Effect shader, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = sprite;
            this.Tint = tint;
            Effects = SpriteEffects.None;
            Shader = shader;
        }

        public SpriteRenderer( string spritePath, Color tint, SortingLayer layer, Effect shader, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = AssetManager.Load<Texture2D>( spritePath );
            this.Tint = tint;
            this.Effects = SpriteEffects.None;
            Shader = shader;
        }

        public SpriteRenderer( Texture2D sprite, Color tint, SortingLayer layer, SpriteEffects effect, Effect shader, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = sprite;
            this.Tint = tint;
            Effects = effect;
            Shader = shader;
        }
        public SpriteRenderer( string spritePath, Color tint, SortingLayer layer, SpriteEffects effect, Effect shader, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Sprite = AssetManager.Load<Texture2D>( spritePath );
            this.Tint = tint;
            Effects = effect;
            Shader = shader;
        } 
        
        public void Draw()
        {
            if (Shader != null)
            {
                DoShaderSetup();
                //Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer, Effect effect
                //BatchRenderer.Draw( Sprite, Transform.Position, null, Tint, Transform.Rotation, origin, Transform.Scale, Effects, (float)Layer, Shader );
                SortedBatchRenderer.Draw( Sprite, Transform.Position, null, Tint, Transform.Rotation, origin, Transform.Scale, Effects, Layer, Shader );
            } else
            {
                //Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth = 0f
                //BatchRenderer.Draw( Sprite,  Transform.Position, null, Tint, Transform.Rotation, origin, Transform.Scale, Effects, (float)Layer );
                SortedBatchRenderer.Draw( Sprite,  Transform.Position, null, Tint, Transform.Rotation, origin, Transform.Scale, Effects, Layer );
            }
        }

        public override void OnDestroy()
        {}

        public virtual void DoShaderSetup() 
        { }

        public void changeSprite(string spritePath)
        {
            this.Sprite = AssetManager.Load<Texture2D>(spritePath);
        }
        public void changeSprite(Texture2D sprite)
        {
            this.Sprite = sprite;
        }
    }
}
