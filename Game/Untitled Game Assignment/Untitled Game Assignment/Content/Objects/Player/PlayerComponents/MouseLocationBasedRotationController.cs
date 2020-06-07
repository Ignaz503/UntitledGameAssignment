using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Util.CustomDebug;
using Util.CustomMath;
using Microsoft.Xna.Framework.Input;

public class MouseLocationBasedRotationController : Component, IUpdate
{
    Vector2 Target;
    float rot;

    public MouseLocationBasedRotationController( GameObject obj ) : base( obj )
    {}

    public override void OnDestroy()
    {}

    public void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        Target = Camera.Active.ScreenToWorld(Input.MousePosition);

        Vector2 dir = (Target - Transform.Position);
        dir.Normalize();

        rot = (VectorMath.Angle(dir.X, dir.Y) + 90.0f) / 180.0f * (float)Math.PI;
        Transform.Rotation = rot;

        //Debug.Log(dir.X + ", " + dir.Y + ": " + rot);
    }
}

