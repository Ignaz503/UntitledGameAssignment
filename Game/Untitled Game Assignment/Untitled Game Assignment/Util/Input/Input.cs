using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace Util.Input
{
    public enum MouseButtons 
    {
        Left,
        Middle,
        Right
    }

    /// <summary>
    /// input manager v 0.1
    /// </summary>
    public static class Input
    {
        static GraphicsDeviceManager graphics;

        static KeyboardState prevKeyboardState;
        static KeyboardState currentKeyboardtState;

        static MouseState prevMouseState;
        static MouseState currentMouseState;

        #region Keyboard
        /// <summary>
        /// checks if a key was pressed this frame
        /// (true only one frame)
        /// </summary>
        /// <param name="key">the key interessted in</param>
        /// <returns>true if key down this frame</returns>
        public static bool IsKeyDown(Keys key ) 
        {
            return prevKeyboardState.IsKeyUp( key ) && currentKeyboardtState.IsKeyDown( key );
        }

        /// <summary>
        /// checks if a key was released this frame 
        /// (true only one frame)
        /// </summary>
        /// <param name="key">the key interessted in</param>
        /// <returns>true if key up this frame</returns>
        public static bool IsKeyUp( Keys key ) 
        {
            return prevKeyboardState.IsKeyDown( key ) && currentKeyboardtState.IsKeyUp( key );
        }

        /// <summary>
        /// returns true if the current key is being pressed (true over multiple frames)
        /// </summary>
        /// <param name="key">the key interessted in</param>
        /// <returns>true if key pressed</returns>
        public static bool IsKeyPressed( Keys key ) 
        {
            return currentKeyboardtState.IsKeyDown( key );
        }

        /// <summary>
        /// checks if a key is not being pressed
        /// (true over multiple frames)
        /// </summary>
        /// <param name="key">the key interessted in</param>
        /// <returns>true if key NOT pressed</returns>
        public static bool IsKeyNotPressed( Keys key ) 
        {
            return currentKeyboardtState.IsKeyUp( key );
        }


        /// <summary>
        /// updates the keyboard state
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void UpdateKeyboardState() 
        {
            prevKeyboardState = currentKeyboardtState;
            currentKeyboardtState = Keyboard.GetState();
        }

        #endregion

        #region mouse

        static Vector2 prevMousePosition;
        /// <summary>
        /// current mouse screen position
        /// </summary>
        public static Vector2 MousePosition { get; private set; }

        public static Vector2 MousePositionUV { get; private set; }

        /// <summary>
        /// the movement delta between the last two frames
        /// </summary>
        public static Vector2 MouseMovementDelta { get; private set; }

        public static float ScrollWheelValue 
        {
            get { return currentMouseState.ScrollWheelValue; }
        }

        public static float ScrollWheelDelta 
        { get; private set; }

        /// <summary>
        /// checks if a button was pressed this frame
        /// (true only one frame)
        /// </summary>
        /// <param name="button">the button interessted in</param>
        /// <returns>true if button down this frame</returns>
        public static bool IsKeyDown( MouseButtons button ) 
        {
            return GetButtonState( prevMouseState, button ) == ButtonState.Released && GetButtonState( currentMouseState, button ) == ButtonState.Pressed;
        }

        /// <summary>
        /// checks if a button was released this frame 
        /// (true only one frame)
        /// </summary>
        /// <param name="button">the button interessted in</param>
        /// <returns>true if button up this frame</returns>
        public static bool IsKeyUp( MouseButtons button ) 
        {
            return (GetButtonState( prevMouseState, button ) == ButtonState.Pressed) && (GetButtonState( currentMouseState, button ) == ButtonState.Released);
        }

        /// <summary>
        /// returns true if the current button is being pressed (true over multiple frames)
        /// </summary>
        /// <param name="button">the button interessted in</param>
        /// <returns>true if button pressed</returns>
        public static bool IsKeyPressed( MouseButtons button ) 
        {
            return GetButtonState( currentMouseState, button ) == ButtonState.Released;
        }

        /// <summary>
        /// checks if a button is not being pressed
        /// (true over multiple frames)
        /// </summary>
        /// <param name="button">the button interessted in</param>
        /// <returns>true if button NOT pressed</returns>
        public static bool IsKeyNotPressed( MouseButtons button ) 
        {
            return GetButtonState( currentMouseState, button ) == ButtonState.Released;
        }

        /// <summary>
        /// retrieves mouse button state
        /// </summary>
        /// <param name="fromState">from this state</param>
        /// <param name="button">retrive this button</param>
        /// <returns>the button state</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ButtonState GetButtonState( MouseState fromState, MouseButtons button ) 
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return fromState.LeftButton;
                case MouseButtons.Middle:
                    return fromState.MiddleButton;
                case MouseButtons.Right:
                    return fromState.RightButton;
                default:
                    throw new InputException( $"Unkown mouse button state requested {button}" );
            }
        }


        /// <summary>
        /// calculates the movement delta between the current and the next frame
        /// </summary>
        /// <returns>the movement delta</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Vector2 CalcMovementDelta() 
        {
            return MousePosition - prevMousePosition;
        }

        /// <summary>
        /// calculates the mouse position as an UV[0,1] in rgard to the graphics window
        /// </summary>
        /// <returns>the mouse pos scaled to [0,1]</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        static Vector2 CalcMousePosUV() 
        {
            return new Vector2( (float)MousePosition.X / (float)graphics.PreferredBackBufferWidth, (float)MousePosition.Y / (float)graphics.PreferredBackBufferHeight );
        }

        /// <summary>
        /// calculates the scroll wheel movement delta between this and prev frame
        /// </summary>
        /// <returns>the delta</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float CalcScrollDelta() 
        {
            return ScrollWheelValue - prevMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// updates the mouse state
        /// </summary>
        static void UpdateMouseState() 
        {
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            prevMousePosition = MousePosition;

            MousePosition = currentMouseState.Position.ToVector2();
            CalcMousePosUV();
            CalcMovementDelta();
            CalcScrollDelta();
        }
        #endregion

        public static bool AnyInput => currentKeyboardtState.GetPressedKeys().Length >= 0 || AnyMouseInput();

        private static bool AnyMouseInput()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed || currentMouseState.MiddleButton == ButtonState.Pressed
                || currentMouseState.ScrollWheelValue != 0;
        }


        /// <summary>
        /// initializes input manger to this graphics device
        /// </summary>
        /// <param name="gfx">this applications graphics device manager</param>
        internal static void Initialize( GraphicsDeviceManager gfx ) 
        {
            graphics = gfx;
            currentKeyboardtState = prevKeyboardState = new KeyboardState();
            currentMouseState = prevMouseState = new MouseState();
        }

        /// <summary>
        /// updates the input state for mouse and keyboard
        /// </summary>
        internal static void Update() 
        {
            UpdateKeyboardState();
            UpdateMouseState();
        }
    }

}
