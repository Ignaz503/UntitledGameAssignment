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
    double Last;

    SpriteRenderer SpriteRen;
    public Rectangle BoundingBox;
    public List<GameObject> Collisions { get; private set; }

    public BoxCollider( SpriteRenderer spriteRen, GameObject obj, SortingLayer layer, List<SortingLayer> neglect = null ) : base( obj )
    {
        Layer = layer;
        SpriteRen = spriteRen;
        BoundingBox = new Rectangle(this.GameObject.Transform.Position.ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions = new List<GameObject>();
        Last = 0.0f;

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
        BoundingBox = new Rectangle(this.GameObject.Transform.Position.ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions.Clear();
        Collide();
        UpdateVelocity();

        Last = TimeInfo.timeStep.TotalGameTime.TotalSeconds;
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

            //Add impulses if applicable
            if (opponent != null && self != null)
            {
                // Add angular momentum to colliding RigidBody
                if (Transform.Velocity.X > 0 && this.IsTouchingLeft(query))
                {
                    Vector2 L = new Vector2(BoundingBox.Left, BoundingBox.Center.Y);
                    //Debug.Log(Transform.Position + " L " + L + " C. " + BoundingBox.Center);

                    if (opponent.IsKinematic)
                        opponent.AddTorque(L, self.Impulse, 1.0f);
                    if (self.IsKinematic)
                        self.AddTorque(L, opponent.Impulse, 1.0f);
                }
                else if (Transform.Velocity.X < 0 && this.IsTouchingRight(query))
                {
                    Vector2 R = new Vector2(BoundingBox.Right, BoundingBox.Center.Y);
                    //Debug.Log(Transform.Position + " R " + R);

                    if (opponent.IsKinematic)
                        opponent.AddTorque(R, self.Impulse, 1.0f);
                    if (self.IsKinematic)
                        self.AddTorque(R, opponent.Impulse, 1.0f);
                }
                else if (Transform.Velocity.Y > 0 && this.IsTouchingTop(query))
                {
                    Vector2 T = new Vector2(BoundingBox.Center.X, BoundingBox.Top);
                    //Debug.Log(Transform.Position + " T " + T);

                    if (opponent.IsKinematic)
                        opponent.AddTorque(T, self.Impulse, 1.0f);
                    if (self.IsKinematic)
                        self.AddTorque(T, opponent.Impulse, 1.0f);
                }
                else if (Transform.Velocity.Y < 0 && this.IsTouchingBottom(query))
                {
                    Vector2 B = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
                    //Debug.Log(Transform.Position + " B " + B);

                    if (opponent.IsKinematic)
                        opponent.AddTorque(B, self.Impulse, 1.0f);
                    if (self.IsKinematic)
                        self.AddTorque(B, opponent.Impulse, 1.0f);
                }

                // Add existing movement and impulse to colliding RigidBody

                if (opponent.IsKinematic)
                    opponent.Bounce(self);
                if (self.IsKinematic)
                    self.Bounce(opponent);

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
        return BoundingBox.Left - Transform.Velocity.X < qBox.Right &&
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
        return BoundingBox.Top - Transform.Velocity.Y < qBox.Bottom &&
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

