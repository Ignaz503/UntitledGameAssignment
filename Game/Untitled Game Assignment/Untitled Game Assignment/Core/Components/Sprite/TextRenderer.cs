using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;
using UntitledGameAssignment.Core.GameObjects;
using Util.Rendering;
using Util.AssetManagment;
using Util.CustomDebug;
using Microsoft.Xna.Framework.Content;

namespace UntitledGameAssignment.Core.Components
{
    public class TextRenderer : Component, IDraw
    {
        /// <summary>
        /// the sorting layer of this sprite drawer
        /// </summary>
        public SortingLayer Layer;
        /// <summary>
        /// the text to draw
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// the tint of the sprite
        /// </summary>
        public Color Tint { get; set; }

        public SpriteEffects Effects { get; set; }

        public Effect Shader { get; set; }

        public SpriteFont Font { get; set; }

        public Vector2 UnscaledTextSize => Font.MeasureString( Text );
        public Vector2 ScaledTextSize => Font.MeasureString( Text ) * Transform.Scale;

        public ContentManager Content { get; set; } //TODO: not call content again, this is just for testing

        public TextRenderer( string text, Color tint, SortingLayer layer, GameObject obj):base(obj)
        {
            this.Layer = layer;
            this.Text = text;
            this.Tint = tint;
            Effects = SpriteEffects.None;
            Shader = null;

            Font = AssetManager.Load<SpriteFont>("Arial");
        }

        public TextRenderer( string text, Color tint, SortingLayer layer, SpriteEffects effect, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Text = text;
            this.Tint = tint;
            Effects = effect;
            Shader = null;

            Font = AssetManager.Load<SpriteFont>("Arial");
        }

        public TextRenderer( string text, Color tint, SortingLayer layer, Effect shader, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Text = text;
            this.Tint = tint;
            Effects = SpriteEffects.None;
            Shader = shader;

            Font = AssetManager.Load<SpriteFont>("Arial");
        }

        public TextRenderer( string text, Color tint, SortingLayer layer, SpriteEffects effect, Effect shader, GameObject obj ) : base( obj )
        {
            this.Layer = layer;
            this.Text = text;
            this.Tint = tint;
            Effects = effect;
            Shader = shader;

            Font = AssetManager.Load<SpriteFont>("Arial");
        }
        
        public void Draw()
        {
            //Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth = 0f, Effect effect
            //BatchRenderer.DrawString(Font, Text, Transform.Position , Tint, Transform.Rotation, Transform.Position, Transform.Scale, Effects, (float)Layer);
            SortedBatchRenderer.DrawString(Font, Text, Transform.Position , Tint, Transform.Rotation, UnscaledTextSize*0.5f, Transform.Scale, Effects, Layer);
        }

        public override void OnDestroy()
        {}

        public virtual void DoShaderSetup() 
        { }

    }
}
