using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;

class Health : Component
{
    public event Action<Health,GameObject>OnDeath;
    int maxHealth;
    int currentHitCount;

    public Health(int health, GameObject obj ) : base( obj )
    {
        this.maxHealth = health;
        currentHitCount = 0;
    }

    int HitCount 
    {
        get { return currentHitCount; }
        set
        {
            currentHitCount = value;
            if (currentHitCount >= maxHealth)
                OnDeath?.Invoke(this, this.GameObject );
        }
    }

    public void Hit(BulletBehaviour b) 
    {
        HitCount++;
        b.GameObject.Destroy();
    }

    public override void OnDestroy()
    {}
}

