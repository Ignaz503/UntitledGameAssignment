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

public class TempPlayer : GameObject
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

    public TempPlayer(Vector2 position, Func<GameObject, MovementController> mC, SortingLayer layer, tint t = tint.white, String sprite_file = "Sprites/playershoulders", Keys increase = Keys.Q,Keys decrease = Keys.E, string name = null) :base()
    {
        Transform.Position = position;

        Transform.Velocity = Vector2.Zero;

        SpriteRen = AddComponent( (obj) => new SpriteRenderer( sprite_file, ResolveTint(t), layer, obj) );

        if(mC != null)
            AddComponent(mC);

        if (name == null)
            Name = $"Temporary Player Gameobject {ID}";
        else
            Name = name;
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

public class ChildInfoPrinter : Component, IUpdate
{
    public ChildInfoPrinter( GameObject obj ) : base( obj )
    {
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        if (Input.IsKeyDown(Keys.F))
        {
            Debug.Log($"Name: {GameObject.Name}, child count: {Transform.ChildCount}");
        }
    }
}
