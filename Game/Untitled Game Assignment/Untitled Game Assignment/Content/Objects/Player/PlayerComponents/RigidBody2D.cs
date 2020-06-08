using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;
using Util.Input;

public class RigidBody2D : Component, IUpdate
{
    // The higher the mass, the faster the impulse decay
    public float Mass = 10.0f;
    
    public Vector2 Impulse;

    public RigidBody2D( GameObject obj, float mass ) : base( obj )
    {
        Mass = Math.Max(mass, 1.0f);
        Impulse = Vector2.Zero;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        Transform.Velocity += Impulse;
        // Impulse decay
        Impulse *= (1.0f / Mass) * 0.999f;
    }

    public void AddImpulse(Vector2 direction, float force)
    {
        Impulse += direction * force;
    }

    public float Energy()
    {
        float imp = Transform.Velocity.Length() * Transform.Velocity.Length() * Mass; //E=mc^2
        return imp * 0.5f; //scale back
    }
}

