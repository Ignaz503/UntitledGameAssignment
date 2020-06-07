using UntitledGameAssignment.Core.GameObjects;

namespace UntitledGameAssignment.Core.SceneGraph
{
    public abstract class SceneGraphVisitor
    {
        /// <summary>
        /// Settings for this visitor
        /// </summary>
        internal VisitSettings Settings { get; private set; }

        protected SceneGraphVisitor( VisitSettings settings )
        {
            Settings = settings;
        }

        protected SceneGraphVisitor(bool recursion, bool enabledCheck)
        {
            Settings = new VisitSettings( recursion, enabledCheck );
        }

        /// <summary>
        /// On Node visit invoked
        /// </summary>
        /// <param name="node">the current node gameobject</param>
        public abstract void OnNodeVisit( GameObject node);

        /// <summary>
        /// current transform visiting
        /// </summary>
        /// <param name="t">the current node</param>
        internal void CurrentNodeVisiting( Transform t ) 
        {
            OnNodeVisit( t.GameObject );
        }

        /// <summary>
        /// starts visiting a specific scene
        /// </summary>
        /// <param name="s"></param>
        public void Start( Scene s ) 
        {
            OnStart();
            s.Visit( this );
        }

        public virtual void OnStart() 
        {}

        public virtual void OnEnd() 
        {}

        /// <summary>
        /// starts visiting currently open scene
        /// </summary>
        public void Start() 
        {
            Start( Scene.Current );
        }

        public struct VisitSettings 
        {
            /// <summary>
            /// setting for recursive visit to childrens children
            /// </summary>
            public bool ChildRecursion { get; set; }
            /// <summary>
            /// settting if children need to be enabled for visit
            /// </summary>
            public bool EnabledCheck { get; set; }

            public VisitSettings( bool childRecursion, bool enabledCheck )
            {
                ChildRecursion = childRecursion;
                EnabledCheck = enabledCheck;
            }
        }
    }

}