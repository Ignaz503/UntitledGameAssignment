using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.Input;
using Util.SortingLayers;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.AssetManagment;
using Util.FrameTimeInfo;

public class PathCreator : Component, IUpdate, IDraw
{
    List<Vector2> points;

    Texture2D pathMarker;

    PathFollower pathFollower;
    bool hasPathPoints => points.Count > 1;
    Vector2 pathMarkerTextureOrigin;

    SpriteFont debugFont;

    float followSpeed;

    public PathCreator(PathFollower pf, Texture2D pathMarker, float followSpeed, GameObject obj ) : base( obj )
    {
        this.pathFollower = pf;
        pathFollower.OnDestroyed -= OnPathFollowerDestroyed;//no double registering
        pathFollower.OnDestroyed += OnPathFollowerDestroyed;

        this.pathMarker = pathMarker ?? throw new ArgumentNullException( nameof( pathMarker ) );
        points = new List<Vector2>();
        pathMarkerTextureOrigin = new Vector2(pathMarker.Width*0.5f,pathMarker.Height*0.5f);
        debugFont = AssetManager.Load<SpriteFont>( "Arial" );
        this.followSpeed = followSpeed;
    }

    public override void OnDestroy()
    { }

    public void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.IsKeyDown( MouseButtons.Left ))
        {
            points.Add( Camera.Active.ScreenToWorld( Input.MousePosition ) );
        }
        if (Input.IsKeyDown( Keys.Space ))
        {
            if (hasPathPoints)
                CreatePathFollower();
            else
               Debug.WriteLine( "Please place some path points(>1) before starting the path following" );
        }
        if (Input.IsKeyDown( Keys.R ))
        {
            Reset();
        }
    }

    private void Reset()
    {
        points.Clear();
        pathFollower.PathToFollow = null;
        pathFollower.GameObject.Disable();
    }

    private void CreatePathFollower()
    {
        Path p = CreatePath();
        pathFollower.PathToFollow = p;
    }

    private void OnPathFollowerDestroyed()
    {
        pathFollower = null;
        pathFollower.OnDestroyed -= OnPathFollowerDestroyed;
    }

    //private void AddObjectsToEitherSide(GameObject obj, int recursion )
    //{
    //    if (recursion == 0)
    //        return;

    //    var parent = obj.Transform;

    //    var childLeft = new GameObject(parent);
    //    childLeft.Name = "PathFollowerChildLeft";
     
    //    var srend =childLeft.AddComponent( ( j ) => new SpriteRenderer( "Sprites/playershoulders", Color.Blue, 1, j ) );
    //    childLeft.Transform.LocalPosition = new Vector2( -srend.Sprite.Width, srend.Sprite.Height * 0.5f );

    //    AddObjectsToEitherSide( childLeft, recursion - 1 );

    //    var childRight = new GameObject(parent);
    //    childRight.Name = "PathFollowerChildLeft";

    //    srend = childRight.AddComponent( ( j ) => new SpriteRenderer( "Sprites/playershoulders", Color.Blue, 1, j ) );
    //    childRight.Transform.LocalPosition = new Vector2( srend.Sprite.Width, srend.Sprite.Height * 0.5f );

    //    AddObjectsToEitherSide( childRight, recursion - 1 );
    //}

    private Path CreatePath()
    {

        return new Path( points.ToArray());
    }

    public void Draw()
    {        
        if (points.Count > 0)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var text = $"{i}";
                Util.Rendering.SortedBatchRenderer.Draw( pathMarker, points[i], null, Color.White, 0f, pathMarkerTextureOrigin, Vector2.One*0.25f, SpriteEffects.None, SortingLayer.Background);
               Util.Rendering.SortedBatchRenderer.DrawString( debugFont, $"{i}", points[i], Color.Black, Transform.Rotation, debugFont.MeasureString(text) * 0.5f, Vector2.One * 0.5f, SpriteEffects.None, SortingLayer.BackgroundSubLayer(1) );
            }
        }
    }
}

