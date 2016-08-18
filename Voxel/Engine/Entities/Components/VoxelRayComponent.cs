using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Voxel.Engine.Managers;
using Voxel.Engine.World;

namespace Voxel.Engine.Entities.Components
{
    public class VoxelRayComponent : BaseComponent
    {
        ChunkManager chunkMgr;

        public VoxelRayComponent(BaseEntity parentEntity) : base(parentEntity)
        {
            Initialize();
        }

        protected override string GetName()
        {
            return "VoxelRay";
        }

        int i = 10;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (chunkMgr == null)
                chunkMgr = this.Parent.Manager.Game.GetManager("Chunk") as ChunkManager;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && i==10)
                CastRay(false);
            if (Mouse.GetState().RightButton == ButtonState.Pressed && i == 10)
                CastRay(true);
            if(i<10)
                i++;
        }

        void CastRay(bool t)
        {

            ///TODO: OPTIMIZE DIS PLZ
            i = 0;
            float range = 100;
            Vector3 origin = new Vector3((float)Math.Floor(Parent.position.X), (float)Math.Floor(Parent.position.Y), (float)Math.Floor(Parent.position.Z));
            Vector3 diff = Parent.rotation.Forward;

            Vector3 step = new Vector3(signum(diff.X), signum(diff.Y), signum(diff.Z));
            Vector3 tMax = new Vector3(intbound(Parent.position.X, diff.X), intbound(Parent.position.Y, diff.Y), intbound(Parent.position.Z, diff.Z));
            Vector3 tDelta = step / diff;

            Vector3 face = new Vector3();

            if (diff == Vector3.Zero)
                throw new Exception("Raycast in Zero Direction");

            float radius = 1 / (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y + diff.Z + diff.Z);

            while (true)
            {
                Chunk currentChunk = chunkMgr.GetChunk((int)origin.X, (int)origin.Y, (int)origin.Z);
                if (currentChunk != null)
                {
                    Vector3i voxelPosition = new Vector3i(origin);
                    if (currentChunk.GetVoxel(voxelPosition.x, voxelPosition.y, voxelPosition.z) > 0)
                    {
                        if (!t)
                        {
                            currentChunk.SetVoxel(voxelPosition.x, voxelPosition.y, voxelPosition.z, 0);
                        }
                        else
                        {
                            currentChunk.SetVoxel(voxelPosition.x + (int)face.X, voxelPosition.y + (int)face.Y, voxelPosition.z + (int)face.Z, 3);
                        }
                        break;
                    }
                }
                if (tMax.X < tMax.Y)
                {
                    if (tMax.X < tMax.Z)
                    {
                        if (tMax.X > range) break;

                        origin.X += step.X;
                        tMax.X += tDelta.X;

                        face.X = -step.X;
                        face.Y = 0;
                        face.Z = 0;
                    }
                    else
                    {
                        if (tMax.Z > range) break;
                        origin.Z += step.Z;
                        tMax.Z += tDelta.Z;
                        face.X = 0;
                        face.Y = 0;
                        face.Z = -step.Z;
                    }
                }
                else
                {
                    if (tMax.Y < tMax.Z)
                    {
                        if (tMax.Y > range) break;
                        origin.Y += step.Y;
                        tMax.Y += tDelta.Y;
                        face.X = 0;
                        face.Y = -step.Y;
                        face.Z = 0;
                    }
                    else
                    {
                        if (tMax.Z > range) break;
                        origin.Z += step.Z;
                        tMax.Z += tDelta.Z;
                        face.X = 0;
                        face.Y = 0;
                        face.Z = -step.Z;
                    }
                }
            }

        }

        float intbound(float s, float ds)
        {
            if (ds < 0)
            {
                return intbound(-s, -ds);
            }
            else
            {
                s = mod(s, 1);
                return (1 - s) / ds;
            }
        }

        int signum(float x)
        {
            return x > 0 ? 1 : x < 0 ? -1 : 0;
        }

        float mod(float val, int mod)
        {
            return ((val % mod) + mod) % mod;
        }
    }
}
