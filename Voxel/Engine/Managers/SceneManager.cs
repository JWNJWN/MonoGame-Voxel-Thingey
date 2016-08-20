using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Voxel.Engine.Entities;
using Voxel.Engine.Entities.Components.Lights;
using Voxel.Engine.GeometricPrimitives;

using Voxel.Engine.World.Voxel;

namespace Voxel.Engine.Managers
{
    public class SceneManager : BaseManager
    {
        private Dictionary<string, BaseEntity> entities;
        public Dictionary<string, BaseEntity> Entities
        {
            get { return entities; }
        }

        public ContentManager Content
        {
            get { return Game.Content; }
        }

        protected override string GetName()
        {
            return "Scene";
        }

        public SceneManager(SceneGame Game) : base(Game)
        {
            this.entities = new Dictionary<string, BaseEntity>();

            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            VoxelIndexer voxIndex = new VoxelIndexer();
            
            PlayerEntity player = new PlayerEntity(this, EngineCommon.RootEntityName, Vector3.Zero);
            player.position = new Vector3(16, 40, 16);
            AddEntityToScene(player);

            BaseEntity directionalLight = new BaseEntity(this, "Sun");
            directionalLight.position = Vector3.Zero;
            directionalLight.rotation = Matrix.CreateRotationX(MathHelper.ToRadians(-45f)) * Matrix.CreateRotationY(MathHelper.ToRadians(30f));
            directionalLight.AddComponent(new DirectionalLightComponent(directionalLight, Vector3.Zero, Color.White, 1f));
            AddEntityToScene(directionalLight);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, BaseEntity> pair in entities)
                (pair.Value).Update(gameTime);
        }

        public void AddEntityToScene(BaseEntity entity)
        {
            BaseEntity checkEntity = null;
            if (entities.TryGetValue(entity.Name, out checkEntity))
                throw new Exception("An entity named " + entity.Name + " already exists.");
            entities.Add(entity.Name, entity);
        }

        public void RemoveEntityFromScene(BaseEntity entity)
        {
            BaseEntity checkEntity = null;
            if (!entities.TryGetValue(entity.Name, out checkEntity))
                throw new Exception("No entity named " + entity.Name + " exists in the scene to be removed.");
            entities.Remove(entity.Name);
        }

        public BaseEntity GetEntity(string entityName)
        {
            BaseEntity entity = null;
            if (!entities.TryGetValue(entityName, out entity))
                throw new Exception("No entity named " + entityName + " exists in the scene.");
            return entity;
        }

        public List<Light> GetLights()
        {
            List<Light> tempLight = new List<Light>();
            foreach (KeyValuePair<string, BaseEntity> pair in entities)
            {
                LightComponent light = (pair.Value).GetComponent("Light") as LightComponent;
                if (light != null)
                {
                    tempLight.Add(new Light(light.Parent.position + light.offset, light.Parent.rotation, light.color, light.intensity, light.type));
                }
            }
            return tempLight;
        }
    }
}
