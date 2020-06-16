using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loyc.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UntitledGameAssignment.Core;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.FrameTimeInfo;
using Util.Input;
using Util.SortingLayers;

class TankShoot : Component,IUpdate
{
    float speed;
    Vector2 prevPos;
    Vector2 prevDir;
    Transform target;

    float frequency;
    float currentCount;

    public TankShoot( Transform target, GameObject @object, float speed = 15f, float frequency = 1): base(@object)
    {
        this.speed = speed;
        this.prevPos = Transform.Position;
        this.target = target;
        this.frequency = frequency;
        currentCount = frequency;
    }


    public override void OnDestroy()
    {}

    public void Update()
    {
        currentCount += TimeInfo.DeltaTime;
        var d = Transform.Position - prevPos;

        if (d.LengthSquared() > 0f)
            d.Normalize();
        else
            d = prevDir;

        if (CheckIfShoot())
        {
            currentCount = 0f;
            Bullet b = new Bullet(Transform.Position + d*17, d, speed, GameObject);
            b.AddComponent( ( obj ) => new LifeTime( obj, 1.5f ) );
        }
        if (d.LengthSquared() > 0)
        { 
            prevDir = d;
        }
        prevPos = Transform.Position;
    }

    private bool CheckIfShoot()
    {
        var v = new Vector2((float)Math.Cos(Transform.Rotation - 90f*Mathf.Deg2Rad),(float)Math.Sin(Transform.Rotation- 90f*Mathf.Deg2Rad));

        v.Normalize();

        var tarDir = target.Position - Transform.Position;
        tarDir.Normalize();

        return Vector2.Dot( tarDir, v ) > .9f && currentCount >= frequency;
    }
}

