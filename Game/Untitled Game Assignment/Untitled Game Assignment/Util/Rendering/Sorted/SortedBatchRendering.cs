using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Text;
using UntitledGameAssignment;
using Util.DataStructure;
using Util.SortingLayers;
using UntitledGameAssignment.Core.Components;

namespace Util.Rendering
{
    public partial class SortedBatchRenderer
    {
        static SortedBatchRenderer instance;

        /// <summary>
        /// the sprite batch
        /// </summary>
        SpriteBatch batch;
        /// <summary>
        /// the sprite batch settings
        /// </summary>
        UnsortedBatchRenderer.Settings currentSettings;

        /// <summary>
        /// gets the current settings
        /// </summary>
        public static UnsortedBatchRenderer.Settings CurrentSettings => instance.currentSettings;

        PriorityQueue<IDrawCall> drawCallQueue;

        bool batchingIsStarted;


        #region ctors and initialization

        private SortedBatchRenderer( SpriteBatch batch, UnsortedBatchRenderer.Settings settings )
        {
            Init( batch, settings );
        }

        private SortedBatchRenderer( GraphicsDevice device, UnsortedBatchRenderer.Settings settings ) : this( new SpriteBatch( device ), settings )
        { }

        /// <summary>
        /// initializes batch renderer
        /// </summary>
        /// <param name="b">the sprite batch</param>
        /// <param name="settings">the settings of the batch renderer</param>
        void Init( SpriteBatch b, UnsortedBatchRenderer.Settings settings )
        {
            batch = b;
            //todo whatever is neceserary as things come up
            currentSettings = settings;
            drawCallQueue = new PriorityQueue<IDrawCall>();
        }
        #endregion

        /// <summary>
        /// changes current settings
        /// </summary>
        /// <param name="newSettings">the desired settings</param>
        public static void ChangeSettings( UnsortedBatchRenderer.Settings newSettings )
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

        void Enqueue( IDrawCall call ) 
        {
            drawCallQueue.Enqueue( call );
        }

        #region drawing interal
        /// <summary>
        /// begins sprite batch drawing
        /// </summary>
        void InternalBegin()
        {
            InternalBatchingBegin();
        }

        void InternalEnd()
        {
            while (drawCallQueue.Count > 0)
            {
                var call = drawCallQueue.Dequeue();
                call.MakeCall();
            }
            InternalBatchingEnd();
        }


        void InternalBatchingBegin()
        {
            if (batchingIsStarted)
                return;
            batch.Begin( currentSettings.SortMode, currentSettings.BlendState, currentSettings.SampleState, currentSettings.DepthStencilState, currentSettings.RasterizerState, currentSettings.BaseEffect, currentSettings.ViewTransformMatrix );
            batchingIsStarted = true;
        }

        void InternalBatchingEnd()
        {
            if (!batchingIsStarted)
                throw new BatchRendererNotStartedException();
            batch.End();
            batchingIsStarted = false;
        }


        #region draw texture
        void InternalDraw( Texture2D texture, Rectangle destinationRectangle, Color col )
        {
            batch.Draw( texture, destinationRectangle, col );
        }

        void InternalDraw( Texture2D texture, Vector2 position, Color col )
        {
            batch.Draw( texture, position, col );
        }

        void InternalDraw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col )
        {
            batch.Draw( texture, destinationRectangle, sourceRectangle, col );
        }

        void InternalDraw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col )
        {
            batch.Draw( texture, position, sourceRectangle, col );
        }

        void InternalDraw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth = 0f )
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
        #endregion
        #region draw string
        void InternalDrawString( SpriteFont spriteFont, string text, Vector2 position, Color col )
        {
            batch.DrawString( spriteFont, text, position, col );
        }

        void InternalDrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col )
        {
            batch.DrawString( spriteFont, text, position, col );
        }
        
        void InternalDrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth )
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        void InternalDrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth )
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        void InternalDrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }

        void InternalDrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth )
        {
            batch.DrawString( spriteFont, text, position, col, rotation, origin, scale, effects, layerDepth );
        }
        #endregion
        #region draw polygon

        private void InternalDrawPolygon( VertexPositionTexture[] mesh, Texture2D texture, BasicEffect shader, SortingLayer layer, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Camera cam )
        {
            var batchingState = batchingIsStarted;
            if (batchingState)
                InternalBatchingEnd();

            //now draw
            shader.TextureEnabled = true;
            shader.Texture = texture;

            var p = (position - origin);

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
                InternalBatchingBegin();
            }
        }



        #endregion

        #endregion

        #region create
        /// <summary>
        /// creates a Batch renderer for a graphics devixe 
        /// </summary>
        /// <param name="settings">the default settings the renderer should use</param>
        /// <param name="createNewIfExisting">creates new batch renderer if instance already existing</param>
        internal static void Create( UnsortedBatchRenderer.Settings settings, bool createNewIfExisting = false )
        {
            if (instance != null && !createNewIfExisting)
                return;
            instance = new SortedBatchRenderer( GameMain.Instance.GfxManager.GraphicsDevice, settings );
        }
        /// <summary>
        /// creates a Batch renderer for a graphics devixe 
        /// </summary>
        /// <param name="settings">the default settings the renderer should use</param>
        /// <param name="createNewIfExisting">creates new batch renderer if instance already existing</param>
        internal static void Create( bool createNewIfExisting = false )
        {
            if (instance != null && !createNewIfExisting)
                return;
            instance = new SortedBatchRenderer( GameMain.Instance.GfxManager.GraphicsDevice, UnsortedBatchRenderer.Settings.Default );
        }

        #endregion

        #region static draw wrappers

        /// <summary>
        /// begins sprite instance drawing
        /// </summary>
        public static void Begin()
        {
            instance.InternalBegin();
        }

        public static void End()
        {
            instance.InternalEnd();
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Color col )
        {
            instance.Enqueue( new DrawCall_Tex_Rec_Col( 0, texture, destinationRectangle, col ) );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Color col, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_Rec_Col_Shader( 0, texture, destinationRectangle, col, effect ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Color col )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_Col(0, texture, position, col ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Color col, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_Col_Shader( 0, texture, position, col, effect ) );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col )
        {
            instance.Enqueue( new DrawCall_Tex_dRec_sRec_Col( 0, texture, destinationRectangle, sourceRectangle, col ) );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_dRec_sRec_Col_Shader( 0, texture, destinationRectangle, sourceRectangle, col, effect ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_sRec_Col( 0, texture, position, sourceRectangle, col ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_sRec_Col_Shader( 0, texture, position, sourceRectangle, col,effect ) );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Tex_dRec_sRec_Col_Rot_Origin_Effects_Depth( layer, texture, destinationRectangle, sourceRectangle, col, rotation, origin, effects ) );
        }

        public static void Draw( Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, SpriteEffects effects, SortingLayer layer, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_dRec_sRec_Col_Rot_Origin_Effects_Depth_Shader( layer, texture, destinationRectangle, sourceRectangle, col, rotation, origin, effects,effect ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_sRec_Col_Rot_Origin_Scale_Effects_Depth( layer, texture, position, sourceRectangle, col, rotation, origin, scale, effects ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, SortingLayer layer, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_sRec_Col_Rot_Origin_Scale_Effects_Depth_Shader( layer, texture, position, sourceRectangle, col, rotation, origin, scale, effects,effect ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_sRec_Col_Rot_Origin_ScaleVec_Effects_Depth( layer, texture, position, sourceRectangle, col, rotation, origin, scale, effects ) );
        }

        public static void Draw( Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer, Effect effect )
        {
            instance.Enqueue( new DrawCall_Tex_Pos_sRec_Col_Rot_Origin_ScaleVec_Effects_Depth_Shader( layer, texture, position, sourceRectangle, col, rotation, origin, scale, effects,effect ) );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col )
        {
            instance.Enqueue( new DrawCall_Font_Text_Pos_Col( 0, spriteFont, text, position, col ) );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, SpriteEffect effect )
        {
            instance.Enqueue( new DrawCall_Font_Text_Pos_Col_Shader( 0, spriteFont, text, position, col, effect ) );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col )
        {
            instance.Enqueue( new DrawCall_Font_StringBuilder_Pos_Col( 0, spriteFont, text, position, col ) );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, SpriteEffect effect )
        {
            instance.Enqueue( new DrawCall_Font_StringBuilder_Pos_Col_Shader( 0, spriteFont, text, position, col, effect ) );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Font_Text_Pos_Col_Rot_Origin_Scale_Effects_Depth( layer, spriteFont, text, position, col, rotation, origin, scale, effects ) );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, SortingLayer layer, SpriteEffect effect )
        {
            instance.Enqueue( new DrawCall_Font_Text_Pos_Col_Rot_Origin_Scale_Effects_Depth_Shader( layer, spriteFont, text, position, col, rotation, origin, scale, effects,effect ) );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_Scale_Effects_Depth( layer, spriteFont, text, position, col, rotation, origin, scale, effects ) );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, float scale, SpriteEffects effects, SortingLayer layer, Effect effect )
        {
            instance.Enqueue( new DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_Scale_Effects_Depth_Shader( layer, spriteFont, text, position, col, rotation, origin, scale, effects,effect ) );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Font_Text_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth( layer, spriteFont, text, position, col, rotation, origin, scale, effects ) );
        }

        public static void DrawString( SpriteFont spriteFont, string text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer, SpriteEffect effect )
        {
            instance.Enqueue( new DrawCall_Font_Text_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth_Shader( layer, spriteFont, text, position, col, rotation, origin, scale, effects,effect ) );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer )
        {
            instance.Enqueue( new DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth( layer, spriteFont, text, position, col, rotation, origin, scale, effects ) );
        }

        public static void DrawString( SpriteFont spriteFont, StringBuilder text, Vector2 position, Color col, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, SortingLayer layer, SpriteEffect effect )
        {
            instance.Enqueue( new DrawCall_Font_StringBuilder_Pos_Col_Rot_Origin_ScaleVec_Effects_Depth_Shader( layer, spriteFont, text, position, col, rotation, origin, scale, effects,effect ) );
        }

        internal static void DrawPolygon( VertexPositionTexture[] mesh, Texture2D texture, BasicEffect shader, SortingLayer layer, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Camera cam )
        {            
            instance.Enqueue( new DrawCall_Mesh_Texture_Shader_Pos_Scale_Rot_Origin_Cam( layer, mesh, texture, shader, position, scale, rotation, origin, cam ));
        }
        #endregion
    }

}
