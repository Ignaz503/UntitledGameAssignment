using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;
using Util.FrameTimeInfo;

public class SpriteFlicker : Component, IUpdate
{
    SpriteRenderer SpriteRen;
    public bool MoveEnable;
    public double TimeToChange { get; set; }
    double CurrentTime;
    List<String> Sprites;
    int CurrentIndex = 0;

    public SpriteFlicker( GameObject obj, SpriteRenderer spriteRen, List<String> sprites, bool moveEnable = true, double time = 0.5f ) : base( obj )
    {
        SpriteRen = spriteRen;
        MoveEnable = moveEnable;
        Sprites = sprites;
        TimeToChange = time;
        CurrentTime = 0.0f;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
        if (MoveEnable && TimeInfo.timeStep.TotalGameTime.TotalSeconds > CurrentTime + TimeToChange)
        {
            CurrentTime = TimeInfo.timeStep.TotalGameTime.TotalSeconds;
            Flicker();
        }

        if (Transform.Velocity.Length() < 0.1f && Transform.Parent.Velocity.Length() < 0.1f)
            MoveEnable = false;
        else
            MoveEnable = true;
    }

    void Flicker()
    {
        SpriteRen.changeSprite(Sprites[CurrentIndex % Sprites.Count]);
        CurrentIndex++;

    }
}

