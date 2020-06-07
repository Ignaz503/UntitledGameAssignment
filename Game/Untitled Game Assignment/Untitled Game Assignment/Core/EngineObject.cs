using System;

namespace UntitledGameAssignment.Core
{
    public abstract class EngineObject
    {
        static long idHeader = 0;
        static long IDGen => idHeader++;

        internal long ID { get; private set; }

        internal EngineObject() 
        {
            ID = IDGen;
        }

        public void Destroy() 
        {
            GameMain.Instance.MarkForDestruction( this );
        }

        public abstract void OnDestroy();

        internal abstract void Dispose();

        #region overrides
        public override bool Equals( object obj )
        {
            return obj is EngineObject @object &&
                   ID == @object.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + ID.GetHashCode();
        }

        public override string ToString()
        {
            return ID.ToString();
        }
        #endregion

        #region operrators
        public static bool operator ==( EngineObject lhs, EngineObject rhs ) 
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals( rhs );
        }

        public static bool operator !=( EngineObject lhs, EngineObject rhs ) 
        {
            return !(lhs == rhs);
        }
        #endregion

    }
}
