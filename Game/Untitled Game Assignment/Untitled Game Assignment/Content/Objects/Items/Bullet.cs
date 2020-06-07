using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Microsoft.Xna.Framework;
using UntitledGameAssignment;
using Util.Input;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System;
using Util.CustomMath;

public class Bullet : GameObject
{
    float rot;

    public Bullet( Vector2 position, Vector2 direction, float speed, GameObject shooter )
    {
        Transform.Position = position;

        rot = (VectorMath.Angle(direction.X, direction.Y) + 90.0f) / 180.0f * (float)Math.PI;
        Transform.Rotation = rot;

        //Debug.Log(direction.X + ", " + direction.Y + ": " + rot);

        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/bullet", Color.White, 1, obj));

        AddComponent( (obj) => new BoxCollider(spriteRen, obj) );
                      
        AddComponent( (obj) => new BulletBehaviour(obj, shooter, direction, speed) );

    }
}