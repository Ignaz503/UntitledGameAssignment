using System;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using System.Diagnostics;
using Util.FrameTimeInfo;
using Microsoft.Xna.Framework;
using Util.CustomMath;
using Loyc.Collections;
using UntitledGameAssignment.Util.Vornoi;
using GeoUtil.Polygons;
using GeoUtil;
using Loyc.Geometry;
using Util.AssetManagment;
using Microsoft.Xna.Framework.Graphics;
using Util.Rendering;
using Util.SortingLayers;
using UntitledGameAssignment;
using System.IO;
using GeoUtil.HelperCollections.Grids;
using System.Text;

class Destructable : Component
{
    BoxCollider ownCollider;
    SpriteRenderer renderer;
    private bool isShattered;
    DissipateInfo info;
    Rect TransformRect => new Rect( Transform.Position, renderer.Sprite.Bounds.Size.ToVector2() );

    public Destructable(BoxCollider collider, SpriteRenderer r, DissipateInfo info, GameObject obj) : base( obj )
    {
        ownCollider = collider ?? throw new ArgumentNullException( nameof( collider ) );
        renderer = r ?? throw new ArgumentNullException( nameof( r ) );
        this.info = info;
    }

    public void OnHit(BoxCollider other)
    {
        if (isShattered)
            return;
        isShattered = true;
        CheckForHit(other);
    }

    void CheckForHit(BoxCollider other) 
    {

        if (other != ownCollider)
        {
            if (other.IsTouchingBottom( ownCollider ))
            {
                SplinterFromBottom(other.Transform.Position);
            } else if (other.IsTouchingLeft( ownCollider ))
            {
                SplinterFromLeft( other.Transform.Position );
            } else if (other.IsTouchingRight( ownCollider ))
            {
                SplinterFromRight( other.Transform.Position );
            } else if (other.IsTouchingTop( ownCollider ))
            {
                SplinterFromTop( other.Transform.Position );
            } else
            {
                Splinter( TransformRect, other.Transform.Position );
            }
        }
        this.GameObject.Destroy();
    }

    private void Splinter(Rect seedPointsRect,Vector2 hitPosition)
    {
        Rect r = new Rect(Transform.Position,renderer.Sprite.Bounds.Size.ToVector2());
        System.Random rng = new Random((int)DateTime.Now.Ticks);

        var seedPointCount = rng.Next(3,9);

        Vector2Int texSize = new Vector2Int(renderer.Sprite.Width,renderer.Sprite.Height);
        var pos = Transform.Position;
        var originaltexture = renderer.Sprite;

        Vector2[] seedPoints = new Vector2[seedPointCount];


        for (int i = 0; i < seedPointCount; i++)
        {
            seedPoints[i] = seedPointsRect.TopLeft + seedPointsRect.Size * new Vector2((float)rng.NextDouble(),(float)rng.NextDouble());

        }

        Shatter.ShatterBox( r, seedPoints, pos, texSize, originaltexture, hitPosition, renderer.Layer, Transform.Scale,info, rng );

        this.GameObject.Destroy();
    }

    private void SplinterFromTop( Vector2 hitPosition )
    {
        Rect totalRect = TransformRect;
        
        var seedPointRect = new Rect(totalRect.TopLeft.X,totalRect.TopLeft.Y,totalRect.BottomRight.X,totalRect.Center.Y);

        Splinter( seedPointRect, hitPosition );
    }

    private void SplinterFromRight( Vector2 hitPosition )
    {
        Rect totalRect = TransformRect;

        var seedPointRect = new Rect(totalRect.Center.X,totalRect.TopLeft.Y,totalRect.BottomRight.X,totalRect.BottomRight.Y);

        Splinter( seedPointRect, hitPosition );
    }

    private void SplinterFromLeft( Vector2 hitPosition )
    {
        Rect totalRect = TransformRect;

        var seedPointRect = new Rect(totalRect.TopLeft.X,totalRect.TopLeft.Y,totalRect.Center.X,totalRect.BottomRight.Y);

        Splinter(seedPointRect, hitPosition );
    }

    private void SplinterFromBottom( Vector2 hitPosition )
    {
        Rect totalRect = TransformRect;

        var seedPointRect = new Rect(totalRect.TopLeft.X,totalRect.Center.Y,totalRect.BottomRight.X,totalRect.BottomRight.Y);

        Splinter(seedPointRect, hitPosition );
    }

    public override void OnDestroy()
    {
        ownCollider = null;
        renderer = null;
    }
}
