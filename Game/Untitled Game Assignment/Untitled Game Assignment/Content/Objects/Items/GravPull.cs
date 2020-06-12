using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using UntitledGameAssignment.Core;
using Util.CustomMath;

public class GravPull : Component, IUpdate
{
    Boolean Rotate;
    Transform Target;
    Vector2 Direction;
    RigidBody2D Rb;
    RigidBody2D TargetRb;

    float Force;
    float Distance;
    float EffectiveRadius;

    public GravPull( GameObject obj, GameObject target, float effectiveRadius = 100.0f , Boolean rotate = true) : base( obj )
    {
        Rb = this.GameObject.GetComponent<RigidBody2D>();
        TargetRb = target.GetComponent<RigidBody2D>();

        Target = target.Transform;
        Rotate = rotate;

        EffectiveRadius = effectiveRadius;

        if (Rb == null || TargetRb == null)
        {
            Debug.Log("GravPull without RigidBody");
            this.Disable();
        }
    }
     
    public override void OnDestroy()
    {}

    public void Update()
    {
        Attract();
    }

    /// <summary>
    /// mostly accurate Newtonian gravity calculation
    /// </summary>
    public void Attract()
    {
        if (Target.Position == Transform.Position)
            return;

        Direction =  Target.Position - Transform.Position;
        Distance = Direction.Length();

        if ( Distance > EffectiveRadius)
            return;

        Force = (Rb.Mass * TargetRb.Mass) / (float)Math.Pow(Distance, 2);

        Direction.Normalize();
        Rb.AddImpulse(Direction, Force);
        

        if (Rotate)
        {
            float rot = (VectorMath.Angle(Direction.X, Direction.Y) + 90.0f) / 180.0f * (float)Math.PI;
            Transform.RotationVelocity += -(Transform.Rotation + rot - 4.0f);
        }
    }
}

