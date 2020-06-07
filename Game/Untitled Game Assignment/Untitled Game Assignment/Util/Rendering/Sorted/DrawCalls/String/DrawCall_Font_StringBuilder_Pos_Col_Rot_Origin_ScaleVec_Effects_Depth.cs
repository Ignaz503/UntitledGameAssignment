using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        
        struct DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            SpriteFont spriteFont;
            StringBuilder builder;
            Vector2 position;
            Color color; 
            float rotation;
            Vector2 origin; 
            Vector2 scale;
            SpriteEffects effects;

            public DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth( SortingLayer layer, SpriteFont spriteFont, StringBuilder builder, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
            {
                Layer = layer;
                this.spriteFont = spriteFont;
                this.builder = builder;
                this.position = position;
                this.color = color;
                this.rotation = rotation;
                this.origin = origin;
                this.scale = scale;
                this.effects = effects;
            }

            public int CompareTo( IDrawCall other )
            {
                return Layer.CompareTo( other.Layer );
            }

            public void MakeCall()
            {
                instance.InternalDrawString( spriteFont, builder, position, color,rotation,origin,scale,effects,(float)Layer );
            }
        }
        
        struct DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth_Shader : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            SpriteFont spriteFont;
            StringBuilder builder;
            Vector2 position;
            Color color; 
            float rotation;
            Vector2 origin; 
            Vector2 scale;
            SpriteEffects effects;
            Effect shader;

            public DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth_Shader( SortingLayer layer, SpriteFont spriteFont, StringBuilder builder, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects,  Effect shader )
            {
                Layer = layer;
                this.spriteFont = spriteFont;
                this.builder = builder;
                this.position = position;
                this.color = color;
                this.rotation = rotation;
                this.origin = origin;
                this.scale = scale;
                this.effects = effects;
                this.shader = shader;
            }

            public int CompareTo( IDrawCall other )
            {
                if (Layer < other.Layer)
                    return -1;
                else if (Layer > other.Layer)
                    return 1;
                return 0;
            }

            public void MakeCall()
            {
                shader.CurrentTechnique.Passes[0].Apply();
                instance.InternalDrawString( spriteFont, builder, position, color,rotation,origin,scale,effects,(float)Layer );
            }
        }

    }
}
