using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;
using Util.Rendering;
using Microsoft.Xna.Framework;

namespace UntitledGameAssignment.Core.Components
{
    public class PolygonRenderer : Component, IDraw
    {
        PolygonContainer container;

        SortingLayer Layer { get; set; }

        Texture2D texture { get; set; }

        BasicEffect shader { get; set; }

        Vector2 origin => container.Polygon.Bounds.Center;

        public PolygonRenderer( PolygonContainer container, SortingLayer layer, Texture2D texture, BasicEffect shader, GameObject obj ) : base( obj )
        {
            this.container = container;
            this.Layer = layer;
            this.texture = texture;
            this.shader = shader;
        }

        public void Draw()
        {
            
            SortedBatchRenderer.DrawPolygon( container.Mesh, texture, shader, Layer, Transform.Position, Transform.Scale, Transform.Rotation, origin, Camera.Active );
        }

        public override void OnDestroy()
        {
            //dispose of effect?
            container = null;
        }
    }
}
