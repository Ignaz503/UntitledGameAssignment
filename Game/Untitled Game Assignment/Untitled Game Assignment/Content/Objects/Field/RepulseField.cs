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
using Loyc.Geometry;

public class RepulseField: Component, IUpdate
{
    float Scale;
    float Step;
    bool CursorMode;
    Func<Vector2, double, Vector2> Eval;

    public RepulseField( GameObject obj, Func<Vector2, double, Vector2> eval, float scale = 1.0f, float step = 0.1f, bool cursorMode = false ) : base( obj )
    {
        Scale = scale;
        Step = step;
        Eval = eval;
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

            if (obj.GetComponent<SpriteRenderer>().Layer == SortingLayer.Particles && obj.GetComponent<LifeTime>() != null)
            {
                EnactForce(obj);
            }
        }
    }

    /// <summary>
    /// Fourth-Order Runge-Kutta technique to evaluate velocity of particle at position and time
    /// </summary>
    /// <param name="position"></param>
    /// <param name="time"></param>
    Vector2 CalcField(Vector2 position, float time)
    {
        //evaluate initial tangent
        Vector2 k1 = Eval(position, time);

        //evaluation on midway position of first tangent
        Vector2 k2 = Eval(position + k1 / 2.0f, time + Step / 2.0f);

        //third midpoint evaluation halfway across new tangent
        Vector2 k3 = Eval(position + k2 / 2.0f, time + Step / 2.0f);

        //evaluation after full step of third tangent at end of interval
        Vector2 k4 = Eval(position + k3, time + Step);

        //final weighted average
        return position + (k1 + 2.0f * k2 + 2.0f * k3 + k4) / 6.0f;

        //return new Vector2(CalcI(position), CalcJ(position));
    }

    /// <summary>
    /// Calculates new Velocity for particles based on RK4 technique
    /// if cursormode, then origin is at cursor, else on player
    /// </summary>
    void EnactForce(GameObject obj)
    {
        Vector2 origin;

        origin = CursorMode == true ? Camera.Active.ScreenToWorld(Input.MousePosition) : Transform.Position;

        obj.Transform.Velocity += CalcField((obj.Transform.Position - origin) * Scale, obj.GetComponent<LifeTime>().Step());//  .GetComponent<RigidBody2D>().AddImpulse(CalcField(obj.Transform.Position - Transform.Position), Multiplier);
    }

    /*float CalcI(Vector2 position)
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
    }*/

}

