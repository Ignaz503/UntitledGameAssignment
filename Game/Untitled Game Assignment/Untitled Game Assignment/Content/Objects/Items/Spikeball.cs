using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Microsoft.Xna.Framework;
using UntitledGameAssignment;
using Util.Input;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System;
using Util.CustomMath;
using Util.SortingLayers;

public class Spikeball : GameObject
{

    public Spikeball( Vector2 position )
    {
        Transform.Position = position;

        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/spikeball", Color.White, SortingLayer.EntitesSubLayer(1), obj));

        AddComponent((obj) => new BoxCollider(spriteRen, obj, SortingLayer.Entities, false));
        AddComponent((obj) => new RigidBody2D(obj, 1.2f));

    }
}