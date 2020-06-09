using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography;
using Util.CustomDebug;
using Util.CustomMath;

public class Path
{
    public Vector2[] Points { get; set; }

    public int Length => Points.Length;

    public int CurrentIdx { get; protected set; }
    
    public int NextIdx 
    {
        get 
        {
            return GetNextIndex( CurrentIdx );
        }
    }

    public int PrevIdx 
    {
        get 
        {
            return GetPreviousIndex( CurrentIdx );
        }
    }

    public bool HasNext 
    {
        get 
        {
            //on loop we always have a next
            //otherwise if current and next are equal (length -1) we don't have a next
            return CurrentIdx != NextIdx;
        }
    }

    public Path( Vector2[] points )
    {
        Points = points ?? throw new ArgumentNullException( nameof( points ) );
        CurrentIdx = 0;
    }

    public Path Reverse() 
    {
        Vector2[] points = new Vector2[Points.Length];
        Points.CopyTo( points, 0 );
        Array.Reverse(points);
        return new Path( points );
    }

    public Path( Vector2[] points, int startIdx ) :this(points)
    {
        CurrentIdx = SanetizeIdx(startIdx);
    }


    public int GetPreviousIndex( int idx ) 
    {
       return Math.Max( idx - 1, 0 );
    }

    public int GetNextIndex( int idx ) 
    {
        return Math.Min( idx + 1, Length - 1 );
    }

    int SanetizeIdx( int someIdx ) 
    {
        if (someIdx >= 0 && someIdx <= Length - 1)
            return someIdx;
        else if (someIdx >= Length)
        {
            return GetNextIndex( someIdx - 1 );
        } else
        {
            //we are below 0 it's freezing
            return GetPreviousIndex(someIdx + 1);
        }
    }

    public Vector2 this[int idx] 
    {
        get { return Points[SanetizeIdx( idx )]; }
    }

    public Vector2 FollowPathCatmullRom( float t ) 
    {
        
        //1 is end
        //0 is is start
        int length = Length -1;

        int currentIdx = SanetizeIdx(Mathf.FloorToInt( MathHelper.Lerp(0,length,t)));

        int prevIdx = GetPreviousIndex(currentIdx);
        int nextIdx = GetNextIndex(currentIdx);
        int nextOneOver = GetNextIndex(nextIdx);

        //rebase t
        float nextT = (float)nextIdx/(float)length;
        float currentT = (float)currentIdx/(float)length;
        float newT;
        if (nextIdx != currentIdx)
        { 
            newT = (t - currentT) / (nextT - currentT);
        }
        else
            newT = currentIdx;
        
        return VectorMath.CatmullRom( Points[prevIdx], Points[currentIdx], Points[nextIdx], Points[nextOneOver], newT);

    }

}

