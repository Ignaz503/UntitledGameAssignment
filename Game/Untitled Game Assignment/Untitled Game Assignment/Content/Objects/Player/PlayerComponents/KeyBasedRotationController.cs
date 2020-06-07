using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework.Input;
using Util.CustomMath;
using Util.Input;
using Util.FrameTimeInfo;
using Util.CustomDebug;

public class KeyBasedRotationController : Component, IUpdate
{
    Keys increase,decrease;
    float changeInRad = 1f;

    public KeyBasedRotationController( GameObject obj,Keys increase = Keys.Q,Keys decrease = Keys.E,float changeInDegrees=1f ) : base( obj )
    {
        this.increase = increase;
        this.decrease = decrease;
        changeInRad = changeInDegrees * Mathf.Deg2Rad;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        float change = 0f;


        if (Input.IsKeyPressed( increase ))
        {
            change += changeInRad * TimeInfo.DeltaTime;
        }
        if( Input.IsKeyPressed( decrease ) )
        {
            change -= changeInRad * TimeInfo.DeltaTime;
        }


        //Debug.Log( $"current rotation: {Transform.LocalRotation}" );
        Transform.LocalRotation += change;
    }
}

