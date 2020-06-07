using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        #region internal

        struct DrawCall_Tex_Pos_sRec_Col_Rot_Origin_ScaleVec_Effects_Depth : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Vector2 position; 
            Rectangle? sourceRectangle;
            Color color;
            float rotation;
            Vector2 origin;
            Vector2 scale; 
            SpriteEffects effects; 

            public DrawCall_Tex_Pos_sRec_Col_Rot_Origin_ScaleVec_Effects_Depth( SortingLayer layer, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects )
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
        #endregion
    }
}
