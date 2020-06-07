using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.SortingLayers;

public class DestructableBox : GameObject
{
    SpriteRenderer spriteRendrer;
    BoxCollider collider;
    public DestructableBox(Texture2D sprite, Vector2 position, TempPlayer player):base()
    {
        System.Diagnostics.Debug.WriteLine( "Hello" );
        spriteRendrer = AddComponent(j => new SpriteRenderer( sprite, Color.White, 1, j) );
        collider = AddComponent(j => new BoxCollider(spriteRendrer,j));
        AddComponent( ( obj ) => { return new Destructable( collider, spriteRendrer, player, obj ); } );
        Transform.Position = position;
    }
}

