using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Util.Rendering.VirtualViewports
{
    public class WindowedVirtualViewport : VirtualViewport
    {

        GameWindow window;

        public WindowedVirtualViewport(GameWindow window,GraphicsDevice gfxD):base(gfxD)
        {
            this.window = window;
            this.window.ClientSizeChanged += OnResize;
        }

        ~WindowedVirtualViewport() 
        {
            this.window.ClientSizeChanged -= OnResize;
        }

        public override int Width => window.ClientBounds.Width;

        public override int Height => window.ClientBounds.Height;

        public override int ViewportWidth => Width;

        public override int ViewportHeight => Height;

        public override Matrix Scale
        {
            get { return Matrix.Identity; }
        }

        public override Matrix InvertedScale
        {
            get { return Matrix.Identity; }
        }


        void OnResize(object obj,EventArgs args) 
        {
            GfxDevice.Viewport = new Viewport( 0, 0, window.ClientBounds.Width, window.ClientBounds.Height );
        }

    }

}
