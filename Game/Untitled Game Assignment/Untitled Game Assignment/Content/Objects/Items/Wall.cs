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

public class Wall : GameObject
{

    public Wall( Vector2 position, float rotation )
    {
        Transform.Position = position;
        Transform.Rotation = rotation;

        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/wall", Color.White, SortingLayer.EntitesSubLayer(1), obj));

        AddComponent((obj) => new BoxCollider(spriteRen, obj, SortingLayer.Entities));

    }
}