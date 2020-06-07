using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Util.Rendering.VirtualViewports
{
    public class UnchangingVirtualViewport : VirtualViewport
    {
        public UnchangingVirtualViewport( GraphicsDevice gfxD ) : base( gfxD )
        {}
        
        public override int Width => ViewportWidth;

        public override int Height => ViewportHeight;

        public override int ViewportWidth => Viewport.Width;

        public override int ViewportHeight => Viewport.Height;

        public override Matrix Scale
        {
            get { return Matrix.Identity; }
        }

        public override Matrix InvertedScale
        {
            get { return Matrix.Identity; }
            
        }

    }
}
