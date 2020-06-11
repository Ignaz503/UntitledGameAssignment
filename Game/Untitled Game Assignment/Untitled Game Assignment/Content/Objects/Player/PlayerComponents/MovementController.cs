using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System.Security.Cryptography;

public class MovementController : Component, IUpdate
{
    float walkSpeed;

    Keys up;
    Keys down;
    Keys left;
    Keys right;

    public MovementController( GameObject obj, Keys up = Keys.W, Keys down = Keys.S, Keys left = Keys.A, Keys right = Keys.D, float walkSpeed = 2f) : base( obj )
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
        this.walkSpeed = walkSpeed;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        Vector2 desiredDir = Vector2.Zero;
        if (Input.IsKeyPressed( up ))
        {
            desiredDir += new Vector2( 0, -1 );
        }
        if (Input.IsKeyPressed( left ))
        {
            desiredDir += new Vector2( -1, 0 );
        }
        if (Input.IsKeyPressed( down ))
        {
            desiredDir += new Vector2( 0, 1 );
        }
        if (Input.IsKeyPressed( right ))
        {
            desiredDir += new Vector2( 1, 0 );
        }

        if (desiredDir != Vector2.Zero)
        {
            desiredDir.Normalize();
        }

        if (GameObject.GetComponent<RigidBody2D>() != null)
            GameObject.GetComponent<RigidBody2D>().AddImpulse(desiredDir, walkSpeed);
        else
            Transform.Velocity += desiredDir * walkSpeed * 50.0f * TimeInfo.DeltaTime;
    }
}

