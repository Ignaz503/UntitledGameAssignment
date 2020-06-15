using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.FrameTimeInfo;
using Util.Input;

public class PathFollower : Component, IFixedUpdate
{
    public event Action OnDestroyed;

    Path pathToFollow;
    Path reversePathToFollow;

    public Path PathToFollow 
    {
        get { return pathToFollow; }
        set 
        { 
            pathToFollow = value; 
            reversePathToFollow = pathToFollow.Reverse();
            Recalc();
            IsEnabled = true;
            reverse = false;
        }
    }

    float s = 0f;

    float sampleRate;

    public float Speed;

    Dictionary<float, float> arcLUT;
    Dictionary<float, float> arcLUTReverse;

    bool reverse;

    public PathFollower( float sampleRate, GameObject obj, float speed ) : base( obj )
    {
        s = 0f;
        this.sampleRate = sampleRate;
        this.Speed = speed;
        reverse = false;
    }

    private void BuildArcLUT()
    {
        arcLUT = new Dictionary<float, float>();
        arcLUTReverse = new Dictionary<float, float>();
        int iter = Mathf.FloorToInt(1f / sampleRate);

        var entriesForward = new ArcPair[iter];
        var entriesBackward = new ArcPair[iter];

        Vector2 prevPositionForward = pathToFollow.FollowPathCatmullRom(0f);
        float totalForward = 0f;

        float totalBackwards = 0f;
        Vector2 prevPositionBackwards = reversePathToFollow.FollowPathCatmullRom(0f);

        for (int i = 0; i < iter; i++)
        {
            float sample = i * sampleRate;

            Vector2 newPosForward = pathToFollow.FollowPathCatmullRom(sample);
            float dForward =  (newPosForward - prevPositionForward).Length();
            totalForward += dForward;
            entriesForward[i] = new ArcPair( totalForward, sample);
            prevPositionForward = newPosForward;

            Vector2 newPosBackward = reversePathToFollow.FollowPathCatmullRom(sample);
            float dBackwards = (newPosBackward-prevPositionBackwards).Length();
            totalBackwards += dBackwards;
            entriesBackward[i] = new ArcPair( totalBackwards, sample );
            prevPositionBackwards = newPosBackward;
        }

        for (int i = 0; i < entriesForward.Length; i++)
        {
            var item = entriesForward[i];
            item.key /= totalForward;
            arcLUT.Add( item.key, item.value );

            var itemBack = entriesBackward[i];
            itemBack.key /= totalBackwards;
            arcLUTReverse.Add( itemBack.key, itemBack.value );
        }
    }

    public void FixedUpdate()
    {
        if(pathToFollow != null)
            FollowPathArc(  );
    }

    private void FollowPathArc()
    {
        s += Speed*sampleRate*TimeInfo.FixedDeltaTime;
        bool done = false;
        if (s >= 1f)
        {
            s = 1f;
            done = true;
        }
        float arc = FindArcStep(s,reverse ? arcLUTReverse  : arcLUT);
        if (arc >= 1f)
        { 
            arc = 1f -0.001f;
        }

        Debug.WriteLine( arc );

        Vector2 newPos;
        if (reverse) {
            newPos = reversePathToFollow.FollowPathCatmullRom( arc ); 
        } else
        {
            newPos = pathToFollow.FollowPathCatmullRom(arc);
        }

        var dir = (newPos-Transform.Position);
        if (!done && dir.LengthSquared() > 0f)
        {
            dir.Normalize();
            Transform.Rotation = 90f * Mathf.Deg2Rad + (float)Math.Atan2( dir.Y, dir.X );
        }
        Transform.Position = newPos;
        
        if (done)
        {
            Done();
        }
    }

    private void Done()
    {
        reverse = !reverse;
        s = 0f;
        Debug.WriteLine( "reverse" );
    }

    private float FindArcStep( float t, Dictionary<float,float> lut )
    {
        if (lut.ContainsKey( t ))
            return lut[t];
        else
        {
            //find the two closest ones and interpolate
            float tClosest = 0;
            float tsecondClosest = 0;

            float difMin = float.MaxValue;
            float difMinSecond = float.MaxValue;

            foreach (var item in lut.Keys)
            {
                var newDif = Mathf.Abs(item - t);

                if (newDif < difMin)
                {
                    difMinSecond = difMin;
                    difMin = newDif;

                    tsecondClosest = tClosest;
                    tClosest = item;
                } else if (newDif < difMinSecond)
                {
                    difMinSecond = newDif;
                    tsecondClosest = item;
                }
            }

            if (tClosest > tsecondClosest)
            {
                var temp = tClosest;
                tClosest = tsecondClosest;
                tsecondClosest = temp;
            }

            float interpolator = (t - tClosest)/(tsecondClosest - tClosest);
            interpolator = Math.Min( interpolator, 1f );

            return MathHelper.Lerp( lut[tClosest], lut[tsecondClosest], interpolator );
        }
    }

    public override void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }

    public void Recalc()
    {
        s = 0f;
        BuildArcLUT();
    }

    struct ArcPair 
    {
        public float key;
        public float value;

        public ArcPair( float key, float value )
        {
            this.key = key;
            this.value = value;
        }
    }

}

