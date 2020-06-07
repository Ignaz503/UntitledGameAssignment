using System;
using System.Runtime.Serialization;

namespace Util.Input
{
    /// <summary>
    /// trhown when input exception is encountered
    /// </summary>
    public class InputException : Exception
    {

        public InputException( string message ) : base( message )
        {}

        public InputException( string message, Exception innerException ) : base( message, innerException )
        {
        }
        protected InputException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }

}
