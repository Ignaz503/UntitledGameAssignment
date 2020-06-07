using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.Input;
using Util.SortingLayers;
using Util.CustomDebug;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.AssetManagment;
using Util.FrameTimeInfo;

public class PathCreator : Component, IUpdate, IDraw
{
    List<Vector2> points;
    bool loop = true;

    Texture2D pathMarker;

    PathFollower pathFollower;
    bool hasPathPoints => points.Count > 1;
    Vector2 pathMarkerTextureOrigin;

    SpriteFont debugFont;

    public PathCreator(Texture2D pathMarker,  GameObject obj ) : base( obj )
    {
        this.pathMarker = pathMarker ?? throw new ArgumentNullException( nameof( pathMarker ) );
        points = new List<Vector2>();
        pathMarkerTextureOrigin = new Vector2(pathMarker.Width*0.5f,pathMarker.Height*0.5f);
        debugFont = AssetManager.Load<SpriteFont>( "Arial" ); 
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
               Debug.LogWarning( "Please place some path points(>1) before starting the path following" );
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
        if (pathFollower == null)
        {
            var obj = new GameObject();
            obj.Name = "PathFollower";

            obj.Transform.Position = p[0];

            var dir =  p[p.GetNextIndex( 0 )] - p[0];
            dir.Normalize();

            obj.Transform.Rotation = 90f * Mathf.Deg2Rad + (float)Math.Atan2( dir.Y, dir.X );

            obj.AddComponent( ( j ) => new SpriteRenderer( "Sprites/playershoulders",  Color.White, 1, j ) );

            pathFollower = obj.AddComponent( ( j ) => new PathFollower( p, 0.05f,  j ) );

            AddObjectsToEitherSide(obj, recursion: 2 );

            pathFollower.OnDestroyed -= OnPathFollowerDestroyed;//no double registering
            pathFollower.OnDestroyed += OnPathFollowerDestroyed;

        } else
        {
            pathFollower.GameObject.Enable();
            pathFollower.PathToFollow = p;
        }
    }

    private void OnPathFollowerDestroyed()
    {
        pathFollower = null;
        pathFollower.OnDestroyed -= OnPathFollowerDestroyed;
    }

    private void AddObjectsToEitherSide(GameObject obj, int recursion )
    {
        if (recursion == 0)
            return;

        var parent = obj.Transform;

        var childLeft = new GameObject(parent);
        childLeft.Name = "PathFollowerChildLeft";
     
        var srend =childLeft.AddComponent( ( j ) => new SpriteRenderer( "Sprites/playershoulders", Color.Blue, 1, j ) );
        childLeft.Transform.LocalPosition = new Vector2( -srend.Sprite.Width, srend.Sprite.Height * 0.5f );

        AddObjectsToEitherSide( childLeft, recursion - 1 );

        var childRight = new GameObject(parent);
        childRight.Name = "PathFollowerChildLeft";

        srend = childRight.AddComponent( ( j ) => new SpriteRenderer( "Sprites/playershoulders", Color.Blue, 1, j ) );
        childRight.Transform.LocalPosition = new Vector2( srend.Sprite.Width, srend.Sprite.Height * 0.5f );

        AddObjectsToEitherSide( childRight, recursion - 1 );
    }

    private Path CreatePath()
    {

        return new Path( points.ToArray(), loop );
    }

    public void Draw()
    {        
        if (points.Count > 0)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var text = $"{i}";
                //Util.Rendering.BatchRenderer.Draw( pathMarker, points[i], null, Color.White, 0f, pathMarkerTextureOrigin, Vector2.One*0.5f, SpriteEffects.None, (SortingLayer)0);
                Util.Rendering.SortedBatchRenderer.Draw( pathMarker, points[i], null, Color.White, 0f, pathMarkerTextureOrigin, Vector2.One*0.25f, SpriteEffects.None, SortingLayer.Background);
               Util.Rendering.SortedBatchRenderer.DrawString( debugFont, $"{i}", points[i], Color.Black, Transform.Rotation, debugFont.MeasureString(text) * 0.5f, Vector2.One * 0.5f, SpriteEffects.None, SortingLayer.BackgroundSubLayer(1) );
            }
        }
    }
}

