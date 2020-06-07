using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    { 
        #region internal

        struct DrawCall_Tex_Pos_sRec_Col_Rot_Origin_Scale_Effects_Depth : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture; 
            Vector2 position; 
            Rectangle? sourceRectangle;
            Color color;
            float rotation;
            Vector2 origin; 
            float scale; 
            SpriteEffects effects; 

            public DrawCall_Tex_Pos_sRec_Col_Rot_Origin_Scale_Effects_Depth( SortingLayer layer, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects)
            {
                Layer = layer;
                this.texture = texture;
                this.position = position;
                this.sourceRectangle = sourceRectangle;
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
               instance.InternalDraw( texture, position, sourceRectangle, color,rotation, origin,scale, effects,(float)Layer);
            }
        }
        struct DrawCall_Tex_Pos_sRec_Col_Rot_Origin_Scale_Effects_Depth_Shader : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture; 
            Vector2 position; 
            Rectangle? sourceRectangle;
            Color color;
            float rotation;
            Vector2 origin; 
            float scale; 
            SpriteEffects effects; 
            Effect shader;

            public DrawCall_Tex_Pos_sRec_Col_Rot_Origin_Scale_Effects_Depth_Shader( SortingLayer layer, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects,  Effect shader )
            {
                Layer = layer;
                this.texture = texture;
                this.position = position;
                this.sourceRectangle = sourceRectangle;
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
                instance.InternalDraw( texture, position, sourceRectangle, color,rotation, origin,scale, effects,(float)Layer);
            }
        }

        #endregion
    }
}
