using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;
using Util.CustomMath;
using Util.FrameTimeInfo;
using Util.Input;
using Util.SortingLayers;

public class RigidBody2D : Component, IUpdate
{
    // The higher the mass, the faster the impulse decay
    public float Mass = 10.0f;
    public float Drag;

    //linear momentum, rate of velocity change
    public Vector2 Impulse;
    public Vector2 ForceImpulse;

    //angular velocity, rate of rotation change
    public float Torque;
    public bool IsKinematic { get; private set; }

    public RigidBody2D( GameObject obj, float mass, float drag = 0.9f, bool isKinematic = true ) : base( obj )
    {
        Drag = drag;
        Mass = Math.Max(mass, 1.0f);
        Impulse = Vector2.Zero;
        ForceImpulse = Vector2.Zero;
        Torque = 0.0f;
        IsKinematic = isKinematic;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        if (ForceImpulse != Vector2.Zero)
        {
            Impulse = ForceImpulse;
            ForceImpulse = Vector2.Zero;
        }

        Transform.Velocity += Impulse;
        // Impulse decay
        Impulse *= (Drag / Mass);

        Transform.RotationVelocity += Torque;
        // Torque decay
        Torque *= ((Drag / Mass) * 0.1f);
    }

    public void AddImpulse(Vector2 direction, float force)
    {
        Impulse += direction * force;
    }

    public void OverrideImpulse(Vector2 direction, float force)
    {
        ForceImpulse = direction * force;
    }

    public void AddTorque(Vector2 position, Vector2 direction, float force)
    {
        Vector2 len = position - GameObject.GetComponent<BoxCollider>().BoundingBox.Center.ToVector2();
        //float diff = (VectorMath.Angle(dir.X, dir.Y) + 90.0f) / 180.0f * (float)Math.PI;
        //float diff = (float)Math.Atan((double)(direction.Length() / len.Length()));
        float diff = VectorMath.DiffAngle(len.X, len.Y, direction.X, direction.Y);
        diff = diff * 4.0f / 180.0f;

        Torque += diff * Mass * force;

        //Debug.Log("hitting at " + len + " with force of " + direction + " yields " + diff);
    }

    public void Bounce(RigidBody2D opponent)
    {
        Vector2 part1 = Impulse - ((2.0f * opponent.Mass) / (opponent.Mass + Mass)) * (Impulse - opponent.Impulse);
        Vector2 n = (Transform.Position - opponent.Transform.Position) / (Transform.Position - opponent.Transform.Position).Length();

        Transform.Position += (part1 * n * n) * 3.0f; //move out of collider
        Transform.Velocity *= -0.5f;
        OverrideImpulse(part1 * n * n, 1.0f);

        //Debug.Log("adding impulse of " + part1 * n * n + " to " + GameObject.Name);
    }
}

