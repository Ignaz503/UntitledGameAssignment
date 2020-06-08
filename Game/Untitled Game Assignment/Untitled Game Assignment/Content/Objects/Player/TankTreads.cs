using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Microsoft.Xna.Framework;
using UntitledGameAssignment;
using Util.Input;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System;
using UntitledGameAssignment.Core.SceneGraph;
using Util.SortingLayers;
using System.Collections.Generic;

public class TankTreads : GameObject
{
    public SpriteRenderer SpriteRen;

    public enum tint 
    {
        white,
        gray,
        green,
        red,
        blue
    }

    public TankTreads(Vector2 position, SortingLayer layer, GameObject parent_obj, tint t = tint.white, String sprite_file = "Sprites/playerlegl") : base()
    {
        Transform.Position = position;
        Transform.Velocity = Vector2.Zero;

        Transform.Parent = parent_obj.Transform;

        SpriteRen = AddComponent((obj) => new SpriteRenderer(sprite_file, ResolveTint(t), layer-1, obj));

        AddComponent( (obj) => new VelocityBasedRotationController(obj, Transform.Parent));

        //AddComponent( ( o ) => new ChildInfoPrinter( o ) );

        Name = $"Tank Tread Player Gameobject {ID}";
    }

    /// <summary>
    /// changes tint of player
    /// </summary>
    /// <param name="t"></param>
    public void ChangeTint(Color c)
    {
        SpriteRen.Tint = c;
    }

    public void ChangeTint(tint t)
    {
        SpriteRen.Tint = ResolveTint(t);
    }

    /// <summary>
    /// returns color value based on input tint enum
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Color ResolveTint(tint t)
    {
        Color return_color = Color.White;

        switch (t)
        {
            case tint.white:
                return_color = Color.White;
                break;
            case tint.gray:
                return_color = Color.LightGray;
                break;
            case tint.green:
                return_color = Color.Green;
                break;
            case tint.red:
                return_color = Color.Red;
                break;
            case tint.blue:
                return_color = Color.Blue;
                break;
            default:
                return_color = Color.White;
                break;
        }

        return return_color;
    }
}
