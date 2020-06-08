using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomDebug;
using Util.CustomMath;
using Util.FrameTimeInfo;

public class PathFollower : Component, IFixedUpdate
{
    public event Action OnDestroyed;

    Path pathToFollow;

    public Path PathToFollow 
    {
        get { return pathToFollow; }
        set { pathToFollow = value; Reset(); }
    }

    float t = 0f;

    float sampleRate;

    public float Speed;

    Dictionary<float, float> arcLUT;

    public PathFollower( Path simplePath, float sampleRate, GameObject obj, float speed, int startIndx = 0  ) : base( obj )
    {
        pathToFollow = simplePath ?? throw new System.ArgumentNullException( nameof( simplePath ) );
        t = 0f;
        this.sampleRate = sampleRate;
        this.Speed = speed;
        BuildArcLUT();
    }

    private void BuildArcLUT()
    {
        arcLUT = new Dictionary<float, float>();
        int iter = Mathf.FloorToInt(1f / sampleRate);

        var entries = new ArcPair[iter];

        Vector2 prevPos = pathToFollow.FollowPathCatmullRom(0f);
        float total = 0f;
        for (int i = 0; i < iter; i++)
        {
            float sample = i * sampleRate;

            Vector2 newPos = pathToFollow.FollowPathCatmullRom(sample);

            float d =  (newPos - prevPos).Length();

            total += d;

            entries[i] = new ArcPair( sample, total);
        }

        for (int i = 0; i < entries.Length; i++)
        {
            var item = entries[i];
            item.value /= total;
            arcLUT.Add( item.key, item.value );
        }

    }

    public void FixedUpdate()
    {
        FollowPathArc( pathToFollow );
    }

    private void FollowPathArc( Path p )
    {
        t += TimeInfo.FixedDeltaTime*Speed;

        if (t >= 1f)
        {
            t = 0f;
        }

        float arc = FindArcStep(t);


        var newPos = pathToFollow.FollowPathCatmullRom(arc);
        var dir = (newPos-Transform.Position);
        dir.Normalize();

        Transform.Rotation = 90f*Mathf.Deg2Rad + (float)Math.Atan2(dir.Y,dir.X);

        //var newPos = Vector2.Lerp(p.Current,p.Next,t);

        Transform.Position = newPos;
    }

    private float FindArcStep( float t )
    {
        if (arcLUT.ContainsKey( t ))
            return arcLUT[t];
        else
        {
            //find the two closest ones and interpolate

            float tClosest = 0;
            float tsecondClosest = 0;

            float dif = float.MaxValue;
            float difSecondClosest = float.MaxValue;

            foreach (var item in arcLUT.Keys)
            {
                var newDif = Mathf.Abs(item - t);

                if (dif > newDif)
                {
                    if (difSecondClosest > dif)
                    { 
                        tsecondClosest = tClosest;
                        difSecondClosest = dif;
                    }
                    dif = newDif;
                    tClosest = item;
                }

                if (newDif < difSecondClosest && newDif > dif)
                {
                    difSecondClosest = newDif;
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
            return MathHelper.Lerp( arcLUT[tClosest], arcLUT[tsecondClosest], interpolator );
        }
    }

    public override void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }

    public void Reset()
    {
        if (pathToFollow != null)
        {
            SetEnabled( true );
            t = 0f;
            if (pathToFollow != null)
                pathToFollow.Reset();
        } else
        {
            SetEnabled( false );
        }
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

