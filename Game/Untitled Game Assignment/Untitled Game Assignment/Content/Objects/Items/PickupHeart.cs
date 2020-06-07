using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;
using Microsoft.Xna.Framework;
using Util.CustomDebug;
using Util.SortingLayers;

public class PickupHeart : GameObject
{

    public PickupHeart( Vector2 position, GameObject heal, Color c)
    {
        Transform.Position = position;

        SpriteRenderer spriteRen = AddComponent((obj) => new SpriteRenderer("Sprites/heart", c , SortingLayer.EntitesSubLayer(1), obj));

        AddComponent( (obj) => new BoxCollider(spriteRen, obj) );

        AddComponent((obj) => new PickupHeartBehaviour(obj, heal, c));

    }
}