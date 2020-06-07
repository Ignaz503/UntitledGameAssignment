using UntitledGameAssignment.Core.GameObjects;

namespace UntitledGameAssignment.Core.SceneGraph
{
    /// <summary>
    /// visits scene and invokes fixed update
    /// </summary>
    public class FixedUpdateVsitor : SceneGraphVisitor
    {
        public FixedUpdateVsitor() : base( recursion: true, enabledCheck: true )
        { }
        /// <summary>
        /// invokes fixed update on node
        /// </summary>
        /// <param name="node">the node to invoke update on</param>
        public override void OnNodeVisit( GameObject node )
        {
            node.FixedUpdateInvoke();
        }
    }

}