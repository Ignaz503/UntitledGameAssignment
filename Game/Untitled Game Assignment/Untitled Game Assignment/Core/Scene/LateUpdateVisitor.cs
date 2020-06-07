using UntitledGameAssignment.Core.GameObjects;

namespace UntitledGameAssignment.Core.SceneGraph
{
    /// <summary>
    /// visits scene and invokes late update
    /// </summary>
    public class LateUpdateVisitor : SceneGraphVisitor
    {
        public LateUpdateVisitor() : base( recursion: true, enabledCheck: true )
        { }
        /// <summary>
        /// Invokes late update on node
        /// </summary>
        /// <param name="node">the node to invoke update on</param>
        public override void OnNodeVisit( GameObject node )
        {
            node.LateUpdateInvoke();
        }
    }

}