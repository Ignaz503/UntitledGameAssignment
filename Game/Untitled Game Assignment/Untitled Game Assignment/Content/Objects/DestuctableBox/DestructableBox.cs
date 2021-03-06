﻿using Microsoft.Xna.Framework;
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
    public DestructableBox(Texture2D sprite, Vector2 position, DissipateInfo info):base()
    {
        spriteRendrer = AddComponent(j => new SpriteRenderer( sprite, Color.White, SortingLayer.EntitesSubLayer(2), j) );
        collider = AddComponent(j => new BoxCollider(spriteRendrer,j, SortingLayer.EntitesSubLayer( 2 ) ) );
        AddComponent( ( obj ) => { return new Destructable( collider, spriteRendrer, info, obj ); } );
        Transform.Position = position;
    }
}

