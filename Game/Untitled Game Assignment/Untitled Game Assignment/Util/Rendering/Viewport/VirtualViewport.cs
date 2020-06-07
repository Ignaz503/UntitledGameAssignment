using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Util.Rendering.VirtualViewports
{
    public abstract class VirtualViewport
    {
        public GraphicsDevice GfxDevice { get; protected set; }

        public Viewport Viewport => GfxDevice.Viewport;

        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }

        public Rectangle Bounds => new Rectangle( 0, 0, Width, Height );

        public Vector2 Center => Bounds.Center.ToVector2();

        public abstract Matrix Scale { get; }

        public abstract Matrix InvertedScale { get; }

        protected VirtualViewport( GraphicsDevice gfxD ) 
        {
            this.GfxDevice = gfxD;
        }

        public virtual Vector2 PointToScreen( Vector2 p ) 
        {
            return Vector2.Transform( p, InvertedScale );
        }
    }

}
