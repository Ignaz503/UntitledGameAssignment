using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;

public class PickupHeartBehaviour : Component, IUpdate
{
    Color Tint;
    TempPlayer Heal;

    public PickupHeartBehaviour (GameObject obj, GameObject heal, Color c ) : base( obj )
    {
        Tint = c;
        Heal = (TempPlayer)heal;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        Hit();
    }

    public void Hit()
    {
        if (this.GameObject.GetComponent<BoxCollider>() != null)
        {
            foreach (GameObject collider in this.GameObject.GetComponent<BoxCollider>().Collisions)
            {
                if (collider == Heal) //cant hit self
                {
                    Heal.ChangeTint(Tint);
                    this.GameObject.Destroy();
                }
            }
        }
    }
}

