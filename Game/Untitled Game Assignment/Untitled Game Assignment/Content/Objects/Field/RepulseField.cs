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

public class RepulseField: Component, IUpdate
{
    float Mult;
    float Scale;
    bool CursorMode;

    public RepulseField( GameObject obj, float scale = 1.0f, float mult = 1.0f, bool cursorMode = false ) : base( obj )
    {
        Mult = mult;
        Scale = scale;
        CursorMode = cursorMode;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        List<GameObject> objects = Scene.Current.gameObjects;
        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent<SpriteRenderer>() == null)
                continue;

            if (obj.GetComponent<SpriteRenderer>().Layer == SortingLayer.Particles)
            {
                EnactForce(obj);
            }
        }
    }

    Vector2 CalcField(Vector2 position)
    {
        return new Vector2(CalcI(position), CalcJ(position)) * Mult;
    }

    float CalcI(Vector2 position)
    {
        //float i = position.Y / (position.X * position.X + position.Y * position.Y);
        //float i = -position.Y * 0.01f;
        //float i = (float)Math.Pow(position.Y, 3) - 9 * position.Y;
        float i = (float)Math.Cos(position.X + 2 * position.Y);
        return i;
    }

    float CalcJ(Vector2 position)
    {
        //float j = - position.X / (position.X * position.X + position.Y * position.Y);
        //float j = position.X * 0.01f;
        //float j = (float)Math.Pow(position.X, 3) - 9 * position.X;
        float j = (float)Math.Sin(position.X - 2 * position.Y);
        return j;
    }

    void EnactForce(GameObject obj)
    {
        Vector2 origin;

        origin = CursorMode == true ? Camera.Active.ScreenToWorld(Input.MousePosition) : Transform.Position;

        obj.Transform.Velocity = CalcField((obj.Transform.Position - origin) * Scale);//  .GetComponent<RigidBody2D>().AddImpulse(CalcField(obj.Transform.Position - Transform.Position), Multiplier);
    }

}

