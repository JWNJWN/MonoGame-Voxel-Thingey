using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpNoise.Modules;

namespace Voxel.Engine.Entities.Components
{
    public class VoxelGeneratorComponent : BaseComponent
    {
        Perlin perlin;

        VoxelContainerComponent voxelContainer;
        VoxelRenderComponent voxelRender;
        
        public VoxelGeneratorComponent(BaseEntity parentEntity) : base(parentEntity)
        {
            Initialize();
        }

        protected override string GetName()
        {
            return "VoxelGenerator";
        }

        protected override void Initialize()
        {
            voxelContainer = Parent.GetComponent("VoxelContainer") as VoxelContainerComponent;
            if (voxelContainer == null)
                throw new Exception("Entity Contains " + GetName() + " but does not contain VoxelContainer.");

            voxelRender = Parent.GetComponent("VoxelRender") as VoxelRenderComponent;
            if (voxelRender == null)
                throw new Exception("Entity Contains " + GetName() + " but does not contain VoxelRenderer.");

            perlin = new Perlin();
            perlin.Seed = 1414;
            perlin.OctaveCount = 1;
            perlin.Lacunarity = 1;
            perlin.Persistence = 1;
            perlin.Frequency = 0.02;
            perlin.Quality = SharpNoise.NoiseQuality.Standard;

            for (int x = 0; x < voxelContainer.containerSize; x++)
            {
                for (int z = 0; z < voxelContainer.containerSize; z++)
                {
                    double pValue = (perlin.GetValue((x + Parent.position.X + 0.5) * voxelRender.Scale.Scale.X, 0, (z + Parent.position.Z + 0.5) * voxelRender.Scale.Scale.Z) + 1) / 2;

                    int height = (int)(pValue * voxelContainer.containerSize);

                    for (int y = 0; y < voxelContainer.containerSize; y++)
                    {
                        if (y == height)
                            voxelContainer.SetVoxel(x, y, z, 1);
                        if (y < height)
                            voxelContainer.SetVoxel(x, y, z, 2);
                    }
                }
            }
            base.Initialize();
        }
    }
}
