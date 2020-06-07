using Util.Input;
using Util.FrameTimeInfo;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Microsoft.Xna.Framework;
using System;
using Util.CustomDebug;
using Util.CustomMath;
using Microsoft.Xna.Framework.Input;
using UntitledGameAssignment.Core.SceneGraph;

public class ShootScript : Component, IUpdate
{
    Vector2 Target;
    float Speed;

    public ShootScript(GameObject obj, float speed) : base(obj)
    {
        Speed = speed;
    }

    public override void OnDestroy()
    { }

    public void Update()
    {
        Target = Camera.Active.ScreenToWorld(Input.MousePosition);
        Vector2 direction = (Target - Transform.Position);
        direction.Normalize();

        if (Input.IsKeyDown(MouseButtons.Right))
        {
            Bullet b = new Bullet(Transform.Position + direction*22, direction, Speed, this.GameObject);
            Scene.Current.Instantiate(b);
        }
    }
}
