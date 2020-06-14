using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System.Threading;

public class LifeTime : Component, IUpdate
{
    double Start;
    double TimeToDie;

    public LifeTime( GameObject obj, double timeToDie = 1.5f ) : base( obj )
    {
        Start = TimeInfo.timeStep.TotalGameTime.TotalSeconds;
        TimeToDie = timeToDie;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        if (TimeInfo.timeStep.TotalGameTime.TotalSeconds > Start + TimeToDie)
        {
            this.GameObject.Destroy();
        }
    }
}

