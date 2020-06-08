using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Util.CustomDebug;
using Util.CustomMath;
using Microsoft.Xna.Framework.Input;
using UntitledGameAssignment.Core;

public class VelocityBasedRotationController : Component, IUpdate
{
    float rot;
    Transform Track;

    public VelocityBasedRotationController( GameObject obj, Transform track ) : base( obj )
    {
        Track = track;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        if (Track.Velocity != Vector2.Zero)
            Rotate();
    }

    private void Rotate()
    {
        Vector2 dir = Track.Velocity;
        dir.Normalize();

        rot = (VectorMath.Angle(dir.X, dir.Y) + 90.0f) / 180.0f * (float)Math.PI;
        Transform.Rotation = rot;
    }
}

