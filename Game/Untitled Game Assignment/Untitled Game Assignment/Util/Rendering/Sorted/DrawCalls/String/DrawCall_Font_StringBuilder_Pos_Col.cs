using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        struct DrawCall_Font_StringBuilder_Pos_Col : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            SpriteFont spriteFont;
            StringBuilder builder; 
            Vector2 position; 
            Color color;

            public DrawCall_Font_StringBuilder_Pos_Col( SortingLayer layer, SpriteFont spriteFont, StringBuilder builder, Vector2 position, Color color )
            {
                Layer = layer;
                this.spriteFont = spriteFont;
                this.builder = builder;
                this.position = position;
                this.color = color;
            }

            public int CompareTo( IDrawCall other )
            {
                return Layer.CompareTo( other.Layer );
            }

            public void MakeCall()
            {
                instance.InternalDrawString( spriteFont, builder, position, color );
            }
        }
        struct DrawCall_Font_StringBuilder_Pos_Col_Shader : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            SpriteFont spriteFont;
            StringBuilder builder; 
            Vector2 position; 
            Color color;
            Effect shader;

            public DrawCall_Font_StringBuilder_Pos_Col_Shader( SortingLayer layer, SpriteFont spriteFont, StringBuilder builder, Vector2 position, Color color, Effect shader )
            {
                Layer = layer;
                this.spriteFont = spriteFont;
                this.builder = builder;
                this.position = position;
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
                instance.InternalDrawString( spriteFont, builder, position, color );
            }
        }

    }
}
