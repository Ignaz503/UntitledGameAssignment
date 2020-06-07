using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UntitledGameAssignment.Core.Components;
using Util.CustomDebug;

namespace UntitledGameAssignment.Core.GameObjects
{
    /// <summary>
    /// the gameobject class a collection of components at a certain location in world space
    /// </summary>
    public class GameObject : EngineObject, IActiveState
    {
        internal static event Action<GameObject> OnCreated;
        internal static event Action<GameObject> OnDisposed;

        public event Action<GameObject> OnDestroying;

        /// <summary>
        /// The transform of this gameobject
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// is the gameobejct enabled or not
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// name of the gameobject
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// list of all components
        /// </summary>
        List<Component> components;

        /// <summary>
        /// collections of components that have a fixed update function
        /// </summary>
        FixedUpdateInvoker fixedUpdateInvoker;
        /// <summary>
        /// collection of components that have an update function
        /// </summary>
        UpdateInvoker updateInvoker;
        /// <summary>
        /// collection of components that have a late update function
        /// </summary>
        LateUpdateInvoker lateUpdateInvoker;
        /// <summary>
        /// collection of components that have a draw call
        /// </summary>
        DrawInvoker drawInvoker;

        public GameObject()
        {
            Transform = new Transform( this, null );
            Init();
        }

        public GameObject( Transform parent ) 
        {
            Transform = new Transform( this, parent );
            Init();
        }

        public GameObject( Transform parent, Vector2 position ) 
        {
            Transform = new Transform( this, parent, position );
            Init();
        }

        public GameObject( Transform parent, Vector2 position, float rotation ) 
        {
            Transform = new Transform( this, parent, position, rotation );
            Init();
        }

        /// <summary>
        /// initializes the components lists and collections
        /// </summary>
        void Init() 
        {
            components = new List<Component>();
            fixedUpdateInvoker = new FixedUpdateInvoker();
            updateInvoker = new UpdateInvoker();
            lateUpdateInvoker = new LateUpdateInvoker();
            drawInvoker = new DrawInvoker();
            Name = ID.ToString();
            IsEnabled = true;
            OnCreated?.Invoke( this );
        }

        /// <summary>
        /// when gameobject destroyed clled cascades destruction to all components
        /// </summary>
        public override void OnDestroy()
        {
            for (int i = components.Count-1; i >= 0; i--)
            {
                components[i].OnDestroy();
            }
            Transform.OnDestroy();
            //transform children on destroy
            Transform.TraverseChildren( ( t ) => t.GameObject.OnDestroy(), childrenTraversalRecursion: true, checkIfEnabledForRecursion: false );
            OnDestroying?.Invoke( this );
        }

        /// <summary>
        /// disposes this gameobject and all the components
        /// </summary>
        internal override void Dispose()
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                components[i].Dispose();
            }
            //transform children
            Transform.TraverseChildren( ( t ) => t.GameObject.Dispose(), childrenTraversalRecursion: true, checkIfEnabledForRecursion: false );

            components.Clear();
            fixedUpdateInvoker.Clear();
            updateInvoker.Clear();
            lateUpdateInvoker.Clear();
            drawInvoker.Clear();

            OnDisposed?.Invoke( this );
        }

        /// <summary>
        /// adds component to gameobject
        /// </summary>
        /// <typeparam name="T">The type of the component(implicit)</typeparam>
        /// <param name="generator">a generator function for that component type</param>
        /// <returns>the newly genrated component</returns>
        public T AddComponent<T>( Func<GameObject,T> generator ) where T : Component
        {
            T comp = generator(this);

            //todo add to correct collection
            components.Add( comp );
            AddComponentToCorrectInvokers( comp );

            return comp;
        }

        /// <summary>
        /// gets a component of a certain type
        /// </summary>
        /// <typeparam name="T">the type of component wanted</typeparam>
        /// <returns>the first component found of this type, or null if no component of this type</returns>
        public T GetComponent<T>() where T: Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T val)
                    return val;
            }
            return null;
        }
        
        /// <summary>
        /// gets component of a certain type
        /// </summary>
        /// <param name="t">the type of component</param>
        /// <returns>a component of type t, or null</returns>
        public Component GetComponent(Type t) 
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == t)
                    return components[i];
            }
            return null;
        }

        /// <summary>
        /// gets all components of type
        /// </summary>
        /// <typeparam name="T">the type of component</typeparam>
        /// <param name="searchChildren">serach children for component as well</param>
        /// <returns>a list of all components of a certain type</returns>
        public List<T> GetComponents<T>(bool searchChildren = false) where T : Component
        {
            List<T> comps = new List<T>();
            FillListWithCompsOfType( ref comps );

            if (searchChildren)
                Transform.TraverseChildren( ( t ) => t.GameObject.FillListWithCompsOfType( ref comps ), childrenTraversalRecursion: true, checkIfEnabledForRecursion: false );
            return comps;
        }  
        /// <summary>
        /// gets a list of components with a certain type
        /// </summary>
        /// <param name="t">the type of components</param>
        /// <param name="searchChildren">if we want to search it's children as well</param>
        /// <returns>a list of components with a certain type</returns>
        public List<Component> GetComponents(Type t, bool searchChildren = false)
        {
            List<Component> comps = new List<Component>();
            FillListWithCompsOfType( ref comps,t );

            if (searchChildren)
                Transform.TraverseChildren( ( trans ) => trans.GameObject.FillListWithCompsOfType( ref comps, t ), childrenTraversalRecursion: true, checkIfEnabledForRecursion: false );
            return comps;
        }

        /// <summary>
        /// fills a list with all components of type
        /// </summary>
        /// <typeparam name="T">type of component</typeparam>
        /// <param name="list">the list to fill</param>
        void FillListWithCompsOfType<T>( ref List<T> list ) where T:Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T val)
                    list.Add( val );
            }
        } 
        
        /// <summary>
        /// fills a list with components of a certain type
        /// </summary>
        /// <param name="list">the list to fill</param>
        /// <param name="t">the type searching </param>
        void FillListWithCompsOfType( ref List<Component> list, Type t )
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == t)
                    list.Add( components[i] );
            }
        }

        /// <summary>
        /// adds a component to the correct invokers
        /// </summary>
        /// <param name="component">the component to add</param>
        private void AddComponentToCorrectInvokers( Component component )
        {
            if (component is IUpdate uComp)
                updateInvoker.Add( uComp );
            if (component is IFixedUpdate fComp)
                fixedUpdateInvoker.Add( fComp );
            if (component is ILateUpdate lComp)
                lateUpdateInvoker.Add( lComp );
            if (component is IDraw dComp)
                drawInvoker.Add( dComp );
        }

        /// <summary>
        /// removes a component from a gameobject
        /// </summary>
        /// <param name="component">the component to remove</param>
        public void RemoveComponent( Component component ) 
        {
            components.Remove( component );
            RemoveFromInvokers( component );
        }

        /// <summary>
        /// removes a component from all the invokers
        /// </summary>
        /// <param name="component">the component to remove</param>
        private void RemoveFromInvokers(Component component)
        {
            if (component is IUpdate uComp)
                updateInvoker.Remove( uComp );
            if (component is IFixedUpdate fComp)
                fixedUpdateInvoker.Remove( fComp );
            if (component is ILateUpdate lComp)
                lateUpdateInvoker.Remove( lComp );
            if (component is Components.IDraw dComp)
                drawInvoker.Remove( dComp );
        }

        /// <summary>
        /// invokes the fixed update invoker
        /// </summary>
        internal void FixedUpdateInvoke() 
        {
            fixedUpdateInvoker.Invoke();
        }

        /// <summary>
        /// invokes the update invoker
        /// </summary>
        internal void UpdateInvoke()
        {
            updateInvoker.Invoke();
        }

        /// <summary>
        /// invokes the update invoker
        /// </summary>
        internal void LateUpdateInvoke()
        {
            lateUpdateInvoker.Invoke();
        }

        /// <summary>
        /// invokes the draw invoker
        /// </summary>
        internal void DrawInvoke()
        {
            drawInvoker.Invoke();
        }

        /// <summary>
        /// enables the gameobject
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
        }

        /// <summary>
        /// disables the gameobject
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
        }

        /// <summary>
        /// toggles the gameobject enabled state
        /// </summary>
        public void Toggle()
        {
            IsEnabled = !IsEnabled;
        }

        /// <summary>
        /// sets the gameobejct enabled state
        /// </summary>
        /// <param name="value">the value of the state</param>
        public void SetEnabled( bool value )
        {
            IsEnabled = value;
        }

        /// <summary>
        /// converts gameobject to transform explicitly
        /// maybe make implicit if not to confusing
        /// </summary>
        /// <param name="obj">the gameobject to convert</param>
        public static explicit operator Transform( GameObject obj ) 
        {
            return obj.Transform;
        }

        /// <summary>
        /// base class for component invocation
        /// </summary>
        /// <typeparam name="T">the type to invoke</typeparam>
        private abstract class Invoker<T> :Collection<T> where T : IActiveState 
        {
            public Invoker() : base()
            {

            }

            public Invoker( T firstItem ) : base()
            {
                Add( firstItem );
            }

            /// <summary>
            /// invokes elements in this collection
            /// </summary>
            public void Invoke()
            {
                for (int i = 0; i < Count; i++)
                {
                    var item = this[i];
                    if (item.IsEnabled)
                        InvokeElement(item);
                        
                }
            }

            /// <summary>
            /// invokes the element
            /// </summary>
            protected abstract void InvokeElement(T elem);
        }

        /// <summary>
        /// class to invoke fixed update components
        /// </summary>
        private class FixedUpdateInvoker : Invoker<IFixedUpdate>
        {
            protected override void InvokeElement( IFixedUpdate elem )
            {
                elem.FixedUpdate();
            }
        }

        /// <summary>
        /// invokes an update from a component
        /// </summary>
        private class UpdateInvoker : Invoker<IUpdate>
        {
            protected override void InvokeElement( IUpdate elem )
            {
                elem.Update();
            }
        }

        /// <summary>
        /// invokes a late update on a component
        /// </summary>
        private class LateUpdateInvoker : Invoker<ILateUpdate>
        {
            protected override void InvokeElement( ILateUpdate elem )
            {
                elem.LateUpdate();
            }
        }

        /// <summary>
        /// invokes a draw call on a component
        /// </summary>
        private class DrawInvoker : Invoker<Components.IDraw>
        {
            protected override void InvokeElement( Components.IDraw elem )
            {
                elem.Draw();
            }
        }
    }
}
