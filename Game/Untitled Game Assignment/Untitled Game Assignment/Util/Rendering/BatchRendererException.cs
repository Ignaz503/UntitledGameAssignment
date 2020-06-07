using System;
using System.Runtime.Serialization;

namespace Util.Rendering
{
    public class BatchRendererException : Exception
    {
        public BatchRendererException()
        {
        }

        public BatchRendererException( string message ) : base( message )
        {
        }

        public BatchRendererException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected BatchRendererException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}
