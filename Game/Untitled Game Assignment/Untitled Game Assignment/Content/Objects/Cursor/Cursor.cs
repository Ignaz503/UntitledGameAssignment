using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Microsoft.Xna.Framework;
using UntitledGameAssignment;
using Util.Input;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System;

public class Cursor : GameObject
{
    public Cursor()
    {
        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/cursor", Color.White, 1, obj));
        AddComponent((obj) => new MouseLocationBasedPositionController(obj));
    }
}