using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.Rendering;
using Util.SortingLayers;

namespace Untitled_Game_Assignment.Util.Objects
{
    public class Sprite
    {
        SortingLayer sortingLayer = 0;
        public Vector2 Position;
        public Vector2 Origin;
        public float _rotation;

        public Sprite Parent;

        private Texture2D _texture;

        /**
         * Origin set to center of sprite
         * **/
        public Sprite(Texture2D texture)
        {
            _texture = texture;
            Origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            Position = new Vector2(0, 0);
        }

        public virtual void FixedUpdate()
        {
            //Use TimeInfo isntead
        }

        public virtual void Draw()
        {
            BatchRenderer.Draw(_texture, Position, null, Color.White, _rotation, Origin, 1, SpriteEffects.None, sortingLayer);
        }

    }
}
