using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.FrameTimeInfo;

class TankShootStationary : Component, IUpdate
{
    float speed;
    Transform target;
    Transform turret;

    float frequency;
    float currentCount;
    float dist;

    float distSquared => dist * dist;
    public TankShootStationary( Transform target, Transform turret, GameObject @object, float speed = 15f, float frequency = 1, float dist = 50f ) : base( @object )
    {
        this.speed = speed;
        this.target = target;
        this.frequency = frequency;
        this.turret = turret;
        currentCount = frequency;
        this.dist = dist;

        target.GameObject.OnDestroying += OnTargetDeath;
    }


    public override void OnDestroy()
    { }

    void OnTargetDeath( GameObject tar )
    {
        tar.OnDestroying -= OnTargetDeath;
        target = null;
    }

    public void Update()
    {
        if (target == null)
        { return; }
        currentCount += TimeInfo.DeltaTime;
        var d = target.Position-Transform.Position;       ;

        if (d.LengthSquared() > 0f)
            d.Normalize();

        if (CheckIfShoot())
        {
            turret.Rotation = (VectorMath.Angle( d.X, d.Y ) + 90f)* Mathf.Deg2Rad;

            currentCount = 0f;
            Bullet b = new Bullet(Transform.Position + d*10, d, speed, GameObject);
            b.AddComponent( ( obj ) => new LifeTime( obj, 1.5f ) );
        }
    }

    private bool CheckIfShoot()
    {
        if (target == null)
            return false;
        var v = new Vector2((float)Math.Cos(Transform.Rotation - 90f*Mathf.Deg2Rad),(float)Math.Sin(Transform.Rotation- 90f*Mathf.Deg2Rad));

        v.Normalize();

        var tarDir = target.Position - Transform.Position;
        float distToTarget = tarDir.LengthSquared();
        tarDir.Normalize();

        return Vector2.Dot( tarDir, v ) > .8f && currentCount >= frequency && distToTarget <= distSquared;
    }
}
