using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using Util.CustomMath;

public class RectField : Component, IUpdate
{
    Rectangle Rect;
    float Multiplier;

    public RectField( GameObject obj, Rectangle rect, float multiplier ) : base( obj )
    {
        Rect = rect;
        Multiplier = multiplier;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
    }

    Vector2 CalcField(Vector2 position)
    {
        return new Vector2(CalcI(position), CalcJ(position));
    }

    float CalcI(Vector2 position)
    {
        float i = position.Y / (position.X * position.X + position.Y * position.Y);
        return i;
    }

    float CalcJ(Vector2 position)
    {
        float j = - position.X / (position.X * position.X + position.Y * position.Y);
        return j;
    }

    void EnactForce()
    {
    }

}

