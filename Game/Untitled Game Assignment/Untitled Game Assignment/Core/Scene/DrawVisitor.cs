using UntitledGameAssignment.Core.Components;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomDebug;
using Util.Rendering;

namespace UntitledGameAssignment.Core.SceneGraph
{
    /// <summary>
    /// visits scene and invokes the draw update
    /// </summary>
    public class DrawVisitor : SceneGraphVisitor
    {
        public DrawVisitor() : base( recursion: true, enabledCheck: true )
        { }
        /// <summary>
        /// invoke draw on node
        /// </summary>
        /// <param name="node">the node to invoke draw on</param>
        public override void OnNodeVisit( GameObject node )
        {
            node.DrawInvoke();
        }

        public override void OnStart()
        {
            //BatchRenderer.SetViewTransformMatrix( Camera.Active.GetViewMatrix() );
            //BatchRenderer.Begin();
            SortedBatchRenderer.SetViewTransformMatrix( Camera.Active.GetViewMatrix() );
            SortedBatchRenderer.Begin();
            base.OnStart();
        }

        public override void OnEnd() 
        {
            //BatchRenderer.End();
            SortedBatchRenderer.End();
        }
    }

}