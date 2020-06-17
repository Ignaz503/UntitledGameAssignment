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
using System.Net.NetworkInformation;

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
            this.VirtualViewport = new AspectRatioMaintainingVirtualViewport(Window,GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height,GraphicsDevice);


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
            TimeInfo.TimeScale = 0f;
            //TODO move to a scene 
            //Cursor c = new Cursor();
            var c = AssetManager.Load<Texture2D>("Sprites/cursor");
            Mouse.SetCursor( MouseCursor.FromTexture2D( c, c.Width/2, c.Height/2 ) );
            //Mouse.SetCursor( MouseCursor.Crosshair );

            //basic
            CreateAdaptiveUpdateRateChanger();

           var player = LoadPlayers();

            //LoadTestGrid();

            CreatePatrol(player, new Vector2[]
            {
                new Vector2(0.2f,0.1f),
                new Vector2(0.3f,0.1f),
                new Vector2(0.5f,0.25f),
                new Vector2(0.75f,0.1f),
                new Vector2(0.8f,0.8f),
            },.5f, Color.White);

            CreatePatrol( player, new Vector2[]
            {
                new Vector2(-0.01f, 0.2f ),
                new Vector2(0.05f,0.8f),
                new Vector2(0.5f,1f),
                new Vector2(0.8f,0.8f),
                new Vector2(0.9f,0.9f),
                new Vector2(1.0f,1.2f)
            }, .2f, Color.Red ) ;

            CreateBoxes(new Vector2[] 
            {
                new Vector2(0.05f,0.05f),
                new Vector2(0.13f,0.05f),
                new Vector2(0.05f,0.165f),
                new Vector2(.13f,0.165f),
                new Vector2(0.05f,0.275f),
                new Vector2(0.13f,0.275f),
                new Vector2(0.05f,0.38f),
                new Vector2(.13f,0.38f),
                new Vector2(0.5f,0.1f)
            } );


            CreateStationaryShootingGuards( new Vector2[]
            {
                new Vector2(.5f,.3f)
            }, player, 1f );

            CreateGoal( new Vector2( 0.2f, 0.1f ),player );

            SetupCamera(player);


            CreateStart();
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

        /// <summary>
        /// The return value of this function is the slope of the vector field function V(X,t)
        /// </summary>
        Func<Vector2, double, Vector2> Eval = delegate (Vector2 vec, double t)
        {
            return new Vector2((float)Math.Cos(vec.X + 2 * vec.Y), (float)Math.Sin(vec.X - 2 * vec.Y)) * (float)t;
            //return new Vector2(-(1.0f / vec.Y), 1.0f/(vec.X / 2.0f)) * (float)t;
        };

        void OnPlayerDeath(Health h, GameObject player ) 
        {
            h.OnDeath -= OnPlayerDeath;
            player.Destroy();

            var winText = new GameObject();
            winText.Transform.Position = NDCToWorld( Vector2.One * .5f );

            winText.AddComponent( j => new TextRenderer( "Congratulations!\nYou Won A Free Death Cupon.\nRedeemed Immeadiately!\nYou died...", Color.Black, SortingLayer.UI, j ) );
            winText.Transform.Scale = Vector2.One * 0.7f;
        }

        private GameObject LoadPlayers()
        {
            Vector2 center = NDCToWorld(new Vector2(.5f,.5f));
            TempPlayer player = new TempPlayer(center, (obj) => new MovementController(obj, walkSpeed: 5.0f), SortingLayer.Entities, TempPlayer.tint.white, "Sprites/playershoulders");
            
            TankTreads treads = new TankTreads(Camera.Active.ScreenToWorld(center), SortingLayer.Entities, player, TankTreads.tint.white, "Sprites/playerlegr");
            List<String> sprites = new List<string>();
            sprites.Add("Sprites/playerlegr");
            sprites.Add("Sprites/playerleg");
            sprites.Add("Sprites/playerlegl");
            sprites.Add("Sprites/playerleg");
            treads.AddComponent( (obj) => new SpriteFlicker(obj, treads.SpriteRen, sprites, true, 0.2f));

            player.AddComponent( ( obj ) => new MouseLocationBasedRotationController( obj ) );
            player.AddComponent( ( obj ) => new ShootScript( obj, 15.0f ) );
            player.AddComponent( ( obj ) => new RigidBody2D( obj, 50.0f, SortingLayer.Entities ) );
            player.AddComponent( ( obj ) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities + 1 ) );

            player.AddComponent( ( obj ) => new VectorField( obj, Eval, 0.005f, 0.1f, true ) );;
            player.AddComponent( ( obj ) => new SpawnParticles( obj, 200, 0.75f, "Sprites/firefly", 1.0f ) );

            var pH = player.AddComponent(obj => new Health(3,obj));
            pH.OnDeath += OnPlayerDeath;

            //Tank "prefab"
            var p2 = new TempPlayer( NDCToWorld(new Vector2(.7f,.75f)), null, SortingLayer.EntitesSubLayer(1), TempPlayer.tint.white, "Sprites/tank_gun");
            var p2_treads = new TankTreads(p2.Transform.Position, SortingLayer.EntitesSubLayer(1), p2, TankTreads.tint.white, "Sprites/tank_treads");
            p2.AddComponent((obj) => new RigidBody2D(obj, 1.5f, SortingLayer.Entities));
            p2.AddComponent((obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities));
            var h2 = p2.AddComponent(j => new Health(5, j));
            h2.OnDeath += OnDeath;

            var p3 = new TempPlayer(NDCToWorld(new Vector2(.35f,.7f)), null, SortingLayer.EntitesSubLayer(1), TempPlayer.tint.white, "Sprites/tank_gun");
            var p3_treads = new TankTreads(p3.Transform.Position, SortingLayer.EntitesSubLayer(1), p3, TankTreads.tint.white, "Sprites/tank_treads");
            p3.AddComponent((obj) => new RigidBody2D(obj, 1.5f, SortingLayer.Entities));
            p3.AddComponent((obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities));
            var h3 = p3.AddComponent(j => new Health(5, j));
            h3.OnDeath += OnDeath;

            var p4 = new TempPlayer(NDCToWorld(new Vector2(.35f,.8f)), null, SortingLayer.EntitesSubLayer(1), TempPlayer.tint.white, "Sprites/tank_gun");
            var p4_treads = new TankTreads(p4.Transform.Position, SortingLayer.EntitesSubLayer(1), p4, TankTreads.tint.white, "Sprites/tank_treads");
            p4.AddComponent((obj) => new RigidBody2D(obj, 1.5f, SortingLayer.Entities));
            p4.AddComponent((obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities));
            var h4 = p4.AddComponent(j => new Health(5, j));
            h4.OnDeath += OnDeath;

            var p5 = new TempPlayer(NDCToWorld(new Vector2(.3f,.7f)), null, SortingLayer.EntitesSubLayer(1), TempPlayer.tint.red, "Sprites/tank_gun");
            var p5_treads = new TankTreads(p5.Transform.Position, SortingLayer.EntitesSubLayer(1), p5, TankTreads.tint.red, "Sprites/tank_treads");
            p5.AddComponent((obj) => new RigidBody2D(obj, 1.5f, SortingLayer.Entities));
            p5.AddComponent((obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities));
            var h5 = p5.AddComponent(j => new Health(10, j));
            h5.OnDeath += OnDeath;

            var p6 = new TempPlayer(NDCToWorld(new Vector2(.3f,.8f)), null, SortingLayer.EntitesSubLayer(1), TempPlayer.tint.white, "Sprites/tank_gun");
            var p6_treads = new TankTreads(p6.Transform.Position, SortingLayer.EntitesSubLayer(1), p6, TankTreads.tint.white, "Sprites/tank_treads");
            p6.AddComponent((obj) => new RigidBody2D(obj, 1.5f, SortingLayer.Entities));
            p6.AddComponent((obj) => new BoxCollider(player.SpriteRen, obj, SortingLayer.Entities));
            var h6 = p6.AddComponent(j => new Health(5, j));
            h6.OnDeath += OnDeath;

            var spike = new Spikeball(NDCToWorld(new Vector2(.15f,.7f)));
            spike.AddComponent( ( obj ) => new GravPull( obj, player, effectiveRadius: 300.0f, rotate: true ) );

            var w1 = new Wall(NDCToWorld(new Vector2(.6f,.5f)), (float)Math.PI);
            w1.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w2 = new Wall(NDCToWorld(new Vector2(.6f,.625f)), (float)Math.PI * 2);
            w2.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w3 = new Wall(NDCToWorld(new Vector2(.6f,.375f)), (float)Math.PI);
            w3.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w4 = new Wall(NDCToWorld(new Vector2(.4f,.375f)), (float)Math.PI * 2);
            w4.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w5 = new Wall(NDCToWorld(new Vector2(.365f,.3f)), (float)Math.PI * 2);
            w5.Transform.Rotation = (float)Math.PI * 0.5f;
            w5.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w6 = new Wall(NDCToWorld(new Vector2(.365f,.45f)), (float)Math.PI * 2);
            w6.Transform.Rotation = (float)Math.PI * 0.5f;
            w6.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w7 = new Wall(NDCToWorld(new Vector2(.6f,.75f)), (float)Math.PI * 2);
            w7.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w8 = new Wall(NDCToWorld(new Vector2(.565f,.825f)), (float)Math.PI * 2);
            w8.Transform.Rotation = (float)Math.PI * 0.5f;
            w8.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            var w9 = new Wall(NDCToWorld(new Vector2(.32f,.51f)), (float)Math.PI * 2);
            w9.AddComponent((obj) => new RigidBody2D(obj, 50.0f, SortingLayer.Entities, false));

            /*var red_heart = new PickupHeart(Camera.Active.ScreenToWorld(new Vector2(130, 100)), heal: player, Color.Red);
            red_heart.AddComponent( ( obj ) => new GravPull( obj, player, effectiveRadius: 200.0f, rotate: false ) );

            var blue_heart = new PickupHeart(Camera.Active.ScreenToWorld(new Vector2(500, 80)), heal: player, Color.Blue);
            blue_heart.AddComponent( ( obj ) => new GravPull( obj, player, effectiveRadius: 200.0f, rotate: false ) );
            
            var green_heart = new PickupHeart(Camera.Active.ScreenToWorld(new Vector2(200, 300)), heal: player, Color.Green);
            green_heart.AddComponent( ( obj ) => new GravPull( obj, player, effectiveRadius: 200.0f, rotate: false ) );*/

            return player;
        }

        private void AddGoalChildren( GameObject @object,float speed,float dir, int recursion)
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
            AddGoalChildren( shield, speed*2f, -dir, recursion);
        }
          

        void CreateBoxes(Vector2[] ndclocations) 
        {
            var boxDissipate = new DissipateInfo(true);

            for (int i = 0; i < ndclocations.Length; i++)
            {
                var p = NDCToWorld(ndclocations[i]);
                var destructableBox = new DestructableBox(AssetManager.Load<Texture2D>("Sprites/small_woodencrate"),p,boxDissipate);
                destructableBox.Transform.Scale = Vector2.One * 1.5f;
            }
        }

        void ShowWinScreen(Goal g, GameObject player) 
        {
            g.OnPlayerReachedGoal -= ShowWinScreen;
            player.Destroy();

            var winText = new GameObject();
            winText.Transform.Position = NDCToWorld( Vector2.One * .5f );

            winText.AddComponent( j => new TextRenderer( "You Won!", Color.Black, SortingLayer.UI, j ) );
        }

        void CreateGoal(Vector2 ndcLocation, GameObject player)
        {
            var obj = new GameObject();
            obj.Name = "Goal";
            obj.Transform.Position = NDCToWorld(ndcLocation);
            var ren = obj.AddComponent(j => new SpriteRenderer("Sprites/shield", Color.White, SortingLayer.EntitesSubLayer(1), j));


            var col = obj.AddComponent((j) => new BoxCollider(ren, j, SortingLayer.Entities));

            var goal = obj.AddComponent(j => new Goal(player, col, j));

            goal.OnPlayerReachedGoal += ShowWinScreen;

            AddGoalChildren(obj, 1f, 1f, 4);
        }

        void CreateStart()
        {
            var obj = new GameObject();
            obj.Name = "Start";

            var start = obj.AddComponent(j => new StartController( obj ));

            start.Load();

            obj.Transform.Position = NDCToWorld(Vector2.One * .45f);

            obj.AddComponent(j => new TextRenderer("Welcome to Tank McShooty\n\nPress -ENTER- to start", Color.Black, SortingLayer.UI, j));
            
        }


        Vector2 NDCToWorld( Vector2 ndc ) 
        {
            var screenWidth = VirtualViewport.Width;
            var screenHeight = VirtualViewport.Height;

            return Camera.Active.ScreenToWorld(new Vector2(ndc.X*screenWidth,ndc.Y*screenHeight));
        }

        /// <summary>
        /// yeah this shouldn't be here but waht evs
        /// </summary>
        /// <param name="obj">the GO that died</param>
        void OnDeath(Health h, GameObject obj ) 
        {
            h.OnDeath -= OnDeath;
            obj.Destroy();
        }

        void CreateStationaryShootingGuards(Vector2[] positionsNDC, GameObject player, float shootFrequency) 
        {
            for (int i = 0; i < positionsNDC.Length; i++)
            {
                var p = NDCToWorld(positionsNDC[i]);

                var guard = new GameObject();
                guard.Name = $"Shooting Guard {i}";
                guard.Transform.Position = p;
                guard.Transform.Rotation = Mathf.PI;

                var gun = new GameObject();
                gun.AddComponent( j => new SpriteRenderer( "Sprites/tank_gun", Color.White, SortingLayer.EntitesSubLayer( 2 ), j ) );
                gun.Transform.Parent = guard.Transform;
                gun.Transform.LocalPosition = Vector2.Zero;
                gun.Transform.LocalRotation = 0f;


                var sRenderer = guard.AddComponent( ( j ) => new SpriteRenderer( "Sprites/tank_treads", Color.White, SortingLayer.EntitesSubLayer( 1 ), j ) );

                guard.AddComponent( ( obj ) => new BoxCollider( sRenderer, obj, SortingLayer.Entities ) );
                guard.AddComponent( ( obj ) => new RigidBody2D( obj, 1.5f, SortingLayer.Entities ) );
                var h = guard.AddComponent( j => new Health( 5, j ) );

                h.OnDeath += OnDeath;

                guard.AddComponent( j => new TankShootStationary( player.Transform, gun.Transform, j, 10f, shootFrequency,dist: 250) );

                //var gun sprite

            }
        }


        void CreatePatrol(GameObject player, Vector2[] pointsNDC, float shootFrequency, Color tint ) 
        {
            var pF = new GameObject();
            pF.Name = nameof( PathFollower );

            //pF.Transform.Position = Camera.Active.ScreenToWorld( new Vector2( 500, 80 ) );

            var sRenderer = pF.AddComponent( ( j ) => new SpriteRenderer( "Sprites/tank_treads", tint, SortingLayer.EntitesSubLayer( 1 ), j ) );

            pF.AddComponent(( obj ) => new BoxCollider( sRenderer, obj, SortingLayer.Entities ) );
            var h = pF.AddComponent( j => new Health( 1, j ) );

            h.OnDeath += OnDeath;

            pF.AddComponent( j => new TankShoot(player.Transform, j,15f, shootFrequency ) );

            //var gun sprite
            var gun = new GameObject();
            gun.AddComponent( j => new SpriteRenderer( "Sprites/tank_gun", tint, SortingLayer.EntitesSubLayer( 2 ), j ) );
            gun.Transform.Parent = pF.Transform;
            gun.Transform.LocalPosition = Vector2.Zero;

            var pathFollower = pF.AddComponent( ( j ) => new PathFollower(0.005f, j, 40f ) );



            var path = new Vector2[pointsNDC.Length];
            for (int i = 0; i < pointsNDC.Length; i++)
            {
                var p = pointsNDC[i];
                path[i] = NDCToWorld( p );
            }

            pathFollower.PathToFollow = new Path( path );
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
