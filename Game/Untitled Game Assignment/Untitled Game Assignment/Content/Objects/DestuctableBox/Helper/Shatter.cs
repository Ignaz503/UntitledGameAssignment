﻿using GeoUtil.HelperCollections.Grids;
using GeoUtil.Polygons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Util.Vornoi;
using Util.CustomMath;
using Util.SortingLayers;
using Util.TextureHelper;

public static class Shatter
{
    public static void ShatterBox(Rect bounds, Vector2[] seedPoints,Vector2 position, Vector2Int textureSize, Texture2D originalTexture, Vector2 hitPosition,SortingLayer sortingLayer,Vector2 originalScale,DissipateInfo dissipateInfo, Random rng) 
    {
        ThreadedDataRequestor.Instance.RequestData( ()=>MakePolygons(seedPoints,bounds), ( p ) => OnPolygonsRecieved( p, bounds, position, textureSize, originalTexture,hitPosition,sortingLayer, originalScale, dissipateInfo, rng ));
    }

    static List<IPolygon> MakePolygons(Vector2[] seedPoints, Rect bounds) 
    {
        return Voronoi.Shatter( seedPoints, bounds );
    }

    static void OnPolygonsRecieved(object obj_polygons,Rect bounds,Vector2 positon, Vector2Int textureSize, Texture2D originalTexture,Vector2 hitPosition, SortingLayer sortingLayer,Vector2 originalScale,DissipateInfo dissipateInfo, Random rng) 
    {
        var polygons = obj_polygons as List<IPolygon>;

        float tForce = (float)rng.NextDouble();
        for (int i = 0; i < polygons.Count; i++)
        {
            var poly = polygons[i];
            var newTexture = new Texture2D(GameMain.Instance.GraphicsDevice,textureSize.X,textureSize.Y);

            float tDir = (float)rng.NextDouble();
            ThreadedDataRequestor.Instance.RequestData(()=> GenerateTexture(poly,textureSize,newTexture,originalTexture.GetPixels(),bounds),(t)=>MakeGameobject(t,positon,hitPosition,sortingLayer,tDir,tForce,originalScale,dissipateInfo,rng));
        }
    }

    static Texture2D GenerateTexture(IPolygon p,Vector2Int textureSize,Texture2D newTexture,Color[] oldTexture,Rect r) 
    {
        int w = textureSize.X-1;
        int h = textureSize.Y-1;
        Color[] data = new Color[textureSize.X*textureSize.Y];

        for (int i = 0; i < data.Length; i++)
        {
            int x = i % textureSize.X;
            int y = i / textureSize.Y;

            float tx = (float)x/(float)w;
            float ty = (float)y/(float)h;


            float xOffset = r.Size.X * tx;
            float yOffset = r.Size.Y * ty;

            var pointToCompare = r.TopLeft + new Vector2(xOffset, yOffset);

            if (GeoUtil.GeometryUtility.PolygonContains( pointToCompare, in p ))
            {
                data[i] = oldTexture.GetPixel( x, y, textureSize.X );
            } else
            {
                data[i] = Color.Transparent;
            }
        }
        //Debug.WriteLine( builder.ToString() );
        newTexture.SetData( data );

        return newTexture;
    }

    static void MakeGameobject( object obj_newTexture, Vector2 position, Vector2 hitPosition, SortingLayer sortingLayer,float tDir, float tForce, Vector2 originalScale, DissipateInfo dissipateInfo,Random rng) 
    {
        var newTexture = obj_newTexture as Texture2D;

        var gObj = new GameObject();
        gObj.Transform.Position = position;
        gObj.Transform.Scale = originalScale;

        gObj.AddComponent( j => new SpriteRenderer( newTexture, Color.White, sortingLayer, SpriteEffects.None, j ) );

        if (dissipateInfo.Dissipate)
        {
            gObj.AddComponent( j => new LifeTime( gObj, dissipateInfo.Time + (2f * (float)rng.NextDouble()) ) );
        }


        var body = gObj.AddComponent( ( obj ) => new RigidBody2D( obj, 1.2f ) );

        var dir = position - hitPosition;

        dir.Normalize();


        var angle = Math.Atan2(dir.Y,dir.X);

        var spread = 50f*Mathf.Deg2Rad;

        var min = angle - spread;
        var max = angle + spread;

        var moveAngle = min*tDir + (1-tDir)*max;

        dir = new Vector2( (float)Math.Cos( moveAngle ), (float)Math.Sin( moveAngle ) );


        body.AddImpulse( dir, 9f * tForce );

    }
}


public struct DissipateInfo 
{
    public bool Dissipate { get; private set; }
    public float Time { get; private set; }

    public DissipateInfo( bool dissipate, float time = 1.5f)
    {
        Dissipate = dissipate;
        Time = time;
    }
}