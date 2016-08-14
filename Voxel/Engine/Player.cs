using System;
using System.Dynamic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Voxel.Engine.Camera;
using Voxel.Engine.World;
using Voxel.Engine.Physics;
using Voxel.Engine.Entities;
using Voxel.Engine.Entities.Components;

namespace Voxel.Engine
{
    public class Player : Entity, Controllable, PhysicsObject
    {
        public PlayerCamera pCamera;

        public Vector3 prevPPosition;

        Vector3 pViewOffset = new Vector3(0.4f, 1.8f, 0.4f);

        Vector3 pForward;
        Vector3 pUp = Vector3.Up;

        public Vector3 pSize = new Vector3(0.8f, 2, 0.8f);

        public AABB pBoundingBox;

        float pSpeed = 50f;

        float pJumpHeight = 0.4f;

        //Temp Chunk Var
        Chunk chunk;

        public Game1 game;

        bool isGrounded = true;

        public Player(GraphicsDevice graphicsDevice, Chunk chunk)
        {
            this.chunk = chunk;
            pCamera = new PlayerCamera(graphicsDevice);
            pBoundingBox = new AABB(Position, pSize);
            TerminalVelocity = new Vector2(10, 20);
            Mass = 70;
        }

        public override void Start()
        {

        }

        TimeSpan oldTime = TimeSpan.Zero;

        Vector3 tempVel = Vector3.Zero;

        public override void Update()
        {
            pCamera.Update();
            Acceleration = Vector3.Zero;
            tempVel = Vector3.Zero;

            pForward = new Vector3(pCamera.cForward.X, 0, pCamera.cForward.Z);
            pForward.Normalize();


            HandleInput();
            HandleGrounding();

            Vector3 velocityChange = tempVel - Velocity;
            velocityChange.Y = 0;
            
            AddForce(velocityChange, pSpeed, ForceType.VelocityChange);

            if(!isGrounded)
                AddForce(Vector3.Down, 9.81f, ForceType.Acceleration);

            /*if (hit != null)
                AddForce(hit.Normal, 1f, ForceType.VelocityChange);*/
            

            Vector3 oldVelocity = Velocity;
            Velocity += Acceleration * Game1.GetDeltaTime();

            float HorizontalLengthSquared = new Vector2(Velocity.X, Velocity.Z).LengthSquared();

            if (HorizontalLengthSquared > TerminalVelocity.X * TerminalVelocity.X && HorizontalLengthSquared != 0)
            {
                float VelRatio = (float)(TerminalVelocity.X / Math.Sqrt(HorizontalLengthSquared));
                Velocity.X *= VelRatio;
                Velocity.Z *= VelRatio;
            }

            float VerticalLengthSqured = Velocity.Y * Velocity.Y;

            if (VerticalLengthSqured > TerminalVelocity.Y * TerminalVelocity.Y && VerticalLengthSqured != 0)
            {
                float VelRatio = TerminalVelocity.Y / Velocity.Y;
                Velocity.Y *= VelRatio;
            }


            pBoundingBox.Min += (oldVelocity + Velocity) * 0.5f * Game1.GetDeltaTime();
            
            Position = pBoundingBox.Min += hit.Delta;
            
            pCamera.cPosition = Position + pViewOffset;
        }

        public override void UpdateState()
        {
            throw new NotImplementedException();
        }

        public void HandleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                tempVel += pForward;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                tempVel -= pForward;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                tempVel -= pCamera.cRight;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                tempVel += pCamera.cRight;


            tempVel *= pSpeed;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && isGrounded)
                AddForce(Vector3.Up, CalculateJump(), ForceType.VelocityChange);

        }

        float CalculateJump()
        {
            return (float)Math.Sqrt(2 * pJumpHeight * 9.81f);
        }

        void HandleGrounding()
        {
            if (Velocity.Y > 0)
                isGrounded = false;
            if (collisionReaction.Y > 0)
                isGrounded = true;
        }
        Hit hit;
        Vector3 collisionReaction = Vector3.Zero;
        public void PhysicsUpdate()
        {
            hit = Collision.Intersect(pBoundingBox, chunk.boundingBoxes);
            if (hit != null)
            {
                collisionReaction = hit.Delta;
            }
            else
            {
                collisionReaction = Vector3.Zero;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
