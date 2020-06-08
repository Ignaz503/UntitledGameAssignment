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

public class VectorField : GameObject
{

    public VectorField( Vector2 position )
    {
        Transform.Position = position;

        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/swirl", Color.White, SortingLayer.EntitesSubLayer(1), obj));

        AddComponent((obj) => new ConcentricField(obj, new Vector2(0.0f, 0.0f), 10.0f, 200.0f));
    }
}