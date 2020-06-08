using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;
using Util.SortingLayers;

public class BoxCollider : Component, IUpdate
{
    /// <summary>
    /// the sorting layer of this collider
    /// </summary>
    public SortingLayer Layer;
    public List<SortingLayer> Neglect;

    SpriteRenderer SpriteRen;
    public Rectangle BoundingBox;
    public List<GameObject> Collisions { get; private set; }
    public bool IsKinematic { get; private set; }
    float CollisionMultiplier = -0.001f;

    public BoxCollider( SpriteRenderer spriteRen, GameObject obj, SortingLayer layer, bool isKinematic = true, List<SortingLayer> neglect = null ) : base( obj )
    {
        Layer = layer;
        SpriteRen = spriteRen;
        BoundingBox = new Rectangle(this.GameObject.Transform.Position.ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions = new List<GameObject>();
        IsKinematic = isKinematic;

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
        CollisionMultiplier = -0.001f;
        BoundingBox = new Rectangle(this.GameObject.Transform.Position.ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions.Clear();
        Collide();
        UpdateVelocity();
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
    /// If next move would cause gameobject to collide, update velocity/impulse
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
                Vector2 newVelocity = Transform.Velocity;
                if ((Transform.Velocity.X > 0 && this.IsTouchingLeft(query)) ||
                    (Transform.Velocity.X < 0 && this.IsTouchingRight(query)) ||
                    (Transform.Velocity.Y > 0 && this.IsTouchingTop(query)) ||
                    (Transform.Velocity.Y < 0 && this.IsTouchingBottom(query)))
                {
                    //Debug.Log("adding impulse of " + Transform.Velocity + " to " + collider.Name);
                    // Add existing movement and impulse to colliding RigidBody
                    Vector2 direction = Transform.Velocity;
                    direction.Normalize();
                    opponent.AddImpulse(direction, self.Energy());
                    
                    //Slow own movement if have RigidBody2D and opponent bigger
                    CollisionMultiplier = Math.Min(1.0f, self.Mass / opponent.Mass);
                }
            }

            if (query.IsKinematic)
            {
                Vector2 newVelocity = Transform.Velocity;
                if ((Transform.Velocity.X > 0 && this.IsTouchingLeft(query)) ||
                    (Transform.Velocity.X < 0 && this.IsTouchingRight(query)))
                {
                    newVelocity.X *= CollisionMultiplier;
                }
                if ((Transform.Velocity.Y > 0 && this.IsTouchingTop(query)) ||
                    (Transform.Velocity.Y < 0 && this.IsTouchingBottom(query)))
                {
                    newVelocity.Y *= CollisionMultiplier;
                }
                Transform.Velocity = newVelocity;
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
            !Neglect.Contains(query.Layer);
    }

    public bool IsTouchingRight(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Left - Transform.Velocity.X < qBox.Right &&
            BoundingBox.Right > qBox.Right &&
            BoundingBox.Bottom > qBox.Top &&
            BoundingBox.Top < qBox.Bottom &&
            !Neglect.Contains(query.Layer);
    }

    public bool IsTouchingTop(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Bottom + Transform.Velocity.Y > qBox.Top &&
            BoundingBox.Top < qBox.Top &&
            BoundingBox.Right > qBox.Left &&
            BoundingBox.Left < qBox.Right &&
            !Neglect.Contains(query.Layer);
    }

    public bool IsTouchingBottom(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Top - Transform.Velocity.Y < qBox.Bottom &&
            BoundingBox.Bottom > qBox.Bottom &&
            BoundingBox.Right > qBox.Left &&
            BoundingBox.Left < qBox.Right &&
            !Neglect.Contains(query.Layer);
    }

    public bool IsTouching(BoxCollider query)
    {
        return IsTouchingTop(query) || IsTouchingBottom(query) || IsTouchingLeft(query) || IsTouchingRight(query);
    }
}

