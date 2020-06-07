using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace Util.Rendering.VirtualViewports
{
    public class ScalingVirtualViewport : VirtualViewport
    {
        public ScalingVirtualViewport(int width, int height, GraphicsDevice gfxD):base(gfxD)
        {
            this.Width = width;
            this.Height = height;
        }

        public override int Width { get; }

        public override int Height { get; }

        public override int ViewportWidth => Viewport.Width;

        public override int ViewportHeight => Viewport.Height;

        float scaleW => (float)ViewportWidth / (float)Width;
        float scaleH => (float)ViewportHeight / Height;

        public override Matrix Scale
        {
            get { return Matrix.CreateScale( scaleW, scaleH, 1f ); }
        }

        public override Matrix InvertedScale
        {
            get { return Matrix.CreateScale( 1f / scaleW, 1f / scaleH, 1f ); }
        }

    }

}
