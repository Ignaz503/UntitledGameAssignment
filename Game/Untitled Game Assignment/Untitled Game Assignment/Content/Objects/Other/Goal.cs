using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;

class Goal : Component, IUpdate
{
    public event Action<Goal,GameObject> OnPlayerReachedGoal;
    GameObject player;
    BoxCollider ownCollider;

    public Goal( GameObject player, BoxCollider collider, GameObject self ) : base( self ) 
    {
        this.player = player;
        ownCollider = collider;
    }

    public override void OnDestroy()
    {
        player = null;
        ownCollider = null;
    }

    public void Update()
    {
        if (ownCollider.Collisions.Count > 0)
        {
            for (int i = 0; i < ownCollider.Collisions.Count; i++)
            {
                var gO = ownCollider.Collisions[i];
                if (gO == player)
                {
                    OnPlayerReachedGoal?.Invoke(this, player );
                }
            }
        }
    }
}

