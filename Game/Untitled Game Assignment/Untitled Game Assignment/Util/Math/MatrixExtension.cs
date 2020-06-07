using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace Util.CustomMath
{
    public static class MatrixExtension
    {
        public static string Stringify( this Matrix m ) 
        {
            StringBuilder b = new StringBuilder();

            b.Append( "|" ).Append(m.M11).Append(", ").Append( m.M12 ).Append( ", " ).Append( m.M13 ).Append( ", " ).Append( m.M14 ).Append( ", " ).AppendLine( "|" )
             .Append( "|" ).Append( m.M21 ).Append( ", " ).Append( m.M22 ).Append( ", " ).Append( m.M23 ).Append( ", " ).Append( m.M24 ).Append( ", " ).AppendLine( "|" )
             .Append( "|" ).Append( m.M31 ).Append( ", " ).Append( m.M32 ).Append( ", " ).Append( m.M33 ).Append( ", " ).Append( m.M34 ).Append( ", " ).AppendLine( "|" )
             .Append( "|" ).Append( m.M41 ).Append( ", " ).Append( m.M42 ).Append( ", " ).Append( m.M43 ).Append( ", " ).Append( m.M44 ).Append( ", " ).AppendLine( "|" );

            return b.ToString();
        }
    }

}
