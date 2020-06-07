using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Util.AssetManagment
{
    public static class AssetManager
    {
        static ContentManager contentManager;

        public static T Load<T>( string assetPath ) 
        {
            return contentManager.Load<T>( assetPath );
        }

        public static T LoadLocalized<T>( string assetPath ) 
        {
            return contentManager.LoadLocalized<T>( assetPath );
        }

        public static void Unload() 
        {
            contentManager.Unload();
        }

        internal static void Initialize( ContentManager manager ) 
        {
            contentManager = manager;
        }
    }
}
