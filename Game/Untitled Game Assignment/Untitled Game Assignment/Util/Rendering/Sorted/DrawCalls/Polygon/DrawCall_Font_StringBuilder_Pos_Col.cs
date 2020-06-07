using Loyc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using UntitledGameAssignment.Core.Components;
using Util.SortingLayers;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        //VertexPositionTexture[] mesh, Texture2D texture, BasicEffect shader, SortingLayer layer, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Camera cam
        struct DrawCall_Mesh_Texture_Shader_Pos_Scale_Rot_Origin_Cam : IDrawCall
        {
            public SortingLayer Layer { get; private set; }

            VertexPositionTexture[] mesh;
            Texture2D texture;
            BasicEffect shader;  
            Vector2 position;
            Vector2 scale;
            float rotation;
            Vector2 origin;
            Camera cam;

            public DrawCall_Mesh_Texture_Shader_Pos_Scale_Rot_Origin_Cam( SortingLayer layer, VertexPositionTexture[] mesh, Texture2D texture, BasicEffect shader,  Vector2 position, Vector2 scale, float rotation, Vector2 origin, Camera cam )
            {
                Layer = layer;
                this.mesh = mesh;
                this.texture = texture;
                this.shader = shader;
                this.position = position;
                this.scale = scale;
                this.rotation = rotation;
                this.origin = origin;
                this.cam = cam;
            }

            public int CompareTo( IDrawCall other )
            {
                return Layer.CompareTo( other.Layer );
            }

            public void MakeCall()
            {
                instance.InternalDrawPolygon( mesh, texture, shader, Layer, position, scale, rotation, origin, cam );
            }
        }
    }
}
