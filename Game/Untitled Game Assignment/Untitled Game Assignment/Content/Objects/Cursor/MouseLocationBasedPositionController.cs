using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class MouseLocationBasedPositionController : Component, IUpdate
{

    public MouseLocationBasedPositionController( GameObject obj ) : base( obj )
    {
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        Transform.Position = Input.MousePosition;
        
    }
}

