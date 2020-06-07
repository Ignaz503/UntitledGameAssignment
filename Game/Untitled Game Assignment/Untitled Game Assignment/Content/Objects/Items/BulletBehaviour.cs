using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;

public class BulletBehaviour : Component, IUpdate
{
    Vector2 Direction;
    float Speed;
    GameObject Shooter;

    public BulletBehaviour( GameObject obj, GameObject shooter, Vector2 direction, float speed ) : base( obj )
    {
        Direction = direction;
        Speed = speed;
        Shooter = shooter;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        Hit();
        Transform.Velocity = Direction * Speed;
    }

    public void Hit()
    {
        if (this.GameObject.GetComponent<BoxCollider>() != null)
        {
            foreach (GameObject collider in this.GameObject.GetComponent<BoxCollider>().Collisions)
            {
                if (collider != Shooter) //cant hit self
                {
                    if (collider is DestructableBox)
                    {
                        var dest = collider.GetComponent<Destructable>();

                        dest.OnHit( GameObject.GetComponent<BoxCollider>() );
                        continue;
                    }
                    collider.Destroy();
                    this.GameObject.Destroy();
                }
            }
        }
    }
}

