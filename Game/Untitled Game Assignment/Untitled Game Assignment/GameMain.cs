using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Util.Input;
using Util.FrameTimeInfo;
using Util.Rendering;
using UntitledGameAssignment.Core;
using UntitledGameAssignment.Core.SceneGraph;
using UntitledGameAssignment.Core.Components;
using Util.Rendering.VirtualViewports;
using Util.AssetManagment;
using GeoUtil.Polygons;
using UntitledGameAssignment.Core.GameObjects;
using Util.SortingLayers;
using Loyc.Geometry;
using Util.CustomMath;

namespace UntitledGameAssignment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public sealed class GameMain : Game
    {
        /// <summary>
        /// instance of the game main
        /// </summary>
        public static GameMain Instance { get; private set; }

        /// <summary>
        /// the graphics device manager
        /// </summary>
        GraphicsDeviceManager graphics;
        /// <summary>
        /// the graphics device manager accessor
        /// </summary>
        public GraphicsDeviceManager GfxManager => graphics;

        public VirtualViewport VirtualViewport { get; private set;}

        /// <summary>
        /// the set of object we marked to destroy this update loop
        /// </summary>
        List<EngineObject> objectsToDestroy;

        //visitors(more mem efficent not to create them every loop)
        UpdateVisitor updateVisitor;
        LateUpdateVisitor lateUpdateVisitor;
        FixedUpdateVsitor fixedUpdateVsitor;
        DrawVisitor drawVisitor;

        public GameMain()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";
            objectsToDestroy = new List<EngineObject>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            CreateBatchRenderer();
            TimeInfo.Initialize( 1f, 0.02f );
            Input.Initialize( graphics );
            AssetManager.Initialize( Content );

            //this.VirtualViewport = new UnchangingVirtualViewport(GraphicsDevice);
            this.VirtualViewport = new AspectRatioMaintainingVirtualViewport(Window,640,480,GraphicsDevice);


            // TODO: Add your initialization logic here
            updateVisitor = new UpdateVisitor();
            lateUpdateVisitor = new LateUpdateVisitor();
            fixedUpdateVsitor = new FixedUpdateVsitor();
            drawVisitor = new DrawVisitor();

            Scene.LoadScene( new Scene() );
            
            base.Initialize();
        }

        private void CreateBatchRenderer()
        {
            //BatchRenderer.Create(  );
            SortedBatchRenderer.Create( );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //TODO move to a scene 
            //Cursor c = new Cursor();
            var c = AssetManager.Load<Texture2D>("Sprites/cursor");
            Mouse.SetCursor( MouseCursor.FromTexture2D( c, c.Width/2, c.Height/2 ) );
            //Mouse.SetCursor( MouseCursor.Crosshair );

            //basic
            CreateAdaptiveUpdateRateChanger();

            // TODO: use this.Content to load your game content here

            var player = LoadPlayers();

            //LoadTestGrid();

            LoadTestPathFollow();

            SetupCamera(player);

        }

        private void SetupCamera(GameObject player)
        {
            Camera.Active.GameObject.AddComponent( j => new TransformTracker( player.Transform, ()=> -Camera.Active.Origin ,j) );        
        }

        private void CreateAdaptiveUpdateRateChanger()
        {
            GameObject obj = new GameObject();
            obj.AddComponent( j => new UpdateRateAdapter( Keys.D9, Keys.D0, j ) );
        }

        private GameObject LoadPlayers()
        {
            TempPlayer player = new TempPlayer(Camera.Active.ScreenToWorld(VirtualViewport.Bounds.Center.ToVector2()), (obj) => new MovementController(obj, walkSpeed: 10f), SortingLayer.Entities, TempPlayer.tint.white, "Sprites/playershoulders");
            
            TankTreads treads = new TankTreads(Camera.Active.ScreenToWorld(VirtualViewport.Bounds.Center.ToVector2()), SortingLayer.Entities, player, TankTreads.tint.white, "Sprites/playerlegr");
            List<String> sprites = new List<string>();
            sprites.Add("Sprites/playerlegr");
            sprites.Add("Sprites/playerlegl");
            treads.AddComponent( (obj) => new SpriteFlicker(obj, treads.SpriteRen, sprites, true, 0.5f));

            player.AddComponent( ( obj ) => new MouseLocationBasedRotationController( obj ) );
            player.AddComponent( ( obj ) => new ShootScript( obj, 4.0f ) );
            player.AddComponent( (obj) => new RigidBody2D(obj, 10.0f) );

            List<SortingLayer> neglectSelf = new List<SortingLayer>();
            neglectSelf.Add(SortingLayer.Entities + 1);
            player.AddComponent( (obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities + 1, true, neglectSelf) );

            var p2 = new TempPlayer(
                Camera.Active.ScreenToWorld(VirtualViewport.Bounds.Center.ToVector2()+Vector2.One*50f),
                null,
                SortingLayer.EntitesSubLayer(1),
                TempPlayer.tint.white,
                "Sprites/playershoulders",
                Keys.Y,
                Keys.X);
            p2.AddComponent( (obj) => new RigidBody2D(obj, 1.5f));
            p2.AddComponent( (obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities) );

            AddShieldToObject( p2, 1f, 1f, 4);

            //////temp, will move this
            //var spike = new Spikeball(Camera.Active.ScreenToWorld(new Vector2(150, 150)));
            //spike.AddComponent( ( obj ) => new GravPull( obj, tankplayer, mass: 0.5f, effectiveRadius: 300.0f, rotate: true ) );

            var red_heart = new PickupHeart(Camera.Active.ScreenToWorld(new Vector2(130, 100)), heal: player, Color.Red);
            red_heart.AddComponent( ( obj ) => new GravPull( obj, player, mass: 0.25f, effectiveRadius: 200.0f, rotate: false ) );
            
            var blue_heart = new PickupHeart(Camera.Active.ScreenToWorld(new Vector2(500, 80)), heal: player, Color.Blue);
            blue_heart.AddComponent( ( obj ) => new GravPull( obj, player, mass: 0.25f, effectiveRadius: 200.0f, rotate: false ) );
            
            var green_heart = new PickupHeart(Camera.Active.ScreenToWorld(new Vector2(200, 300)), heal: player, Color.Green);
            green_heart.AddComponent( ( obj ) => new GravPull( obj, player, mass: 0.25f, effectiveRadius: 200.0f, rotate: false ) );

            var destructableBox = new DestructableBox(AssetManager.Load<Texture2D>("Sprites/small_woodencrate"),Camera.Active.ScreenToWorld(new Vector2(500, 300)),player);
            var destructableBox1 = new DestructableBox(AssetManager.Load<Texture2D>("Sprites/small_woodencrate"),Camera.Active.ScreenToWorld(new Vector2(130, 100)),player);
            var destructableBox2 = new DestructableBox(AssetManager.Load<Texture2D>("Sprites/small_woodencrate"),Camera.Active.ScreenToWorld(new Vector2(500, 80)),player);
            var destructableBox3 = new DestructableBox(AssetManager.Load<Texture2D>("Sprites/small_woodencrate"),Camera.Active.ScreenToWorld(new Vector2(200, 300)),player);

            return player;
        }

        private void AddShieldToObject( GameObject @object,float speed,float dir, int recursion)
        {

            if (recursion == 0)
                return;

            recursion--;
            var motor = new GameObject();
            motor.Transform.Parent = @object.Transform;
            motor.Transform.LocalPosition = Vector2.Zero;

            motor.AddComponent( j => new Rotator( j, speed: speed, direction:dir) );

            var shield = new GameObject();

            shield.Transform.Parent = motor.Transform;
            shield.Transform.LocalPosition = new Vector2( 0f, 40f );
            shield.Transform.Scale = new Vector2( 0.75f, 0.75f );
            var ren =shield.AddComponent(j=> new SpriteRenderer( "Sprites/shield", Color.White, SortingLayer.EntitesSubLayer( 1 ), j ) );
            AddShieldToObject( shield, speed*2f, -dir, recursion);
        }

        private void LoadTestGrid()
        {
            GameObject obj = new GameObject();
            var t = obj.AddComponent( ( j ) => new TestTileMap( Camera.Active.Origin - new Vector2(40,40)*.5f * 50f,40, j ) );
            var at = AStarTileMap.CreateAStarMap( t );
        }

        void LoadTestPathFollow()
        {
            var obj = new GameObject();
            obj.Transform.Position = Camera.Active.ScreenToWorld( Vector2.Zero );

            var pF = new GameObject();

            pF.Transform.Position = Camera.Active.ScreenToWorld( new Vector2( 500, 80 ) );

            pF.AddComponent( ( j ) => new SpriteRenderer( "Sprites/heart", Color.White, SortingLayer.EntitesSubLayer( 1 ), j ) );

            var pathFollower = pF.AddComponent( ( j ) => new PathFollower(0.005f, j, 0.5f ) );

            //AddObjectsToEitherSide(obj, recursion: 2 );

            obj.AddComponent( ( j ) => new PathCreator(pathFollower,AssetManager.Load<Texture2D>( "Sprites/WhiteSquare" ), .5f, j ) );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Scene.UnloadScene( Scene.Current );
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            //default generated
            if (GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ))
            {
                Exit();
            }
            TimeInfo.TimeStep = gameTime;
            Input.Update();

            // TODO: Add your update logic here
            HandleFixedUpdateLoop();
            HandleUpdateLoop();
            HandleLateUpdateLoop();

            base.Update( gameTime );

            HandleObjectDestruction();
        }

        /// <summary>
        /// handle late update loop
        /// </summary>
        private void HandleLateUpdateLoop()
        {
            lateUpdateVisitor.Start();
        }

        /// <summary>
        /// handles the update loop
        /// </summary>
        private void HandleUpdateLoop()
        {
            updateVisitor.Start();
        }

        /// <summary>
        /// handles the fixed update loop
        /// </summary>
        private void HandleFixedUpdateLoop()
        {
            for (int i = 0; i < TimeInfo.NumberFixedUpdates; i++)
            {
                fixedUpdateVsitor.Start();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( Color.CornflowerBlue );

            // TODO: Add your drawing code here

            HandleDrawLoop();

            base.Draw( gameTime );
        }

        /// <summary>
        /// handles the draw loop
        /// </summary>
        private void HandleDrawLoop()
        {
            drawVisitor.Start();
        }

        /// <summary>
        /// marks object for destruction
        /// </summary>
        /// <param name="engineObject">the object we want to destroy</param>
        internal void MarkForDestruction( EngineObject obj )
        {
            if(!objectsToDestroy.Contains(obj))
                objectsToDestroy.Add( obj );
        }

        private void HandleObjectDestruction()
        {
            for (int i = 0; i < objectsToDestroy.Count; i++)
            {
                var obj = objectsToDestroy[i];
                obj.OnDestroy();
                obj.Dispose();
            }
            objectsToDestroy.Clear();
        }
    }
}
