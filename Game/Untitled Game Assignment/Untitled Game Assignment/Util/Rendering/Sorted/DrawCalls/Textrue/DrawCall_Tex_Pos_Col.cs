using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        #region internal

        struct DrawCall_Tex_Pos_Col : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Vector2 Pos;
            Color color;

            public DrawCall_Tex_Pos_Col( SortingLayer layer, Texture2D texture, Vector2 pos, Color color )
            {
                Layer = layer;
                this.texture = texture;
                Pos = pos;
                this.color = color;
            }

            public int CompareTo( IDrawCall other )
            {
                return Layer.CompareTo( other.Layer );
            }

            public void MakeCall()
            {
               instance.InternalDraw( texture, Pos, color);
            }
        } 
        
        struct DrawCall_Tex_Pos_Col_Shader : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            Texture2D texture;
            Vector2 Pos;
            Color color;
            Effect shader;

            public DrawCall_Tex_Pos_Col_Shader( SortingLayer layer, Texture2D texture, Vector2 pos, Color color, Effect shader )
            {
                Layer = layer;
                this.texture = texture;
                Pos = pos;
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
                instance.InternalDraw( texture, Pos, color);
            }
        } 

        #endregion
    }
}
