using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Util.CustomMath;

namespace Util.Rendering.VirtualViewports
{
    public class AspectRatioMaintainingVirtualViewport : ScalingVirtualViewport
    {
        GameWindow window;

        /// <summary>
        /// areas that can be safely cut off from edges
        /// </summary>
        int horizontalCutoff;
        /// <summary>
        /// areas that can be safely cut off from edges
        /// </summary>
        int verticalCutoff;

        public AspectRatioMaintainingVirtualViewport( GameWindow window, int width, int height, GraphicsDevice gfxD, int horizontalCutoff=0, int verticalCutoff=0):base(width,height,gfxD)
        {
            this.window = window;
            this.horizontalCutoff = horizontalCutoff;
            this.verticalCutoff = verticalCutoff;
            window.ClientSizeChanged += OnResize;
        }
        ~AspectRatioMaintainingVirtualViewport()
        {
            this.window.ClientSizeChanged -= OnResize;
        }

        public override Vector2 PointToScreen( Vector2 p) 
        {
            return Vector2.Transform( new Vector2( p.X - Viewport.X, p.Y - Viewport.Y ), InvertedScale );
        }

        public void OnResize( object obj, EventArgs args ) 
        {
            var b = window.ClientBounds;
            var newScale = new Vector2( (float)b.Width/(float)Width, (float)b.Height/(float)Height);

            var newScaleCut = new Vector2(
                (float)b.Width/(float)(Width-horizontalCutoff), (float)b.Height/(float)(Height-verticalCutoff));
        
            float actualScale = Math.Min(newScale.MaxElement(),newScaleCut.MinElement());

            int vPWidth = (int)Math.Ceiling(actualScale*(float)Width);
            int vPHeight = (int)Math.Ceiling(actualScale*(float)Height);

            int vPX = (b.Width/2)-(vPWidth/2);
            int vPY = (b.Height/2)-(vPHeight/2);
            GfxDevice.Viewport = new Viewport( vPX, vPY, vPWidth, vPHeight );
        }

    }

}
