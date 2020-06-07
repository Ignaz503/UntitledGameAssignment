
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using UntitledGameAssignment;
using UntitledGameAssignment.Core.Components;
using Util.AssetManagment;
using Util.SortingLayers;

namespace Util.Rendering
{
    /// <summary>
    /// Batch renderer for sprites
    /// </summary>
    public sealed class UnsortedBatchRenderer : IDisposable
    {
        static UnsortedBatchRenderer instance;

        /// <summary>
        /// the sprite batch
        /// </summary>
        SpriteBatch batch;
        /// <summary>
        /// the sprite batch settings
        /// </summary>
        Settings currentSettings;

        /// <summary>
        /// gets the current settings
        /// </summary>
        public static Settings CurrentSettings => instance.currentSettings;

        /// <summary>
        /// is the batcher started or not
        /// </summary>
        bool isStarted;

        /// <summary>
        /// accessor if instance is started
        /// needs to be true for end not to throw an exception when called
        /// </summary>
        public static bool IsStarted => instance.isStarted;

        #region ctors and initialization

        private UnsortedBatchRenderer( SpriteBatch batch,Settings settings) 
        {
            Init(batch,settings);
        }

        private UnsortedBatchRenderer( GraphicsDevice device,Settings settings ) : this(new SpriteBatch(device),settings )
        {}

        /// <summary>
        /// initializes batch renderer
        /// </summary>
        /// <param name="b">the sprite batch</param>
        /// <param name="settings">the settings of the batch renderer</param>
        void Init( SpriteBatch b, Settings settings) 
        {
            batch = b;
            //todo whatever is neceserary as things come up
            currentSettings = settings;
        }
        #endregion

        /// <summary>
        /// changes current settings
        /// </summary>
        /// <param name="newSettings">the desired settings</param>
        public static void ChangeSettings( Settings newSettings )
        {
            instance.currentSettings = newSettings;
            
        }

        /// <summary>
        /// changes view transform matrix for batch renderer
        /// </summary>
        /// <param name="newViewTransformMat"></param>
        public static void SetViewTransformMatrix( Matrix newViewTransformMat ) 
        {
            instance.currentSettings.ViewTransformMatrix = newViewTransformMat;
        }

        /// <summary>
        /// disposes of batch
        /// </summary>
        public void Dispose()
        {
            batch.Dispose();
        }

        #region drawing interal
        /// <summary>
        /// begins sprite batch drawing
        /// </summary>
        void InternalBegin() 
        {
            if (isStarted)
                return;
            batch.Begin( currentSettings.SortMode, currentSettings.BlendState, currentSettings.SampleState, currentSettings.DepthStencilState, currentSettings.RasterizerState, currentSettings.BaseEffect, currentSettings.ViewTransformMatrix );
            isStarted = true;
        }

        void InternalEnd() 
        {
            if (!isStarted)
                throw new BatchRendererNotStartedException();
            batch.End();
            isStarted = false;
        }

        private void InternalDrawPolygon( VertexPositionTexture[] mesh, Texture2D texture, BasicEffect shader, SortingLayer layer, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Camera cam )
        {
            var batchingState = isStarted;
            if (batchingState)
                InternalEnd();

            //now draw
            shader.TextureEnabled = true;
            shader.Texture = texture;

            var p = (position - origin);

            //todo maybe no - layer depends on how other depth things are written
            shader.World = Matrix.CreateRotationZ( rotation ) * Matrix.CreateScale( scale.X, scale.Y, 1f ) * Matrix.CreateTranslation( p.X, p.Y, -(float)layer );

            shader.View = (Matrix)currentSettings.ViewTransformMatrix;

            shader.Projection = Matrix.Identity;

            for (int i = 0; i < shader.CurrentTechnique.Passes.Count; i++)
            {
                shader.CurrentTechnique.Passes[i].Apply();
                batch.GraphicsDevice.DrawUserPrimitives( PrimitiveType.TriangleList, mesh, 0, mesh.Length / 3 );
            }

            if (batchingState)
            {
                InternalBegin();
            }
        }


        void InternalDraw(Texture2D texture,Rectangle destinationRectangle, Color col) 
        {
            batch.Draw( texture, destinationRectangle, col );
        }

        void InternalDraw(Texture2D texture,Vector2 position,Color col) 
        {
            batch.Draw( texture, position, col );
        }

        void InternalDraw(Texture2D texture, Rectangle destinationRectangle,Rectangle? sourceRectangle, Color col) 
        {
            batch.Draw( texture, destinationRectangle, sourceRectangle, col );
        }

        void InternalDraw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col ) 
        {
            batch.Draw( texture, position, sourceRectangle, col );
        }

        void InternalDraw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col,float rotation, Vector2 origin, SpriteEffects effects,float layerDepth=0f) 
        {
            batch.Draw( texture, destinationRectangle, sourceRectangle, col, rotation, origin, effects, layerDepth );
        }



        void InternalDraw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth = 0f ) 
        {
            batch.Draw( texture, position, sourceRectangle, col, rotation, origin, scale, effects, layerDepth );
        }

        void InternalDraw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth = 0f ) 
        {
            batch.Draw( texture, position, sourceRectangle, col, rotation, origin, scale, effects, layerDepth );
        }

        void InternalDrawString(SpriteFont spriteFont,string text, Vector2 position, Color col) 
        {
            batch.DrawString( spriteFont, text, position, col );
        }

        void InternalDrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col ) 
        {
            batch.DrawString( spriteFont, text, position, col );
        }

        void InternalDrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) 
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }  
        
        void InternalDrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) 
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        void InternalDrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) 
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }  
        
        void InternalDrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth) 
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }
        #endregion

        #region static draw wrappers

        /// <summary>
        /// begins sprite instance drawing
        /// </summary>
        public static void Begin()
        {
            instance.InternalBegin( );
        }

        public static void End()
        {
            instance.InternalEnd();
        }

        static void ApplyEffect( Effect effect ) 
        {
            effect.CurrentTechnique.Passes[0].Apply();
        } 

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Color col )
        {
            instance.InternalDraw( texture, destinationRectangle, col );
        }  
        
        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Color col, Effect effect )
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, destinationRectangle, col );
        }

        public static void Draw( Texture2D texture, Vector2 position, Color col )
        {
            instance.InternalDraw( texture, position, col );
        }
        
        public static void Draw( Texture2D texture, Vector2 position, Color col, Effect effect)
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, position, col );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col )
        {
            instance.InternalDraw( texture, destinationRectangle, sourceRectangle, col );
        }
        
        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, Effect effect )
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, destinationRectangle, sourceRectangle, col );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col )
        {
            instance.InternalDraw( texture, position, sourceRectangle, col );
        } 
        
        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, Effect effect )
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, position, sourceRectangle, col );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth = 0f )
        {
            instance.InternalDraw( texture, destinationRectangle, sourceRectangle, col, rotation, origin, effects, layerDepth );
        }
        
        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, Effect effect )
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, destinationRectangle, sourceRectangle, col, rotation, origin, effects, layerDepth );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth = 0f )
        {
            instance.InternalDraw( texture, position, sourceRectangle, col, rotation, origin, scale, effects, layerDepth );
        } 
        
        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, Effect effect )
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, position, sourceRectangle, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth = 0f )
        {
            instance.InternalDraw( texture, position, sourceRectangle, col, rotation, origin, scale, effects, layerDepth );
        } 
        
        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Effect effect)
        {
            ApplyEffect( effect );
            instance.InternalDraw( texture, position, sourceRectangle, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col )
        {
            instance.InternalDrawString( spriteFont, text, position, col );
        }  
        
        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, SpriteEffect effect )
        {
            ApplyEffect( effect );
            instance.InternalDrawString( spriteFont, text, position, col );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col )
        {
            instance.InternalDrawString( spriteFont, text, position, col );
        }
        
        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, SpriteEffect effect )
        {
            ApplyEffect( effect );
            instance.InternalDrawString( spriteFont, text, position, col );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth )
        {
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, SpriteEffect effect )
        {
            ApplyEffect( effect );
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth )
        {
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth, Effect effect )
        {
            ApplyEffect( effect );
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth,SpriteEffect effect )
        {
            ApplyEffect( effect );
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }  
        
        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, SpriteEffect effect )
        {
            ApplyEffect( effect );
            instance.InternalDrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }


        internal static void DrawPolygon( VertexPositionTexture[] mesh, Texture2D texture, BasicEffect shader, SortingLayer layer, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Camera cam )
        {
            instance.InternalDrawPolygon( mesh, texture, shader, layer, position, scale, rotation, origin, cam );
        }

        #endregion

        #region create
        /// <summary>
        /// creates a Batch renderer for a graphics devixe 
        /// </summary>
        /// <param name="settings">the default settings the renderer should use</param>
        /// <param name="createNewIfExisting">creates new batch renderer if instance already existing</param>
        internal static void Create(Settings settings, bool createNewIfExisting = false) 
        {
            if (instance != null && !createNewIfExisting)
                return;
            instance = new UnsortedBatchRenderer( GameMain.Instance.GfxManager.GraphicsDevice, settings );
        }
        /// <summary>
        /// creates a Batch renderer for a graphics devixe 
        /// </summary>
        /// <param name="settings">the default settings the renderer should use</param>
        /// <param name="createNewIfExisting">creates new batch renderer if instance already existing</param>
        internal static void Create(bool createNewIfExisting = false) 
        {
            if (instance != null && !createNewIfExisting)
                return;
            instance = new UnsortedBatchRenderer( GameMain.Instance.GfxManager.GraphicsDevice, Settings.Default );
        }
        #endregion

        #region internal helper

        /// <summary>
        /// a basic effect
        /// </summary>
        public static BasicEffect BasicEffect => new BasicEffect( GameMain.Instance.GraphicsDevice );

        static Effect polygonShader;
        public static Effect PolygonShader 
        {
            get 
            {
                if (polygonShader == null)
                    polygonShader = AssetManager.Load<Effect>( "PolygonShader" );
                return polygonShader;
            }
        }

        /// <summary>
        /// settings class holding all the sprite batch begin settings
        /// </summary>
        public struct Settings 
        {
            public static Settings Default { get {
                    return new Settings( sortMode: SpriteSortMode.Immediate,
                                         blendState: BlendState.AlphaBlend,
                                         sampleState: null,
                                         depthStencilState: DepthStencilState.None,
                                         rasterizerState: null,
                                         baseEffect: null,
                                         viewTransformMatrix: null );
                                         } }
            /// <summary>
            /// the sort mode of the sprite batch
            /// </summary>
            public SpriteSortMode SortMode { get; set; }
            /// <summary>
            /// the blend state of the sprite batch
            /// </summary>
            public BlendState BlendState { get; set; }
            /// <summary>
            /// the sampler state the sprite batch
            /// </summary>
            public SamplerState SampleState { get; set; }
            /// <summary>
            /// the sample state of the sprite batch
            /// </summary>
            public DepthStencilState DepthStencilState { get; set; }
            /// <summary>
            /// the rasterizer state of the sprite batch
            /// </summary>
            public RasterizerState RasterizerState { get; set; }
            /// <summary>
            /// the base effect of the sprite batch
            /// </summary>
            public Effect BaseEffect { get; set; }
            /// <summary>
            /// the view transform matrix of the sprite batch
            /// </summary>
            public Matrix? ViewTransformMatrix { get; set; }

            public Settings( SpriteSortMode sortMode, BlendState blendState, SamplerState sampleState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect baseEffect, Matrix? viewTransformMatrix )
            {
                SortMode = sortMode;
                BlendState = blendState;
                SampleState = sampleState;
                DepthStencilState = depthStencilState;
                RasterizerState = rasterizerState;
                BaseEffect = baseEffect;
                ViewTransformMatrix = viewTransformMatrix;
            }
        }
        #endregion
    }
}
