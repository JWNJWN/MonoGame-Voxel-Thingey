using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VoxEngine.Voxels;
using LibNoise;

namespace VoxEngine.Managers
{
    public class SceneChunkManager : GameComponent
    {
        private static Dictionary<Vector3, Chunk> _chunks = new Dictionary<Vector3, Chunk>();

        private static bool _initialized = false;
        public static bool Initialized
        {
            get { return _initialized; }
        }

        private static Perlin _perlin;
        public static Perlin Perlin
        {
            get { return _perlin; }
        }

        public SceneChunkManager(Game game) : base(game)
        {
            Enabled = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            _perlin = new Perlin();
            _perlin.Frequency = 0.008f;
            _perlin.Lacunarity = 1f;
            _perlin.NoiseQuality = NoiseQuality.Standard;
            _perlin.OctaveCount = 1;
            _perlin.Persistence = 1f;
            _perlin.Seed = 100;

            _initialized = true;
        }

        public static void Draw(GameTime gameTime)
        {
            foreach (Chunk chunk in _chunks.Values)
                chunk.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Chunk chunk in _chunks.Values)
                chunk.Update(gameTime);
        }

        public static void DirtyChunks()
        {
            foreach (Chunk chunk in _chunks.Values)
                chunk.Dirty = true;
        }

        public static void RemoveChunk(Vector3 chunkPos)
        {
            Chunk tempChunk = GetChunk(chunkPos);
            if (tempChunk != null)
            {
                tempChunk.UnloadContent();
                _chunks.Remove(chunkPos);
            }
        }

        public static void AddChunk(Vector3 chunkPos)
        {
            Chunk tempChunk;
            if (_chunks.TryGetValue(chunkPos, out tempChunk))
                return;
            tempChunk = new Chunk(32, chunkPos);
            GenerateChunk(tempChunk);
            _chunks.Add(chunkPos, tempChunk);
        }

        public static Chunk GetChunk(Vector3 chunkPos)
        {
            return _chunks[chunkPos];
        }
       
        public static void GenerateChunk(Chunk tempChunk)
        {
            Random r = new Random();
            for (int x = (int)tempChunk.Position.X; x < (int)tempChunk.Position.X + tempChunk.Size; x++)
            {
                for (int z = (int)tempChunk.Position.Z; z < (int)tempChunk.Position.Z + tempChunk.Size; z++)
                {
                    double height = (_perlin.GetValue(x, 0, z) + 0.25) * tempChunk.Size;

                    for (int y = (int)tempChunk.Position.Y; y < (int)tempChunk.Position.Y + tempChunk.Size; y++)
                    {
                        if (y < height)
                            if (r.Next(0, 100) > 50)
                            {
                                tempChunk.SetVoxel(x, y, z, 1);
                            }
                            else
                            {
                                tempChunk.SetVoxel(x, y, z, 2);
                            }
                    }
                }
            }
        }
    }
}