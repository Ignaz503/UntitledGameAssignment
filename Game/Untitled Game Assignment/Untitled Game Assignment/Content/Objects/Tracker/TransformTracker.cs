using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Util.Input;
using UntitledGameAssignment.Core;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;

public class TransformTracker : Component, ILateUpdate
{
    public Transform ToTrack { get; set; }
    bool trackRotation;
    Func<Vector2> getOffset;
    public TransformTracker( Transform toTrack, Func<Vector2> getOffset, GameObject obj, bool trackRotation = false  ) : base( obj )
    {
        this.ToTrack = toTrack;
        toTrack.GameObject.OnDestroying -= OnToTrackDestroyed; //no double register
        toTrack.GameObject.OnDestroying += OnToTrackDestroyed;
        this.trackRotation = trackRotation;
        this.getOffset = getOffset ?? throw new ArgumentNullException(nameof(getOffset));
    }

    private void OnToTrackDestroyed( GameObject obj )
    {
        ToTrack.GameObject.OnDestroying -= OnToTrackDestroyed;
        ToTrack = null;
    }

    public void LateUpdate()
    {
        if (ToTrack != null)
        {

            Transform.Position = ToTrack.Position;
            Transform.Position += getOffset();
            if (trackRotation)
                Transform.Rotation = ToTrack.Rotation;

        }
    }

    public override void OnDestroy()
    {}
}
