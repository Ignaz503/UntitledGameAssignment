using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;
using Util.CustomDebug;
using Util.CustomMath;
using UntitledGameAssignment.Core.SceneGraph;

public class SpawnParticles : Component, IUpdate
{
    double TimeToSpawn;
    double Last;
    int Dist;
    string FilePath;

    public SpawnParticles( GameObject obj, int dist, double timeToSpawn, string filePath ) : base( obj )
    {
        TimeToSpawn = timeToSpawn;
        FilePath = filePath;
        Dist = dist;
        Last = 0.0f;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {

        if (TimeInfo.timeStep.TotalGameTime.TotalSeconds - Last > TimeToSpawn )
        {

            Random rand = new Random();
            float x = rand.Next(-Dist, Dist);
            float y = rand.Next(-Dist, Dist);

            Particle firefly = new Particle(Transform.Position + new Vector2((float)x, (float)y), FilePath);
            firefly.AddComponent((obj) => new LifeTime(obj, 1.0f));
            Scene.Current.Instantiate(firefly);
        }
    }

}

