using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        #region internal

        struct DrawCall_Tex_Rec_Col : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Rectangle rect;
            Color color;

            public DrawCall_Tex_Rec_Col( SortingLayer layer, Texture2D texture, Rectangle rect, Color color )
            {
                Layer = layer;
                this.texture = texture;
                this.rect = rect;
                this.color = color;
            }

            public int CompareTo( IDrawCall other )
            {
                return Layer.CompareTo( other.Layer );
            }

            public void MakeCall()
            {
               instance.InternalDraw( texture, rect, color);
            }
        }
        struct DrawCall_Tex_Rec_Col_Shader : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Rectangle rect;
            Color color;
            Effect shader;

            public DrawCall_Tex_Rec_Col_Shader( SortingLayer layer, Texture2D texture, Rectangle rect, Color color, Effect shader )
            {
                Layer = layer;
                this.texture = texture;
                this.rect = rect;
                this.color = color;
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
                instance.InternalDraw( texture, rect, color);
            }
        }

        #endregion
    }
}
