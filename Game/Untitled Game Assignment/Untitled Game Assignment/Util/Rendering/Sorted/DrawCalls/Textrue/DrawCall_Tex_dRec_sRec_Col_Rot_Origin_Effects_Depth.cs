using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        #region internal

        struct DrawCall_Tex_dRec_sRec_Col_Rot_Origin_Effects_Depth : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Rectangle destinationRectangle; 
            Rectangle? sourceRectangle;
            Color color;
            float rotation;
            Vector2 origin; 
            SpriteEffects effects; 

            public DrawCall_Tex_dRec_sRec_Col_Rot_Origin_Effects_Depth( SortingLayer layer, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects )
            {
                Layer = layer;
                this.texture = texture;
                this.destinationRectangle = destinationRectangle;
                this.sourceRectangle = sourceRectangle;
                this.color = color;
                this.rotation = rotation;
                this.origin = origin;
                this.effects = effects;
            }

            public int CompareTo( IDrawCall other )
            {
                return Layer.CompareTo( other.Layer );
            }

            public void MakeCall()
            {
               instance.InternalDraw( texture, destinationRectangle,sourceRectangle, color,rotation,origin, effects,(float)Layer);
            }
        }
        
        struct DrawCall_Tex_dRec_sRec_Col_Rot_Origin_Effects_Depth_Shader : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Rectangle destinationRectangle; 
            Rectangle? sourceRectangle;
            Color color;
            float rotation;
            Vector2 origin; 
            SpriteEffects effects; 
            Effect shader;

            public DrawCall_Tex_dRec_sRec_Col_Rot_Origin_Effects_Depth_Shader( SortingLayer layer, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, Effect shader )
            {
                Layer = layer;
                this.texture = texture;
                this.destinationRectangle = destinationRectangle;
                this.sourceRectangle = sourceRectangle;
                this.color = color;
                this.rotation = rotation;
                this.origin = origin;
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
                instance.InternalDraw( texture, destinationRectangle,sourceRectangle, color,rotation,origin, effects,(float)Layer);
            }
        }

        #endregion
    }
}
