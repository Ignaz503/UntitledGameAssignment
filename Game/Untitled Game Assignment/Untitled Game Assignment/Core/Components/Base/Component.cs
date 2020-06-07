using UntitledGameAssignment.Core.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace UntitledGameAssignment.Core.Components
{
    /// <summary>
    /// basic component class
    /// </summary>
    public abstract class Component : EngineObject, IActiveState
    {
        /// <summary>
        /// the gameobject this component belongs to
        /// </summary>
        public GameObject GameObject { get; private set; }
        /// <summary>
        /// the transform of the gameobejct this component belongs to
        /// </summary>
        public Transform Transform => GameObject.Transform;

        public bool IsEnabled { get; set; }

        public Component(GameObject obj)
        {
            this.GameObject = obj;
            IsEnabled = true;
        }

        /// <summary>
        /// disposes of component by removeing it from gameobject it belongs to
        /// On destroy has been called before
        /// </summary>
        internal override void Dispose()
        {
            GameObject.RemoveComponent( this );
            GameObject = null;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public void Toggle()
        {
            IsEnabled = !IsEnabled;
        }

        public void SetEnabled( bool value )
        {
            IsEnabled = value;
        }

    }
}
