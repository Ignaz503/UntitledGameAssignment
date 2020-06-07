using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Util.Input;
using Util.CustomDebug;
using UntitledGameAssignment.Core.GameObjects;

public class Player : GameObject
    {
        public Vector2 Target;
        public float walkSpeed;

        public Player()
        {
            Target = new Vector2();
            walkSpeed = 1.2f;
        }

        /*public override void FixedUpdate()
        {
            base.FixedUpdate();
            RotateToCursor();
            MovePlayer();
        } TODO: fix this with TimeInfo*/

        private void RotateToCursor()
        {
            Target = Input.MousePosition;
            System.Diagnostics.Debug.WriteLine("Mouse: " + Target.X + " " + Target.Y);
            //Debug.Log("Mouse: " + Target.X + " " + Target.Y);

            Vector2 center = Transform.Position;
            Vector2 dir = (Target - center);
            dir.Normalize();
            float rot = (float)Math.Atan(dir.Y / dir.X) + 90;
            
            //TODO: fix issue that sprite flips when passing 90 degree threshold
            Transform.Rotation = rot;
        }

        private void MovePlayer()
        {
            if (Input.IsKeyPressed(Keys.W))
            {
                Transform.Position += new Vector2(0, -walkSpeed);
            }
            if(Input.IsKeyPressed( Keys.A))
            {
                Transform.Position += new Vector2(-walkSpeed, 0);
            }
            if (Input.IsKeyPressed( Keys.S))
            {
                Transform.Position += new Vector2(0, walkSpeed);
            }
            if (Input.IsKeyPressed( Keys.D))
            {
                Transform.Position += new Vector2(walkSpeed, 0);
            }
        }
    }
