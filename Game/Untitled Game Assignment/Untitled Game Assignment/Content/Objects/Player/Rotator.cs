using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.FrameTimeInfo;
using System.Security.Cryptography;

public class Rotator : Component, IUpdate
{
    float tRot;
    float direction;
    float speed;

    public Rotator( GameObject obj, float direction=1f, float speed=10f  ) : base(obj)
    {
        this.direction = Math.Sign( direction );
        this.speed = speed;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        tRot += speed * direction * TimeInfo.DeltaTime;

        if (tRot > 2 * Mathf.PI)
            tRot -= 2 * Mathf.PI;

        Transform.LocalRotation =  tRot ;
    }
}

