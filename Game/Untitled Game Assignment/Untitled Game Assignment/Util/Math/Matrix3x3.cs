using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Util.CustomMath
{
    public struct Matrix3x3
    {
        public float m00, m01, m02,
                     m10, m11, m12,
                     m20, m21, m22;
        #region ctors
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Matrix3x3( float val )
        {
            m00 = m01 = m02 =
            m10 = m11 = m12 =
            m20 = m21 = m22 = val;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Matrix3x3( float m00, float m01, float m02,
                          float m10, float m11, float m12,
                          float m20, float m21, float m22 )
        {
            this.m00 = m00; this.m01 = m01; this.m02 = m02;
            this.m10 = m10; this.m11 = m11; this.m12 = m12;
            this.m20 = m20; this.m21 = m21; this.m22 = m22;
        }


        public Matrix3x3( Vector3 col0, Vector3 col1, Vector3 col2 )
        {
            this.m00 = col0.X; this.m01 = col1.X; this.m02 = col2.X;
            this.m10 = col0.Y; this.m11 = col1.Y; this.m12 = col2.Y;
            this.m20 = col0.Z; this.m21 = col1.Z; this.m22 = col2.Z;
        }

        public Matrix3x3( Matrix3x3 src )
        {
            this.m00 = src.m00; this.m01 = src.m01; this.m02 = src.m02;
            this.m10 = src.m10; this.m11 = src.m11; this.m12 = src.m12;
            this.m20 = src.m20; this.m21 = src.m21; this.m22 = src.m22;
        }

        #endregion

        #region conversion
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator Matrix3x3( float v )
        { return new Matrix3x3( v ); }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static explicit operator Matrix( Matrix3x3 v )
        {
            return new Matrix( v.m00, v.m01, v.m02, 0f,
                               v.m10, v.m11, v.m12, 0f,
                               v.m20, v.m21, v.m22, 0f,
                               0f, 0f, 0f, 1f );
        }

        #endregion

        #region static
        /// <summary>
        /// creates an 3x3 identity matrix
        /// </summary>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 Identity()
        {
            return new Matrix3x3( 1f, 0f, 0f,
                                  0f, 1f, 0f,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// makes a translation matrix
        /// </summary>
        /// <param name="t">the translaktion parameters</param>
        /// <returns>a translation matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeTranslation( Vector2 t )
        {
            return new Matrix3x3( 1f, 0f, t.X,
                                  0f, 1f, t.Y,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// makes a translation matrix
        /// </summary>
        /// <param name="x">x axis translation</param>
        /// <param name="y">y axis translation</param>
        /// <returns>a translation matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeTranslation( float x, float y )
        {
            return new Matrix3x3( 1f, 0f, x,
                                  0f, 1f, y,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// make a rotation matrix from radians
        /// </summary>
        /// <param name="rad">given radians</param>
        /// <returns>a rotation matrix ( Z axis )</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeRotation( float rad )
        {
            float cos = (float)System.Math.Cos(rad);
            float sin = (float)System.Math.Sin(rad);
            return new Matrix3x3( cos, -sin, 0f,
                                  sin, cos, 0f,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// make a rotation matrix from degree
        /// </summary>
        /// <param name="rad">given degree</param>
        /// <returns>a rotation matrix ( Z axis )</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeRotationDeg( float deg )
        {
            return MakeRotation( deg * Mathf.Deg2Rad );
        }

        /// <summary>
        /// make scaling matrix
        /// </summary>
        /// <param name="scale">the x and y axis scaling</param>
        /// <returns>a scaling matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeScale( Vector2 scale )
        {
            return new Matrix3x3( scale.X, 0f, 0f,
                                  0f, scale.Y, 0f,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// makes a scale matrix
        /// </summary>
        /// <param name="x">x axis scaling</param>
        /// <param name="y">y axis scaling</param>
        /// <returns>a scale matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeScale( float x, float y )
        {
            return new Matrix3x3( x, 0f, 0f,
                                  0f, y, 0f,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// makes a scale matrix
        /// </summary>
        /// <param name="s">both axis scaling</param>
        /// <returns>a scale matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 MakeScale( float s )
        {
            return new Matrix3x3( s, 0f, 0f,
                                  0f, s, 0f,
                                  0f, 0f, 1f );
        }

        /// <summary>
        /// Invert a translation matrix (returns new matrix)
        /// cause compiler makes safety copy
        /// </summary>
        /// <param name="mat">translation matrix to invert</param>
        /// <returns>a translation matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 InvertTranslation( Matrix3x3 mat )
        {
            mat.m02 = -mat.m02;
            mat.m12 = -mat.m12;
            return mat;
        }

        /// <summary>
        /// inverts translation matrix in place returns ref to same matrix as taken in for chaining
        /// </summary>
        /// <param name="mat">the translation matrix to invert</param>
        /// <returns>the ref to mat inverted</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ref Matrix3x3 InvertTranslation( ref Matrix3x3 mat )
        {
            mat.m02 = -mat.m02;
            mat.m12 = -mat.m12;
            return ref mat;
        }

        /// <summary>
        /// Invert a rotation matrix (returns new matrix)
        /// cause compiler makes safety copy
        /// </summary>
        /// <param name="mat">rotation matrix to invert</param>
        /// <returns>a rotation matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 InvertRotation( Matrix3x3 mat )
        {
            return mat.Transpose();
        }

        /// <summary>
        /// inverts rotation matrix in place returns ref to same matrix as taken in for chaining
        /// </summary>
        /// <param name="mat">the rotation matrix to invert</param>
        /// <returns>the ref to mat inverted</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ref Matrix3x3 InvertRotation( ref Matrix3x3 mat )
        {
            mat.Transpose();
            return ref mat;
        }

        /// <summary>
        /// Invert a scale matrix (returns new matrix)
        /// cause compiler makes safety copy
        /// </summary>
        /// <param name="mat">scale matrix to invert</param>
        /// <returns>a scale matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 InvertScale( Matrix3x3 mat )
        {
            mat.m00 = 1f / mat.m00;
            mat.m11 = 1f / mat.m11;
            return mat;
        }

        /// <summary>
        /// inverts scale matrix in place returns ref to same matrix as taken in for chaining
        /// </summary>
        /// <param name="mat">the scale matrix to invert</param>
        /// <returns>the ref to mat inverted</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ref Matrix3x3 InvertScale( ref Matrix3x3 mat )
        {
            mat.m00 = 1f / mat.m00;
            mat.m11 = 1f / mat.m11;
            return ref mat;
        }

        /// <summary>
        /// Inverts a matrix
        /// </summary>
        /// <param name="mat">the matrix to invert</param>
        /// <returns>an inverted matrix</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 Invert( Matrix3x3 mat )
        {
            //calc determinante
            float det = mat.Determinante();
            if (det == 0f)
                throw new MatrixException( "No inverse exists for this matrix\n" + mat.ToString() );

            //build transpose
            var transpose = Transpose(mat);

            //calc 2x2 sub matrices determinantes
            // m_r_c the row excluded and the column excluded
            float m_1_1 = (transpose.m11*transpose.m22) - (transpose.m12*transpose.m21);
            float m_1_2 = (transpose.m10*transpose.m22) - (transpose.m12*transpose.m20);
            float m_1_3 = (transpose.m10*transpose.m21) - (transpose.m11*transpose.m20);

            float m_2_1 = (transpose.m01*transpose.m22) - (transpose.m02*transpose.m21);
            float m_2_2 = (transpose.m00*transpose.m22) - (transpose.m02*transpose.m20);
            float m_2_3 = (transpose.m00*transpose.m21) - (transpose.m01*transpose.m20);

            float m_3_1 = (transpose.m01*transpose.m12) - (transpose.m02*transpose.m11);
            float m_3_2 = (transpose.m00*transpose.m12) - (transpose.m02*transpose.m10);
            float m_3_3 = (transpose.m00*transpose.m11) - (transpose.m01*transpose.m10);

            var adjunctMat = new Matrix3x3(
                    +m_1_1, -m_1_2, +m_1_3,
                    -m_2_1 ,+m_2_2, -m_2_3,
                    +m_3_1, -m_3_2, +m_3_3);
            return (1f / det) * adjunctMat;
        }

        /// <summary>
        /// Inverts a matrix
        /// </summary>
        /// <param name="mat">the matrix to invert</param>
        /// <returns>an inverted matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ref Matrix3x3 Invert( ref Matrix3x3 mat )
        {
            //calc determinante
            float det = mat.Determinante();
            if (det == 0f)
                throw new MatrixException( "No inverse exists for this matrix\n" + mat.ToString() );

            //build transpose
            Transpose(ref mat);

            //calc 2x2 sub matrices determinantes
            // m_r_c the row excluded and the column excluded
            float m_1_1 = (mat.m11*mat.m22) - (mat.m12*mat.m21);
            float m_1_2 = (mat.m10*mat.m22) - (mat.m12*mat.m20);
            float m_1_3 = (mat.m10*mat.m21) - (mat.m11*mat.m20);

            float m_2_1 = (mat.m01*mat.m22) - (mat.m02*mat.m21);
            float m_2_2 = (mat.m00*mat.m22) - (mat.m02*mat.m20);
            float m_2_3 = (mat.m00*mat.m21) - (mat.m01*mat.m20);

            float m_3_1 = (mat.m01*mat.m12) - (mat.m02*mat.m11);
            float m_3_2 = (mat.m00*mat.m12) - (mat.m02*mat.m10);
            float m_3_3 = (mat.m00*mat.m11) - (mat.m01*mat.m10);

            mat = (1f / det) * new Matrix3x3(
                    +m_1_1, -m_1_2, +m_1_3,
                    -m_2_1 ,+m_2_2, -m_2_3,
                    +m_3_1, -m_3_2, +m_3_3);
            return ref mat;
        }

        /// <summary>
        /// make transpose of matrix (return copy)
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static  Matrix3x3 Transpose(Matrix3x3 mat)
        {
            var c0 = mat.col0;
            var c1 = mat.col1;
            var c2 = mat.col2;

            mat.row0 = c0;
            mat.row1 = c1;
            mat.row2 = c2;
            return mat;
        }

        /// <summary>
        /// make transpose of matrix in place
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ref Matrix3x3 Transpose( ref Matrix3x3 mat )
        {
            var c0 = mat.col0;
            var c1 = mat.col1;
            var c2 = mat.col2;

            mat.row0 = c0;
            mat.row1 = c1;
            mat.row2 = c2;
            return  ref mat;
        }
         
        /// <summary>
        /// make transpose of matrix (return copy)
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static  float Determinante(Matrix3x3 mat)
        {
            return mat.Determinante();
        }

        /// <summary>
        /// make transpose of matrix in place
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float Determinante( ref Matrix3x3 mat )
        {
            return mat.Determinante();
        }

        #endregion

        #region getter setter
        /// <summary>
        /// first column of matrix
        /// </summary>
        public Vector3 col0
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return new Vector3( m00, m10, m20 ); }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { this.m00 = value.X; this.m10 = value.Y; this.m20 = value.Z; }
        }

        /// <summary>
        /// second column of matrix
        /// </summary>
        public Vector3 col1
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return new Vector3( m01, m11, m21 ); }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { this.m01 = value.X; this.m11 = value.Y; this.m21 = value.Z; }
        }
        /// <summary>
        /// third column of matrix
        /// </summary>
        public Vector3 col2
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return new Vector3( m02, m12, m22 ); }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { this.m02 = value.X; this.m12 = value.Y; this.m22 = value.Z; }
        }

        /// <summary>
        /// first row of matrix
        /// </summary>
        public Vector3 row0
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return new Vector3( m00, m01, m02 ); }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { this.m00 = value.X; this.m01 = value.Y; this.m02 = value.Z; }
        }

        /// <summary>
        /// second row of matrix
        /// </summary>
        public Vector3 row1
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return new Vector3( m10, m11, m21 ); }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { this.m10 = value.X; this.m11 = value.Y; this.m12 = value.Z; }
        }

        /// <summary>
        /// third row of matrix
        /// </summary>
        public Vector3 row2
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get { return new Vector3( m20, m21, m22 ); }
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set { this.m20 = value.X; this.m21 = value.Y; this.m22 = value.Z; }
        }
        #endregion

        #region functions

        /// <summary>
        /// make transpose of matrix
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public Matrix3x3 Transpose()
        {
            var c0 = col0;
            var c1 = col1;
            var c2 = col2;

            row0 = c0;
            row1 = c1;
            row2 = c2;
            return this;
        }

        /// <summary>
        /// calculate determinante of matrix
        /// </summary>
        /// <returns>the determinante of the matrix</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Determinante()
        {
            return (m00 * m11 * m22) + (m01 * m12 * m20) + (m10 * m21 * m02) - (m02 * m11 * m20) - (m01 * m10 * m22) - (m12 * m21 * m00);
        }

        /// <summary>
        /// inverts this matrix
        /// </summary>
        public void Invert()
        {
            Invert( ref this );
        }


        public override string ToString()
        {
            return $"|{m00},{m01},{m02}|\n|{m10},{m11},{m12}|\n|{m20},{m21},{m22}|\n";

        }
        #endregion

        #region operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x3 operator *( float scalar, Matrix3x3 rhs ) 
        {
            return new Matrix3x3(
                scalar * rhs.m00, scalar * rhs.m01, scalar * rhs.m02,
                scalar * rhs.m10, scalar * rhs.m11, scalar * rhs.m12,
                scalar * rhs.m20, scalar * rhs.m21, scalar * rhs.m22
                );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 operator *( Matrix3x3 lhs, float scalar)
        {
            return new Matrix3x3(
                lhs.m00 * scalar, lhs.m01 * scalar, lhs.m02 * scalar,
                lhs.m10 * scalar, lhs.m11 * scalar, lhs.m12 * scalar,
                lhs.m20 * scalar, lhs.m21 * scalar, lhs.m22 * scalar
                );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 operator *( Matrix3x3 lhs, Matrix3x3 rhs ) 
        {
            return new Matrix3x3(
                (lhs.m00 * rhs.m00) + (lhs.m01 * rhs.m10) + (lhs.m02 * rhs.m20),
                (lhs.m00 * rhs.m01) + (lhs.m01 * rhs.m11) + (lhs.m02 * rhs.m21),
                (lhs.m00 * rhs.m02) + (lhs.m01 * rhs.m12) + (lhs.m02 * rhs.m22),

                (lhs.m10 * rhs.m00) + (lhs.m11 * rhs.m10) + (lhs.m12 * rhs.m20),
                (lhs.m10 * rhs.m01) + (lhs.m11 * rhs.m11) + (lhs.m12 * rhs.m21),
                (lhs.m10 * rhs.m02) + (lhs.m11 * rhs.m12) + (lhs.m12 * rhs.m22),

                (lhs.m20 * rhs.m00) + (lhs.m21 * rhs.m10) + (lhs.m22 * rhs.m20),
                (lhs.m20 * rhs.m01) + (lhs.m21 * rhs.m11) + (lhs.m22 * rhs.m21),
                (lhs.m20 * rhs.m02) + (lhs.m21 * rhs.m12) + (lhs.m22 * rhs.m22) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector3 operator *( Matrix3x3 lhs, Vector3 rhs ) 
        {
            return new Vector3(
                lhs.m00*rhs.X+lhs.m01*rhs.Y+lhs.m02*rhs.Z,
                lhs.m10*rhs.X+lhs.m11*rhs.Y+lhs.m12*rhs.Z,
                lhs.m20*rhs.X+lhs.m21*rhs.Y+lhs.m22*rhs.Z);
        } 
        
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector3 operator *( Vector3 lhs, Matrix3x3 rhs ) 
        {
            return new Vector3(
            lhs.X * rhs.m00 + lhs.Y *  rhs.m10 + lhs.Z* rhs.m20,
            lhs.X * rhs.m01 + lhs.Y *  rhs.m11 + lhs.Z* rhs.m21,
            lhs.X * rhs.m02 + lhs.Y *  rhs.m12 + lhs.Z* rhs.m22);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator *( Matrix3x3 lhs, Vector2 rhs ) 
        {
            var v3 = lhs*new Vector3(rhs.X,rhs.Y,1f);
            return new Vector2( v3.X / v3.Z, v3.Y / v3.Z );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Vector2 operator *( Vector2 lhs, Matrix3x3 rhs )
        {
            var v3 = new Vector3(lhs.X,lhs.Y,1f) * rhs;
            return new Vector2( v3.X / v3.Z, v3.Y / v3.Z );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 operator +( Matrix3x3 lhs, Matrix3x3 rhs ) 
        {
            return new Matrix3x3(
                lhs.m00 + rhs.m00, lhs.m01 + rhs.m01, lhs.m02 + rhs.m02,
                lhs.m10 + rhs.m10, lhs.m11 + rhs.m11, lhs.m12 + rhs.m12,
                lhs.m20 + rhs.m20, lhs.m21 + rhs.m21, lhs.m22 + rhs.m22 );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static Matrix3x3 operator -( Matrix3x3 lhs, Matrix3x3 rhs ) 
        {
            return new Matrix3x3(
                lhs.m00 - rhs.m00, lhs.m01 - rhs.m01, lhs.m02 - rhs.m02,
                lhs.m10 - rhs.m10, lhs.m11 - rhs.m11, lhs.m12 - rhs.m12,
                lhs.m20 - rhs.m20, lhs.m21 - rhs.m21, lhs.m22 - rhs.m22 );
        }

        #endregion
    }
}
