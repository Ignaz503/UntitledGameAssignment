using Microsoft.Xna.Framework;
using System;
using Util.CustomDebug;
using Util.CustomMath;

public class Path
{
    public Vector2[] Points { get; set; }

    public bool Loop { get; set; }

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
            return Loop || CurrentIdx != NextIdx;
        }
    }

    public bool IsEnd( int idx )
    {
        return Loop ? false : idx == Points.Length - 1;
    }

    public bool HasPrev 
    {
        get 
        {
            //on loop there is always a previous
            //otherwise if current and previous are equal (both 0) we don't have a previous
            return Loop || CurrentIdx != PrevIdx;
        }
    }

    public Vector2 Current => Points[CurrentIdx];

    public Vector2 Next => Points[NextIdx];

    public Vector2 Prev => Points[PrevIdx];

    public Path( Vector2[] points, bool loop = false )
    {
        Points = points ?? throw new ArgumentNullException( nameof( points ) );
        Loop = loop;
        CurrentIdx = 0;
    }

    public Path( Vector2[] points, int startIdx, bool loop = false ) :this(points,loop)
    {
        CurrentIdx = SanetizeIdx(startIdx);
    }

    public void MoveForward() 
    {
        CurrentIdx = NextIdx;
    }

    public void Reset()
    {
        CurrentIdx = 0;
    }

    public void MoveBackward() 
    {
        CurrentIdx = PrevIdx;
    }

    public int GetPreviousIndex( int idx ) 
    {
        if (Loop)
            return idx == 0 ? Length - 1 : idx - 1;
        else
            return Math.Max( idx - 1, 0 );
    }

    public int GetNextIndex( int idx ) 
    {
        if (Loop)
        {
            return (idx + 1) % Length;
        }
        else
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
        

        int length;
        if (!Loop)
        {
            //1 is end
            //0 is is start
            length = Length -1;
            if (t > 1f)
                t -= 1f;

        } else
        {
            length = Length;
            if (t >= 1f)
                t -= 1f;
        }
        int currentIdx = SanetizeIdx(Mathf.FloorToInt( MathHelper.Lerp(0,length,t)));

        int prevIdx = GetPreviousIndex(currentIdx);
        int nextIdx = GetNextIndex(currentIdx);
        int nextOneOver = GetNextIndex(nextIdx);


        //rebase t
        float newT;
        if (currentIdx == Points.Length - 1 && Loop)
        {
            //looping to start
            float nextT = (float)(currentIdx+1)/(float)length;
            float currentT = (float)currentIdx/(float)length;

            newT = (t - currentT)/(nextT-currentT);
        } else
        {

            float nextT = (float)nextIdx/(float)length;
            float currentT = (float)currentIdx/(float)length;

            newT = (t - currentT)/(nextT-currentT);
        }
        return VectorMath.CatmullRom( Points[prevIdx], Points[currentIdx], Points[nextIdx], Points[nextOneOver], newT);

    }

}

