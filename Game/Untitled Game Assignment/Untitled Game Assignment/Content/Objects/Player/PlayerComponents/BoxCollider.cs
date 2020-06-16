using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;
using Util.CustomMath;
using Util.FrameTimeInfo;
using Util.SortingLayers;

public class BoxCollider : Component, IUpdate
{
    /// <summary>
    /// the sorting layer of this collider
    /// </summary>
    public SortingLayer Layer;
    public List<SortingLayer> Neglect;

    bool Visualize;
    double ShowTime;

    SpriteRenderer SpriteRen;
    public Rectangle BoundingBox;
    public List<GameObject> Collisions { get; private set; }

    /// <summary>
    /// Adds collider component to gameobject, allowing for collision detection
    /// </summary>
    /// <param name="spriteRen"></param>
    ///     Sprite renderer, needed for bounding box
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    ///     SortingLayer of bounding box
    /// <param name="neglect"></param>
    ///     List of SortingLayers to ignore during collision detection
    /// <param name="visualize"></param>
    ///     generates fireflies at corners of bounding box
    public BoxCollider( SpriteRenderer spriteRen, GameObject obj, SortingLayer layer, List<SortingLayer> neglect = null, bool visualize = false ) : base( obj )
    {
        Layer = layer;
        SpriteRen = spriteRen;
        BoundingBox = new Rectangle((this.GameObject.Transform.Position - SpriteRen.Sprite.Bounds.Size.ToVector2() / 2.0f).ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions = new List<GameObject>();

        Visualize = visualize;
        ShowTime = 0.0f; // TimeInfo.timeStep.TotalGameTime.TotalSeconds + 0.1f;

        if (neglect == null)
            Neglect = new List<SortingLayer>();
        else
        {
            Neglect = neglect;
        }
    }

    public override void OnDestroy()
    {}

    /// <summary>
    /// Transform point based on gameobject rotation
    /// </summary>
    Vector2 RotTransform(Vector2 vec, Vector2 origin)
    {
        Vector2 trans = vec - origin;
        double rad = (57.2957795948f + (Transform.Rotation - 1.0f) * 180.0f / Math.PI) * Math.PI / 180.0f;
        Vector2 rot = new Vector2(trans.X * (float)Math.Cos(rad) - trans.Y * (float)Math.Sin(rad),
                                  trans.X * (float)Math.Sin(rad) + trans.Y * (float)Math.Cos(rad));
        return rot + origin;
    }

    /// <summary>
    /// Axis-aligned bounding box around gameobject, takes rotation into consideration
    /// </summary>
    Rectangle MinXY()
    {
        Vector2 L0 = Transform.Position - SpriteRen.Sprite.Bounds.Size.ToVector2() / 2.0f;
        Vector2 R0 = L0 + SpriteRen.Sprite.Bounds.Size.ToVector2();
        Vector2 T0 = L0 + new Vector2(R0.X - L0.X, 0.0f);
        Vector2 B0 = L0 + new Vector2(0.0f, R0.Y - L0.Y);

        Vector2 L = RotTransform(L0, Transform.Position);
        Vector2 R = RotTransform(R0, Transform.Position);
        Vector2 T = RotTransform(T0, Transform.Position);
        Vector2 B = RotTransform(B0, Transform.Position);

        float minx = Math.Min(L.X, Math.Min(R.X, Math.Min(T.X, B.X)));
        float maxx = Math.Max(L.X, Math.Max(R.X, Math.Max(T.X, B.X)));
        float miny = Math.Min(L.Y, Math.Min(R.Y, Math.Min(T.Y, B.Y)));
        float maxy = Math.Max(L.Y, Math.Max(R.Y, Math.Max(T.Y, B.Y)));

        Rectangle rect = new Rectangle(new Vector2(minx, miny).ToPoint(), new Vector2(maxx - minx, maxy - miny).ToPoint());
        return rect;
    }

    public void Update()
    {
        //BoundingBox = new Rectangle((Transform.Position - SpriteRen.Sprite.Bounds.Size.ToVector2() / 2.0f).ToPoint(), SpriteRen.Sprite.Bounds.Size);
        BoundingBox = MinXY();

        Collisions.Clear();
        Collide();
        UpdateVelocity();

        if (TimeInfo.timeStep.TotalGameTime.TotalSeconds > ShowTime && Visualize)
        {
            Particle Lcoll = new Particle(new Vector2(BoundingBox.Left, BoundingBox.Top));
            Lcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Lcoll);

            Particle Rcoll = new Particle(new Vector2(BoundingBox.Right, BoundingBox.Bottom));
            Rcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Rcoll);

            Particle Tcoll = new Particle(new Vector2(BoundingBox.Right, BoundingBox.Top));
            Tcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Tcoll);

            Particle Bcoll = new Particle(new Vector2(BoundingBox.Left, BoundingBox.Bottom));
            Bcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Bcoll);

            ShowTime = TimeInfo.timeStep.TotalGameTime.TotalSeconds + 0.1f;
        }
    }

    /// <summary>
    /// checks against each object in scene and updates collision list accordingly
    /// </summary>
    void Collide()
    {
        List<GameObject> objects = Scene.Current.gameObjects;
        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent<BoxCollider>() != null && obj != this.GameObject)
            {
                if (this.IsTouching(obj.GetComponent<BoxCollider>()))
                {
                    Collisions.Add(obj);
                }
            }
        }
    }

    /// <summary>
    /// If both have rigidbodies and next move would cause gameobject to collide, update velocity/impulse
    /// </summary>
    /// <param name="query"></param>
    public void UpdateVelocity()
    {
        foreach (GameObject collider in Collisions)
        {
            BoxCollider query = collider.GetComponent<BoxCollider>();
            RigidBody2D opponent = collider.GetComponent<RigidBody2D>();
            RigidBody2D self = this.GameObject.GetComponent<RigidBody2D>();

            Vector2 collisionPoint = Vector2.Zero;

            //Add impulses if applicable
            if (opponent != null && self != null)
            {
                // Add angular momentum to colliding RigidBody at center of bounding box edge
                if (Transform.Velocity.X > 0 && this.IsTouchingLeft(query))
                {
                    collisionPoint = new Vector2(BoundingBox.Left, BoundingBox.Center.Y);
                }
                else if (Transform.Velocity.X < 0 && this.IsTouchingRight(query))
                {
                    collisionPoint = new Vector2(BoundingBox.Right, BoundingBox.Center.Y);
                }
                else if (Transform.Velocity.Y > 0 && this.IsTouchingTop(query))
                {
                    collisionPoint = new Vector2(BoundingBox.Center.X, BoundingBox.Top);
                }
                else if (Transform.Velocity.Y < 0 && this.IsTouchingBottom(query))
                {
                    collisionPoint = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
                }

                // Add existing movement and impulse to colliding RigidBody
                if (opponent.IsKinematic)
                {
                    opponent.AddTorque(collisionPoint, self.Impulse, 1.0f);
                    opponent.Bounce(self);
                }
                if (self.IsKinematic)
                {
                    self.AddTorque(collisionPoint, opponent.Impulse, 1.0f);
                    self.Bounce(opponent);
                }

                //query.Collisions.Remove(GameObject);
            }
        }
    }

    public bool IsTouchingLeft(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Right + Transform.Velocity.X > qBox.Left &&
            BoundingBox.Left < qBox.Left &&
            BoundingBox.Bottom > qBox.Top &&
            BoundingBox.Top < qBox.Bottom &&
            !Neglect.Contains(query.Layer) &&
            !query.Neglect.Contains(Layer);
    }

    public bool IsTouchingRight(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Left + Transform.Velocity.X < qBox.Right &&
            BoundingBox.Right > qBox.Right &&
            BoundingBox.Bottom > qBox.Top &&
            BoundingBox.Top < qBox.Bottom &&
            !Neglect.Contains(query.Layer) &&
            !query.Neglect.Contains(Layer);
    }

    public bool IsTouchingTop(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Bottom + Transform.Velocity.Y > qBox.Top &&
            BoundingBox.Top < qBox.Top &&
            BoundingBox.Right > qBox.Left &&
            BoundingBox.Left < qBox.Right &&
            !Neglect.Contains(query.Layer) &&
            !query.Neglect.Contains(Layer);
    }

    public bool IsTouchingBottom(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Top + Transform.Velocity.Y < qBox.Bottom &&
            BoundingBox.Bottom > qBox.Bottom &&
            BoundingBox.Right > qBox.Left &&
            BoundingBox.Left < qBox.Right &&
            !Neglect.Contains(query.Layer) &&
            !query.Neglect.Contains(Layer);
    }

    public bool IsTouching(BoxCollider query)
    {
        return IsTouchingTop(query) || IsTouchingBottom(query) || IsTouchingLeft(query) || IsTouchingRight(query);
    }
}

