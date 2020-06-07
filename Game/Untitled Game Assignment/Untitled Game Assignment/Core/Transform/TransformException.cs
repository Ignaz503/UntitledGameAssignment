using System;
using System.Runtime.Serialization;
using UntitledGameAssignment.Core.GameObjects;

namespace UntitledGameAssignment.Core
{
    public class TransformException : Exception
    {
        public Transform Invokee { get; private set; }
        public TransformException(Transform t)
        {
            Invokee = t;
        }

        public TransformException( Transform t, string message ) : base( message )
        {
            Invokee = t;
        }

        public TransformException( Transform t, string message, Exception innerException ) : base( message, innerException )
        {
            Invokee = t;
        }

        protected TransformException( Transform t, SerializationInfo info, StreamingContext context ) : base( info, context )
        {
            Invokee = t;
        }

        public override string Message 
        {
            get 
            {
                string v = $"Object ID: {((GameObject)Invokee).ID}\n";
                v += base.Message;
                return v;
            }
        } 
    }

}
