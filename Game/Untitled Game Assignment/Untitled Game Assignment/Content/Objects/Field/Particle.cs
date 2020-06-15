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
using System.Collections.Generic;

public class Particle : GameObject
{
    public SpriteRenderer SpriteRen;

    public Particle( Vector2 position, string filePath = "Sprites/firefly" )
    {
        Transform.Position = position;

        AddComponent((obj) => new VelocityBasedRotationController(obj, Transform) );

        SpriteRen = AddComponent((obj) => new SpriteRenderer(filePath, Color.White, SortingLayer.Particles, obj));

    }
}