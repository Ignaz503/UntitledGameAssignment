using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UntitledGameAssignment.Core.GameObjects;
using UntitledGameAssignment.Core.SceneGraph;
using UntitledGameAssignment.Core.Components;
using Util.CustomMath;

namespace UntitledGameAssignment.Core
{
    /// <summary>
    /// Location of world and rotation manager of gameobject
    /// </summary>
    public class Transform
    {
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// the world position
        /// todo take rotation into consideration
        /// </summary>
        public Vector2 Position {
            get
            {
                return TransformPoint( Vector2.Zero );
            }
            set
            {
                if (HasParent)
                {
                    // similar to invert point function but with no translation for the this tansform step
                    Matrix trans= Matrix.Identity;

                    //TODO maybe switch
                    TraverseToRoot( ( pT ) => trans = pT.inverseTransform * trans);
                    LocalPosition = Vector2.Transform( value, trans );

                } else
                {
                    LocalPosition = value;
                }

            }
        }

        /// <summary>
        /// Velocity, relative position at next update
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// local translation matrix
        /// </summary>
        Matrix localTranslationMatrix;

        /// <summary>
        /// inverse local translation matrix
        /// </summary>
        Matrix inverseLocalTranslationMatrix =>  Matrix.CreateTranslation( -LocalPosition.X, -LocalPosition.Y, 0f );

        /// <summary>
        /// local position
        /// </summary>
        Vector2 localPos;

        /// <summary>
        /// local position in regard to parent
        /// </summary>
        public Vector2 LocalPosition
        {
            get { return localPos; }
            set { localPos = value; localTranslationMatrix = Matrix.CreateTranslation( value.X, value.Y, 0f ); }
        }

        /// <summary>
        /// local rotation matrix
        /// </summary>
        Matrix localRotationMatrix;

        /// <summary>
        /// inverse local rotaion matrix
        /// </summary>
        Matrix inverseLocalRotationMatrix 
        { 
            get 
            { 
                Matrix.Transpose( ref localRotationMatrix, out Matrix mat );
                return mat;
            }
        }

        /// <summary>
        /// local rotation in radians
        /// </summary>
        float localRot;

        /// <summary>
        /// local rotation in regard to parent accessor
        /// in radians
        /// </summary>
        public float LocalRotation 
        {
            get { return localRot; }
            set { localRot = value; localRotationMatrix = Matrix.CreateRotationZ( value ); }
        }

        /// <summary>
        /// world rotation
        /// in radians
        /// </summary>
        public float Rotation
        {
            get
            {
                float val = LocalRotation;
                TraverseToRoot( ( t ) => val += t.LocalRotation );
                return val;
            }
            set
            {
                float val = value;
                TraverseToRoot( ( t ) => val -= t.LocalRotation );
                LocalRotation = val;
            }
        }

        /// <summary>
        /// scale of object
        /// </summary>
        Matrix scaleMatrix;
        /// <summary>
        /// scale of object accessor
        /// setting treated as setting local scale(does not traverse parents and divdes through their scale)
        /// getting treated as getting world scale (traverses and multiplies parent scale)
        /// </summary>
        public Vector2 Scale 
        {
            get 
            {
                var p = scaleMatrix;

                TraverseToRoot( t => p = t.scaleMatrix * p );

                return new Vector2(p.M11,p.M22);
            }
            set 
            {
                scaleMatrix = Matrix.CreateScale( value.X, value.Y,1f);
            }
        }

        /// <summary>
        /// inverse of the scale 
        /// </summary>
        Matrix inverseScaleMatrix => Matrix.CreateScale( 1f / scaleMatrix.M11, 1f / scaleMatrix.M22, 1f );


        /// <summary>
        /// the transform matrix
        /// rot * scale * trans
        /// </summary>
        Matrix transform => localRotationMatrix * scaleMatrix * localTranslationMatrix;

        /// <summary>
        /// inverse transform matrix
        /// invserTrans * inverseScale * inverseRot
        /// </summary>
        Matrix inverseTransform => inverseLocalTranslationMatrix * inverseScaleMatrix * inverseLocalRotationMatrix;


        /// <summary>
        /// parent of this transform
        /// </summary>
        Transform parent;
        /// <summary>
        /// parent of this transform accessor
        /// </summary>
        public Transform Parent 
        {
            get => parent;
            set 
            {
                var oldWorldPos = Vector2.Zero;
                var oldWorldRot = 0f;
                var oldScale = Vector2.One;
                bool updatevalues = false;
                if (parent != null)
                {
                    oldWorldPos = Position;
                    oldWorldRot = Rotation;
                    oldScale = new Vector2(scaleMatrix.M11,scaleMatrix.M22);
                    updatevalues = true;
                }
                if (value != null && value != Scene.Current.Root)
                {
                    //ensure removed from prev parentage
                    //(parent value is never null as instead of null we set the value to scene Root)
                    if(parent != null) parent.RemoveChild( this );

                    //ensure there are no loops created
                    bool loop = false;
                    //traverse to root from parent and check if this is in that path
                    //if yes we would create a transform hierarchy loop, if no we have no problems
                    TraverseHierarchyToRoot( start: value, ( t ) => { if (t.Equals( this )) loop = true; } );
                    if(!loop)
                    {
                        parent = value;
                        //inform parent of having a new child
                        parent.AddChild( this );
                    } else
                        throw new TransformLoopException( this, value );
                } else
                {
                    //trying to unparent this transform
                    if (HasParent)
                        parent.RemoveChild( this );
                    parent = Scene.Current.Root;
                    parent.AddChild( this );
                }//end else value != null

                if (updatevalues)
                {
                    Rotation = oldWorldRot;
                    Scale = oldScale;
                    Position = oldWorldPos;
                }
            }
        }

        public Transform Root 
        {
            get 
            {
                var t = this;
                while (t.HasParent)
                {
                    t = t.Parent;
                }
                return t;
            }
        }

        /// <summary>
        /// check if has parent
        /// </summary>
        public bool HasParent => parent != null && parent != Scene.Current.Root;

        /// <summary>
        /// list of all children
        /// </summary>
        List<Transform> children;

        /// <summary>
        /// count of children
        /// </summary>
        public int ChildCount => children.Count;

        /// <summary>
        /// check if tranform has children
        /// </summary>
        public bool HasChildren => ChildCount > 0;

        private Transform() 
        {
            GameObject = null;
            this.parent = null;
            this.children = new List<Transform>();
            LocalPosition = Vector2.Zero;
            Scale = Vector2.One;
            LocalRotation = 0f;
        }

        internal Transform(GameObject obj,Transform parent)
        {
            this.GameObject = obj ?? throw new ArgumentNullException(nameof(GameObject));
            children = new List<Transform>();
            this.Parent = parent;
            LocalPosition = Vector2.Zero;
            Scale = Vector2.One;
            LocalRotation = 0f;
        }

        internal Transform( GameObject obj, Transform Parent, Vector2 position ) : this( obj, Parent )
        {
            this.Position = position;
        }

        internal Transform( GameObject obj, Transform parent, Vector2 position, float rotation ) : this( obj, parent, position )
        {
            this.Rotation = rotation;
        }
        
        internal Transform( GameObject obj, Transform parent, Vector2 position, float rotation ,Vector2 scale) : this( obj, parent, position, rotation)
        {
            this.Scale = scale;
        }

        /// <summary>
        /// adds child at the end
        /// </summary>
        /// <param name="newChild">the new child to add</param>
        void AddChild( Transform newChild) 
        {
             children.Add( newChild );
        }

        /// <summary>
        /// inserts a chil at given index
        /// </summary>
        /// <param name="newChild">the new child</param>
        /// <param name="idx">the index of where insertion is wanted</param>
        public void InsertChild( int idx , Transform newChild, bool setParentOfChild = true ) 
        {
            children.Insert( idx, newChild);
            if (setParentOfChild)
                newChild.Parent = this;
        }

        /// <summary>
        /// informs parent of destruction
        /// sets parent null via member not accessor
        /// children informed by recursion from gameobject
        /// </summary>
        internal void OnDestroy()
        {
            parent.RemoveChild( this );
            parent = null;
        }


        /// <summary>
        /// removes child at index
        /// MOVES CHILD TO SCENE ROOT(NULL Transform)
        /// </summary>
        /// <param name="idx"></param>
        public void RemoveChild( int idx ) 
        {
            if (idx > 0 && idx < children.Count)
            {
                children[idx].Parent = null;
                children.RemoveAt( idx );
            }
        }

        /// <summary>
        /// removes a child from the children collection
        /// DOES NOT INFORM CHILD OF REMOVAL
        /// </summary>
        /// <param name="t">child to remove</param>
        void RemoveChild( Transform t ) 
        {
            children.Remove( t );
        }

        /// <summary>
        /// transforms local point to world position
        /// </summary>
        /// <param name="localPoint"> a local point</param>
        /// <returns>a point in world space</returns>
        public Vector2 TransformPoint( Vector2 localPoint ) 
        {
            //we transform from this space to parent space to parent spcae to parent space to ...
            //in the end we get:
            //point = t0 * s0 * r0 .... * tn * sn * rn * localPoint
            //where tn sn pn are the matrices of this transform and the root is t-s-r0
            Matrix trans = transform;

            //if (GameObject != null && GameObject.Name == nameof( PathFollower ))
            //{
            //    Debug.WriteLine($"local rot: {localRotationMatrix}");
            //    Debug.WriteLine($"local scale: {scaleMatrix}");
            //    Debug.WriteLine($"local tanslation: {localTranslationMatrix}");
            //    Debug.WriteLine($"transform :{trans}");
            //}

            TraverseToRoot( ( pT ) => {
                    trans = trans * pT.transform;
            } );

            var r = Vector2.Transform( localPoint, trans );

            //if (HasParent)
            //    Debug.Log( $"{GameObject.Name} Rotation: {Rotation}" );

            return r;
        }

        /// <summary>
        /// transforms world point to local point
        /// </summary>
        /// <param name="worldPoint">a point in world space</param>
        /// <returns>a point in local space</returns>
        public Vector2 InverseTransformPoint( Vector2 worldPoint)
        {
            // we go from world inv translate to inv root, scale to root, inv rotate to root
            // and from there we do the same in reverse until we are at child
            // mat = irn * isn * itn * .... * ir0 * is0 * it0  
            Matrix trans= inverseTransform;

            TraverseToRoot( ( pT ) => trans = pT.inverseTransform * trans );
            return Vector2.Transform( worldPoint, trans );
        }

        /// <summary>
        /// transform local direction to world direction
        /// </summary>
        /// <param name="localDirection">a local dirction</param>
        /// <returns>the world direction</returns>
        public Vector2 TransformDirection( Vector2 localDirection ) 
        {
            return VectorMath.Rotate( localDirection, Rotation );
        }

        /// <summary>
        /// transforms world direction to local direction
        /// </summary>
        /// <param name="worldDirectin">a direction in world space</param>
        /// <returns>the direction in local space</returns>
        public Vector2 InverseTransformDirection(Vector2 worldDirection) 
        {
            float inverseRot = Mathf.InverseRadians(Rotation);
            return VectorMath.Rotate( worldDirection, inverseRot );
        }


        /// <summary>
        /// traverse from this transform to root node of this transform hierarchy
        /// </summary>
        /// <param name="actionForNodeOnPath">action taken for every node on this path</param>
        /// <param name="startWithParent">if true start with <c>this</c>.Parent else with <c>this</c></param>
        void TraverseToRoot( Action<Transform> actionForNodeOnPath ) 
        {
            if (actionForNodeOnPath == null)
                return;
            Transform t = this;
            while (t.HasParent)
            {
                t = t.Parent;
                actionForNodeOnPath( t );
            }
        }

        /// <summary>
        /// goes through all children and invokes an action
        /// </summary>
        /// <param name="actionForChild">the action taken with this child transform</param>
        /// <param name="childrenTraversalRecursion">if true traverse children called on every child, if false not</param>
        /// <param name="checkIfEnabledForRecursion">if true only invoked if child object is enabled, if false invoked no matter the object active state</param>
        internal void TraverseChildren( Action<Transform> actionForChild, bool childrenTraversalRecursion= true, bool checkIfEnabledForRecursion = true ) 
        {

            if (this != Scene.Current.Root && !GameObject.IsEnabled)
            {
                return;
            }

            if (actionForChild == null || children == null)
                return;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                actionForChild( child );
                if (childrenTraversalRecursion)
                {
                    if (checkIfEnabledForRecursion)
                    {
                        if (child.GameObject.IsEnabled)
                        {
                            child.TraverseChildren( actionForChild, childrenTraversalRecursion, checkIfEnabledForRecursion );
                        }
                    } else
                    {
                        child.TraverseChildren( actionForChild, childrenTraversalRecursion, checkIfEnabledForRecursion );
                    }
                }
            }

            ////TODO: move this maybe, although it does work here, just doesnt fit in this method
            this.Position += this.Velocity;
            //this.Velocity -= this.Velocity * 0.99f;
            this.Velocity *= 0.75f;
        }

        /// <summary>
        /// traverse the transform hierarchy from a certain start point, along the parents. invoking an action for every node
        /// on the path
        /// </summary>
        /// <param name="start">the start point</param>
        /// <param name="actionForNodeOnPath">action to take for a node on the path (including start)</param>
        public static void TraverseHierarchyToRoot( Transform start,Action<Transform> actionForNodeOnPath ) 
        {
            if (actionForNodeOnPath == null)
                return;
            var t = start;
            while (t.HasParent)
            {
                actionForNodeOnPath( t );
                t = t.Parent;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder("Transform of object ");
            b.AppendLine( GameObject.Name )
            .AppendLine( "Parent: " ).AppendLine( HasParent ? Parent.GameObject.Name : "No Parent" )
            .AppendLine( "World Space" )
            .Append( "\tPosition: " ).AppendLine( Position.ToString() )
            .Append( "\tRotation: " ).AppendLine( Rotation.ToString() )
            .Append( "\tScale:" ).AppendLine( Scale.ToString() )
            .AppendLine( "Local Space:" )
            .Append( "\tPosition:" ).AppendLine( LocalPosition.ToString() )
            .Append( "\tRotation: " ).AppendLine( LocalRotation.ToString() )
            .Append( "Child Count: " ).AppendLine( ChildCount.ToString() );
            return b.ToString();
        }

        public override bool Equals( object obj )
        {
            if (obj is Transform t)
            {
                return t.GameObject.Equals( this.GameObject );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GameObject.GetHashCode();
        }

        /// <summary>
        /// explicit conversion to transform
        /// todo maybe change to implicit if not confusing
        /// </summary>
        /// <param name="t">the transform to convert</param>
        public static explicit operator GameObject( Transform t ) 
        {
            return t.GameObject;
        }

        /// <summary>
        /// creates a transform that is used as the scene root
        /// </summary>
        /// <returns>a transform without a gameobject used as scene root</returns>
        internal static Transform CreateSceneRoot() 
        {
            return new Transform();
        }
        
    }
}
