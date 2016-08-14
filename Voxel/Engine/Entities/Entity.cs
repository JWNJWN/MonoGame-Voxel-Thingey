using System;

using Microsoft.Xna.Framework;

namespace Voxel.Engine.Entities
{
    public abstract class Entity
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Acceleration = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector2 TerminalVelocity = Vector2.Zero;
        public ActionState state = ActionState.Stationary;
        public float Mass = 1f;
        public float Health = 0f;

        public abstract void Start();

        public virtual void Update()
        {
            Vector3 oldVelocity = Velocity;
            Velocity += Acceleration * Game1.GetDeltaTime();

            float HorizontalLengthSquared = new Vector2(Velocity.X, Velocity.Z).LengthSquared();

            if (HorizontalLengthSquared > TerminalVelocity.X * TerminalVelocity.X && HorizontalLengthSquared != 0)
            {
                float VelRatio = (float)(TerminalVelocity.X / Math.Sqrt(HorizontalLengthSquared));
                Velocity.X *= VelRatio;
                Velocity.Z *= VelRatio;
            }

            float VerticalLengthSqured = Velocity.Y * Velocity.Y * 2;

            if(VerticalLengthSqured > TerminalVelocity.Y * TerminalVelocity.Y && VerticalLengthSqured != 0)
            {
                float VelRatio = TerminalVelocity.Y / Velocity.Y;
                Velocity.Y *= VelRatio;
            }

            Position += (oldVelocity + Velocity) * 0.5f * Game1.GetDeltaTime();
        }

        public abstract void UpdateState();

        public void AddForce(Vector3 Direction, float Force, ForceType forceType = ForceType.Force)
        {
            switch(forceType)
            {
                case ForceType.Force:
                    Acceleration += (Direction * Force) / Mass;
                    break;
                case ForceType.Impulse:
                    Acceleration += (Direction * Force) / (Mass * Game1.GetDeltaTime());
                    break;
                case ForceType.Acceleration:
                    Acceleration += Direction * Force;
                    break;
                case ForceType.VelocityChange:
                    Acceleration += (Direction * Force) / Game1.GetDeltaTime();
                    break;

            }
        }
    }
}
