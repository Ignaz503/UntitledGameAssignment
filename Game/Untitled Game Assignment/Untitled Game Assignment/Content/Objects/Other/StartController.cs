using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using Util.Input;
using Util.CustomDebug;

class StartController : Component, IUpdate
{
    List<GameObject> objects;

    public StartController( GameObject obj ) : base( obj ) 
    {
    }

    public override void OnDestroy()
    {
    }

    public void Update()
    {
        if (Input.IsKeyDown(Keys.Enter))
        {
            foreach( GameObject h in objects )
            {
                h.Enable();
            }
            this.GameObject.Destroy();
        }
        
    }
    public void Load()
    {
        objects = Scene.Current.gameObjects;
        foreach (GameObject g in objects)
        {
            Debug.Log(g.Name);
            if (g.Name != ("Start"))
                g.Disable();
        }
    }
}

