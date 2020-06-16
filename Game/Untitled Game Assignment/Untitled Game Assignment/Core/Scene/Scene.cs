using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.Components;


namespace UntitledGameAssignment.Core.SceneGraph
{
    public class Scene
    {
        public static Scene Current { get; private set; }

        public List<GameObject> gameObjects { get; private set; }

        public Transform Root { get; private set; }

        public Scene()
        {
        }

        void OnLoadScene()
        {
            gameObjects = new List<GameObject>();
            RegisterGameobjectEvents();

            Root = Transform.CreateSceneRoot();
            
            //probably temp
            Camera.CreateDefaultCamera();
        }

        /// <summary>
        /// loads new gameobject into scene
        /// </summary>
        /// <param name="newObject"></param>
        public void Instantiate(GameObject newObject)
        {
            if(!gameObjects.Contains(newObject))
                gameObjects.Add( newObject );
        }

        /// <summary>
        /// unloads scene
        /// </summary>
        void OnUnloadScene() 
        {
            UnregisterGameobjectEvents();
        }

        /// <summary>
        /// unregister from gameobject create and destroy events
        /// </summary>
        void UnregisterGameobjectEvents() 
        {
            GameObject.OnCreated -= OnGameObjectCreated;
            GameObject.OnDisposed -= OnGameobjectDestroyed;
        }

        /// <summary>
        /// registér to gameobject events ensuring no double registering
        /// </summary>
        void RegisterGameobjectEvents() 
        {
            //ensure no double regiser
            UnregisterGameobjectEvents();
            GameObject.OnCreated += OnGameObjectCreated;
            GameObject.OnDisposed += OnGameobjectDestroyed;
        }

        /// <summary>
        /// visits all nodes in scene from root
        /// </summary>
        /// <param name="visitor">the visitor we inform of all nodes</param>
        public void Visit(SceneGraphVisitor visitor) 
        {
            Root.TraverseChildren( visitor.CurrentNodeVisiting, childrenTraversalRecursion: visitor.Settings.ChildRecursion, checkIfEnabledForRecursion: visitor.Settings.EnabledCheck);
            visitor.OnEnd();
        }

        /// <summary>
        /// called when object destroyed 
        /// </summary>
        /// <param name="object">the destroyed object</param>
        private void OnGameobjectDestroyed( GameObject @object) 
        {
            gameObjects.Remove( @object );   
        }

        /// <summary>
        /// called when gameobejct created
        /// </summary>
        /// <param name="object">the created object</param>
        private void OnGameObjectCreated( GameObject @object) 
        {
            gameObjects.Add( @object );
        }


        /// <summary>
        /// finds a gameobject with a certain name
        /// </summary>
        /// <param name="name">the name of the object</param>
        /// <returns>null if no object of name true otherweise</returns>
        public GameObject FindObjectWithName( string name ) 
        {
            GameObject obj = null;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].Name == name)
                {
                    obj = gameObjects[i];
                    break;
                }    
            }

            return obj; 
        }



        /// <summary>
        /// loads a scene
        /// </summary>
        /// <param name="s">the scene to load</param>
        public static void LoadScene( Scene s ) 
        {
            if (Current != null)
                UnloadScene( Current );
            Current = s;
            //TODO probably more stuff
            s.OnLoadScene();
        }

        /// <summary>
        /// unloads a scene that is loaded
        /// </summary>
        /// <param name="loadedScene">the scene to unload</param>
        public static void UnloadScene( Scene loadedScene )
        {
            loadedScene.Visit( new DestroyVisitor() );
            loadedScene.OnUnloadScene();
            //TODO probably more stuff todo
        }
    }

}