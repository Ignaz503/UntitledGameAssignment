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
    Vector2 StoredVelocity;
    Vector2 Direction;

    float Distance;
    float Force;
    float EffectiveRadius;
    public float Mass;
    float TargetMass;

    public GravPull( GameObject obj, GameObject target, float mass = 5.0f , float effectiveRadius = 100.0f , Boolean rotate = true) : base( obj )
    {
        Target = target.Transform;
        Force = 0.0f;
        Mass = mass;
        Rotate = rotate;
        StoredVelocity = Vector2.Zero;
        EffectiveRadius = effectiveRadius;

        if (target.GetComponent<GravPull>() == null)
        {
            TargetMass = 5.0f;
        }
        else
        {
            TargetMass = target.GetComponent<GravPull>().Mass;
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

        Force = (Mass * TargetMass) / (float)Math.Pow(Distance, 2);

        //clamp max force
        if (Force >= 10.0f)
        {
            Force = 10.0f;
        }
        
        Transform.Velocity += (Direction * Force) + StoredVelocity;

        if (Rotate)
        {
            float rot = (VectorMath.Angle(Direction.X, Direction.Y) + 90.0f) / 180.0f * (float)Math.PI;
            Transform.Rotation = rot;
        }

        // store old velocity delta to add to next velocity, skews results, but keeps momentum better than setting velocity to 0 after update
        StoredVelocity = (Direction * Force) + StoredVelocity * 0.9f;
    }
}

