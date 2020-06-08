﻿using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System.Threading;

public class BulletBehaviour : Component, IUpdate
{
    GameObject Shooter;
    double Start;
    double TimeToDie;

    public BulletBehaviour( GameObject obj, GameObject shooter, double timeToDie = 2.0f ) : base( obj )
    {
        Shooter = shooter;
        Start = TimeInfo.timeStep.TotalGameTime.TotalSeconds;
        TimeToDie = timeToDie;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        Hit();

        if (TimeInfo.timeStep.TotalGameTime.TotalSeconds > Start + TimeToDie)
        {
            this.GameObject.Destroy();
        }
        
        //Debug.Log("V = " + Transform.Velocity);
        //Transform.Velocity = Direction * Speed;
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
                        collider.GetComponent<Destructable>().OnHit(GameObject.GetComponent<BoxCollider>());
                    }

                    //collider.Destroy();
                    this.GameObject.Destroy();
                }
            }
        }
    }
}

