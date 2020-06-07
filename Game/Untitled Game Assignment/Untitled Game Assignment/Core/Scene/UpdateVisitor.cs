using UntitledGameAssignment.Core.GameObjects;

namespace UntitledGameAssignment.Core.SceneGraph
{
    /// <summary>
    /// visits scene and invokes update
    /// </summary>
    public class UpdateVisitor : SceneGraphVisitor
    {
        public UpdateVisitor():base(recursion:true,enabledCheck:true)
        {}

        /// <summary>
        /// invokes update on node
        /// </summary>
        /// <param name="node">the node we invoke update on</param>
        public override void OnNodeVisit( GameObject node )
        {
            node.UpdateInvoke();
        }
    }

    public class DestroyVisitor : SceneGraphVisitor
    {
        public DestroyVisitor() : base( recursion: false, enabledCheck: false )
        { }

        /// <summary>
        /// invokes update on node
        /// </summary>
        /// <param name="node">the node we invoke update on</param>
        public override void OnNodeVisit( GameObject node )
        {
            node.Destroy();
        }
    }

}