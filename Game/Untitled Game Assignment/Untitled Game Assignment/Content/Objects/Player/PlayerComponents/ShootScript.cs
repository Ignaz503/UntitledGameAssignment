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
    MouseButtons shootButton;

    public ShootScript(GameObject obj, float speed,MouseButtons shootKey = MouseButtons.Left) : base(obj)
    {
        Speed = speed;
        shootButton = shootKey;
    }

    public override void OnDestroy()
    { }

    public void Update()
    {
        Target = Camera.Active.ScreenToWorld(Input.MousePosition);
        Vector2 direction = (Target - Transform.Position);
        direction.Normalize();

        if (Input.IsKeyDown(shootButton))
        {
            Bullet b = new Bullet(Transform.Position + direction*17, direction, Speed, this.GameObject);
            b.AddComponent( ( obj ) => new LifeTime( obj, 1.5f ) );
            Scene.Current.Instantiate(b);
        }
    }
}
