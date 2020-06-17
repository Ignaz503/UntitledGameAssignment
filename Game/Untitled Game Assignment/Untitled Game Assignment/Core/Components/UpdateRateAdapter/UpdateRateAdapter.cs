using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Util.SortingLayers;
using UntitledGameAssignment.Core.GameObjects;
using Util.Rendering;
using Util.AssetManagment;
using Util.CustomDebug;
using Microsoft.Xna.Framework.Input;
using Util.FrameTimeInfo;
using Util.Input;

namespace UntitledGameAssignment.Core.Components
{
    public class UpdateRateAdapter : Component, IUpdate, ILateUpdate
    {
        int currentUpdateRateInMS;
        Keys increaseRate,decreaseRate;
        TextRenderer textRenderer;

        public UpdateRateAdapter(Keys increase, Keys decrease,GameObject obj ) : base( obj )
        {
            currentUpdateRateInMS = (int)(TimeInfo.FixedUpdateRate * 1000f);

            this.increaseRate = increase;
            this.decreaseRate = decrease;
            Transform.Scale = Vector2.One * 0.5f;
            CreateDisplay();
        }

        private void CreateDisplay()
        {
            textRenderer= GameObject.GetComponent<TextRenderer>();
            if (textRenderer == null)
            {
                textRenderer = GameObject.AddComponent(j=> new TextRenderer( $"Adatptive Update Rate: { currentUpdateRateInMS } ms", Color.White, SortingLayer.UI,j));
            }
            
            PlaceDisplay();
        }

        public void LateUpdate()
        {
            DisplayCurrentUpdateRate();
        }

        public override void OnDestroy()
        {}

        public void Update()
        {
            if(Input.IsKeyDown(increaseRate))
            {
                currentUpdateRateInMS -= 1;
            }
            if (Input.IsKeyDown( decreaseRate ))
            {
                currentUpdateRateInMS += 1;
            }
            currentUpdateRateInMS = Math.Max( 1, currentUpdateRateInMS );
            TimeInfo.FixedUpdateRate = (float)currentUpdateRateInMS / 1000f;
        }

        private void DisplayCurrentUpdateRate()
        {
            textRenderer.Text = $"Adatptive Update Rate: {currentUpdateRateInMS} ms";
            PlaceDisplay();
        }

        private void PlaceDisplay()
        {
            var textSize = textRenderer.ScaledTextSize;
            Vector2 pos =  Camera.Active.ScreenToWorld(new Vector2(GameMain.Instance.VirtualViewport.Viewport.Width,0));

            pos += new Vector2( -textSize.X*.5f, textRenderer.UnscaledTextSize.Y*.5f );

            Transform.Position = pos; 
        }
    }
}