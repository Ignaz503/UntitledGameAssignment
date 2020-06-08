using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;

public class VectorField : Component, IUpdate
{
    Vector2 Pos;
    RigidBody2D Rb;

    public VectorField( GameObject obj) : base( obj )
    {
        Rb = this.GameObject.GetComponent<RigidBody2D>();
        if (Rb == null)
        {
            this.Destroy();
        }
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        Pos = Transform.Position;
        EnactForce();
    }

    Vector2 CalcField()
    {
        Vector2 Force = new Vector2(-Pos.Y, Pos.X);
        Force.Normalize();
        
        return Force;
    }

    void EnactForce()
    {
        Debug.Log("P: " + Pos + ", F: " + CalcField());
        Rb.AddImpulse(CalcField(), 0.5f);
    }

}

