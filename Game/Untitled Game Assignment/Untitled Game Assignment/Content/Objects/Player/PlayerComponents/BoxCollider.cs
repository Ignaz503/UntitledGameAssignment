using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;
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

    public void Update()
    {
        BoundingBox = new Rectangle((this.GameObject.Transform.Position - SpriteRen.Sprite.Bounds.Size.ToVector2() / 2.0f).ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions.Clear();
        Collide();
        UpdateVelocity();

        if (TimeInfo.timeStep.TotalGameTime.TotalSeconds > ShowTime && Visualize)
        {
            Particle Lcoll = new Particle(new Vector2(BoundingBox.Left, BoundingBox.Center.Y));
            Lcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Lcoll);

            Particle Rcoll = new Particle(new Vector2(BoundingBox.Right, BoundingBox.Center.Y));
            Rcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Rcoll);

            Particle Tcoll = new Particle(new Vector2(BoundingBox.Center.X, BoundingBox.Top));
            Tcoll.AddComponent((obj) => new LifeTime(obj, 0.0001f));
            Scene.Current.Instantiate(Tcoll);

            Particle Bcoll = new Particle(new Vector2(BoundingBox.Center.X, BoundingBox.Bottom));
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
                // Add angular momentum to colliding RigidBody
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

