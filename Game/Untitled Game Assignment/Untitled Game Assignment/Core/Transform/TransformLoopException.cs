using System;
using System.Runtime.Serialization;

namespace UntitledGameAssignment.Core
{
    public class TransformLoopException : TransformException
    {
        public TransformLoopException( Transform t, Transform loopCause ) : base( t, $"setting the parent of {t.GameObject.Name} to {loopCause.GameObject.Name} would cause a loop in the transform hierarchy" )
        {
        }
    }

}
