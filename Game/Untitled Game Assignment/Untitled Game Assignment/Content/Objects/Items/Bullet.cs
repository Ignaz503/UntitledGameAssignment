using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Microsoft.Xna.Framework;
using UntitledGameAssignment;
using Util.Input;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System;
using Util.CustomMath;
using Util.SortingLayers;
using System.Collections.Generic;

public class Bullet : GameObject
{
    public Bullet( Vector2 position, Vector2 direction, float speed, GameObject shooter )
    {
        Transform.Position = position;

        //Transform.Rotation =  (VectorMath.Angle(direction.X, direction.Y) + 90.0f) / 180.0f * (float)Math.PI;

        AddComponent((obj) => new VelocityBasedRotationController(obj, Transform) );

        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/bullet", Color.White, 1, obj));

        List<SortingLayer> shooterlayer = new List<SortingLayer>();
        shooterlayer.Add(shooter.GetComponent<BoxCollider>().Layer);
        AddComponent( (obj) => new BoxCollider(spriteRen, obj, SortingLayer.Entities, shooterlayer) );

        RigidBody2D rb = AddComponent( (obj) => new RigidBody2D(obj, 1.01f, 1.0f) );
                      
        AddComponent( (obj) => new BulletBehaviour(obj, shooter) );

        rb.AddImpulse(direction, speed);

    }
}