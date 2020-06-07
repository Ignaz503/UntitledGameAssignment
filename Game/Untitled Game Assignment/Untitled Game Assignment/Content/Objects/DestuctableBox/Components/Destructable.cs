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

class Destructable : Component
{
    BoxCollider ownCollider;
    SpriteRenderer renderer;
    TempPlayer player;
    private bool isShattered;

    public Destructable(BoxCollider collider, SpriteRenderer r, TempPlayer player,GameObject obj ) : base( obj )
    {
        ownCollider = collider ?? throw new ArgumentNullException( nameof( collider ) );
        renderer = r ?? throw new ArgumentNullException( nameof( r ) );
        this.player = player ?? throw new ArgumentNullException( nameof( player ) );
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
                SplinterFromBottom();
            } else if (other.IsTouchingLeft( ownCollider ))
            {
                SplinterFromLeft();
            } else if (other.IsTouchingRight( ownCollider ))
            {
                SplinterFromRight();
            } else if (other.IsTouchingTop( ownCollider ))
            {
                SplinterFromTop();
            } else
            {
                Splinter();
            }
        }
        this.GameObject.Destroy();
    }

    private void Splinter()
    {
        Rect r = new Rect(Transform.Position,renderer.Sprite.Bounds.Size.ToVector2());
        System.Random rng = new System.Random((int)DateTime.Now.Ticks);

        Vector2[] seedPoints = new Vector2[]
        {
            r.TopLeft + r.Size * new Vector2((float)rng.NextDouble(),(float)rng.NextDouble()),
            r.TopLeft + r.Size * new Vector2((float)rng.NextDouble(),(float)rng.NextDouble()),
            r.TopLeft + r.Size * new Vector2((float)rng.NextDouble(),(float)rng.NextDouble()),
            r.TopLeft + r.Size * new Vector2((float)rng.NextDouble(),(float)rng.NextDouble()),
            r.TopLeft + r.Size * new Vector2((float)rng.NextDouble(),(float)rng.NextDouble())
        };

        var polygons = Voronoi.Shatter(seedPoints,r);
        

        for (int i = 0; i < polygons.Count; i++)
        {
            Debug.WriteLine( polygons[i].ToString() );
            var obj = CreatePolygon( polygons[i],r,i,rng);
        }


        //obj.Transform.Scale = new Vector2( Transform.Scale.X / polygons.Count, Transform.Scale.Y / polygons.Count );

        this.GameObject.Destroy();
    }

    GameObject CreatePolygon( IPolygon p, Rect r, int i, System.Random rng ) 
    {
        var newObj = new GameObject();
        newObj.Transform.Position = GeometryUtility.CalculateCentroid( p );
        //newObj.AddComponent( j => new GravPull( j, player, mass: 0.5f, effectiveRadius: 200.0f, rotate: false ) );
        var t = makenewTexture( p, r, i, rng);

        newObj.AddComponent( j => new SpriteRenderer( t, Color.White, SortingLayer.Entities, SpriteEffects.None, j ) );

        return newObj;
    }

    Texture2D makenewTexture(IPolygon p, Rect r, int i,System.Random rng) 
    {
        const int w = 128;
        const int h = 128;
        Texture2D newTexture = new Texture2D(GameMain.Instance.GraphicsDevice,w,h);
        Color c = new Color( rng.Next( 0, 256 ), rng.Next( 0, 256 ), rng.Next( 0, 256 ), 255 ); 
        Color[] data = new Color[w*h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                float tx = (float)x/(float)w;
                float ty = (float)y/(float)h;

                var t = new Vector2(r.Size.X*tx,r.Size.Y*ty);

                var point = r.TopLeft + t;

                if (GeometryUtility.PolygonContains( point, in p ))
                {
                    data[w * y + x] = c;
                } else
                {
                    data[w * y + x] = Color.Transparent;
                }
            }
        }
        newTexture.SetData( data );
        //using (FileStream fs = File.Create( $"poly{i}.png" ))
        //{
        //    newTexture.SaveAsPng( fs, newTexture.Width, newTexture.Height );
        //    System.Diagnostics.Debug.WriteLine( fs.Name );
        //}

        return newTexture;
    }


    private void SplinterFromTop()
    {
        Debug.WriteLine( "Splinter from top" );
        Splinter();
    }

    private void SplinterFromRight()
    {
        Debug.WriteLine( "Spliter from right" );
        Splinter();
    }

    private void SplinterFromLeft()
    {
        Debug.WriteLine( "Splinter from left" );
        Splinter();
    }

    private void SplinterFromBottom()
    {
        Debug.WriteLine( "Splinter from bottom" );
        Splinter();
    }

    public override void OnDestroy()
    {
        ownCollider = null;
        player = null;
        renderer = null;
    }
}
