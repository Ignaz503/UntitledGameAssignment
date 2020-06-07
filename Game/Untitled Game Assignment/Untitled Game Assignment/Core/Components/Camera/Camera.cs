using Microsoft.Xna.Framework;
using System;
using UntitledGameAssignment.Core.GameObjects;
using Util.CustomMath;
using Util.Rendering.VirtualViewports;
using Microsoft.Xna.Framework.Input;
using Util.Input;
using Util.FrameTimeInfo;
using Util.CustomDebug;

namespace UntitledGameAssignment.Core.Components
{
    public class Camera : Component
    {
        static Camera active;
        public static Camera Active
        { 
            get 
            {
                if (active == null)
                {
                    CreateNewDefaultCamera();
                }
                return active;
            }
            set 
            {
                if (value != null)
                    active = value;
            }
        }

        internal static void CreateDefaultCamera()
        {
            CreateNewDefaultCamera();
        }
        
        private static void CreateNewDefaultCamera()
        {
            var obj = new GameObject();
            obj.AddComponent( ( j ) => new Camera( GameMain.Instance.VirtualViewport, j ) );
            obj.AddComponent( j => new CameraZoom( j ) );
            //obj.AddComponent( j => new MovementController( obj, up:Keys.I, down: Keys.K, left: Keys.J, right: Keys.L, walkSpeed: 100f ) );
            //obj.AddComponent( go => new KeyBasedRotationController( go,increase:Keys.U,decrease:Keys.O ,changeInDegrees: 45f ) );
        }


        /// <summary>
        /// virutal viewport camera is using
        /// </summary>
        VirtualViewport virtuaVP;

        /// <summary>
        /// camera zoom
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// camera bounds
        /// </summary>
        public Rect Bounds 
        {
            get
            {
                var corners = Frustum.GetCorners();
                Vector2 tL = corners[0].ToVec2();
                Vector2 bR = corners[2].ToVec2();
                Vector2 size = new Vector2(
                    bR.X-tL.X,bR.Y-tL.Y);
                return new Rect( tL, size );
            }
        }

        /// <summary>
        /// camera origin
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// camera center
        /// </summary>
        public Vector2 Center => Transform.Position + Origin;

        /// <summary>
        /// camera frustrum
        /// </summary>
        public BoundingFrustum Frustum 
        { 
            get
            {
                return new BoundingFrustum( GetProjectionMatrix( GetVirtualViewMatrix() ) );
            }
        }


        public Camera(VirtualViewport viewport, GameObject obj ) : base( obj )
        {
            this.virtuaVP = viewport; 
            Zoom = 1f;
            Origin = new Vector2( viewport.Width / (float)2, viewport.Height / (float)2 );
            Active = this;
        }

        public override void OnDestroy()
        {}

        public Vector2 WorldToScreen( Vector2 worldPos)
        {
            return Vector2.Transform( worldPos + new Vector2( virtuaVP.Viewport.X, virtuaVP.Viewport.Y ), GetViewMatrix() );
        }

        public Vector2 ScreenToWorld( Vector2 screenPos)
        {
            return Vector2.Transform(screenPos - new Vector2( virtuaVP.Viewport.X,virtuaVP.Viewport.Y),GetInverseViewMatrix());
        }

        public void LookAt( Vector2 p ) 
        {
            Transform.Position -= new Vector2( virtuaVP.Width / 2f, virtuaVP.Height/ 2f );
        }

        private Matrix GetInverseViewMatrix()
        {
            return Matrix.Invert( GetViewMatrix() );
        }

        public Matrix GetViewMatrix() 
        {
            return GetVirtualViewMatrix() * virtuaVP.Scale;
        }

        public Matrix GetVirtualViewMatrix()
        {
            return
                Matrix.CreateTranslation( new Vector3( -Transform.Position, 0f ) ) *
                Matrix.CreateTranslation( new Vector3( -Origin, 0f ) ) *
                Matrix.CreateRotationZ( Transform.Rotation ) *
                Matrix.CreateScale( Zoom, Zoom, 1f ) *
                Matrix.CreateTranslation( new Vector3( Origin, 0f ) );
        }

        public Matrix GetProjectionMatrix( Matrix viewMatrix ) 
        {

            return viewMatrix * Matrix.CreateOrthographicOffCenter( 0, virtuaVP.Width, virtuaVP.Height, 0, -1, 0 );
        }
    }

    public class CameraZoom : Component, IUpdate
    {
        Keys zoomInKey;
        Keys zoomOutKey;

        float zoomSpeed = 1f;

        float currentZoom => Camera.Active.Zoom;

        public CameraZoom( GameObject obj, Keys zoomIn = Keys.O, Keys zoomOut = Keys.P ) : base( obj )
        {
            this.zoomInKey = zoomIn;
            this.zoomOutKey = zoomOut; 
        }

        public override void OnDestroy()
        {}

        public void Update()
        {
            float change = 0f;
            if (Input.IsKeyPressed( zoomInKey ))
                change -= zoomSpeed * TimeInfo.DeltaTime;
            if (Input.IsKeyPressed( zoomOutKey ))
                change += zoomSpeed * TimeInfo.DeltaTime;
            if (currentZoom + change < 0)
                Camera.Active.Zoom = 0.1f;
            else
                Camera.Active.Zoom += change;
        }
    }
}
