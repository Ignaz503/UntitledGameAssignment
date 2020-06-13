using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using System.Collections.Generic;
using UntitledGameAssignment.Core.SceneGraph;
using Util.SortingLayers;

public class ConcentricField: Component, IUpdate
{
    float EffectiveRadius;
    float Multiplier;

    public ConcentricField( GameObject obj, Vector2 origin, float multiplier, float effectiveRadius = 100.0f ) : base( obj )
    {
        Multiplier = multiplier;
        EffectiveRadius = effectiveRadius;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        List<GameObject> objects = Scene.Current.gameObjects;
        foreach (GameObject obj in objects)
        {
            if ((obj.Transform.Position - Transform.Position).Length() < EffectiveRadius && obj.GetComponent<RigidBody2D>() != null && obj.GetComponent<SpriteRenderer>().Layer == SortingLayer.Particles)
            {
                EnactForce(obj);
            }
        }
    }

    Vector2 CalcField(Vector2 position)
    {
        return new Vector2(CalcI(position), CalcJ(position));
    }

    float CalcI(Vector2 position)
    {
        float i = position.Y / (position.X * position.X + position.Y * position.Y);
        return i;
    }

    float CalcJ(Vector2 position)
    {
        float j = - position.X / (position.X * position.X + position.Y * position.Y);
        return j;
    }

    void EnactForce(GameObject obj)
    {
        obj.GetComponent<RigidBody2D>().AddImpulse(CalcField(obj.Transform.Position - Transform.Position), Multiplier);
    }

}

