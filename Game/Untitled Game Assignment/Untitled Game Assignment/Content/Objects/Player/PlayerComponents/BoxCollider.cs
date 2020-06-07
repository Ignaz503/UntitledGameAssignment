using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.CustomDebug;

public class BoxCollider : Component, IUpdate
{
    SpriteRenderer SpriteRen;
    public Rectangle BoundingBox;
    public List<GameObject> Collisions { get; private set; }
    public bool IsKinematic { get; private set; }

    public BoxCollider( SpriteRenderer spriteRen, GameObject obj, bool isKinematic = true ) : base( obj )
    {
        SpriteRen = spriteRen;
        BoundingBox = new Rectangle(this.GameObject.Transform.Position.ToPoint(), SpriteRen.Sprite.Bounds.Size);
        Collisions = new List<GameObject>();
        IsKinematic = isKinematic;
    }

    public override void OnDestroy()
    {}

    public void Update()
    {
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
    /// If next move would cause gameobject to collide, set axis to 0
    /// </summary>
    /// <param name="query"></param>
    public void UpdateVelocity()
    {
        foreach (GameObject collider in Collisions)
        {
            BoxCollider query = collider.GetComponent<BoxCollider>();
            if (query.IsKinematic)
            {
                Vector2 newVelocity = Transform.Velocity;
                if ((Transform.Velocity.X > 0 && this.IsTouchingLeft(query)) ||
                    (Transform.Velocity.X < 0 && this.IsTouchingRight(query)))
                {
                    newVelocity.X *= -0.001f;
                }
                if ((Transform.Velocity.Y > 0 && this.IsTouchingTop(query)) ||
                    (Transform.Velocity.Y < 0 && this.IsTouchingBottom(query)))
                {
                    newVelocity.Y *= -0.001f;
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
            BoundingBox.Top < qBox.Bottom;
    }

    public bool IsTouchingRight(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Left - Transform.Velocity.X < qBox.Right &&
            BoundingBox.Right > qBox.Right &&
            BoundingBox.Bottom > qBox.Top &&
            BoundingBox.Top < qBox.Bottom;
    }

    public bool IsTouchingTop(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Bottom + Transform.Velocity.Y > qBox.Top &&
            BoundingBox.Top < qBox.Top &&
            BoundingBox.Right > qBox.Left &&
            BoundingBox.Left < qBox.Right;
    }

    public bool IsTouchingBottom(BoxCollider query)
    {
        Rectangle qBox = query.BoundingBox;
        return BoundingBox.Top - Transform.Velocity.Y < qBox.Bottom &&
            BoundingBox.Bottom > qBox.Bottom &&
            BoundingBox.Right > qBox.Left &&
            BoundingBox.Left < qBox.Right;
    }

    public bool IsTouching(BoxCollider query)
    {
        return IsTouchingTop(query) || IsTouchingBottom(query) || IsTouchingLeft(query) || IsTouchingRight(query);
    }
}

