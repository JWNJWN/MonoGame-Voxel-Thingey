using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Voxel.Engine.Entities;
using Voxel.Engine.Entities.Components;
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

            RenderComponent rendComp;

            PlayerEntity player = new PlayerEntity(this, EngineCommon.RootEntityName, Vector3.Zero);
            player.position = new Vector3(5, 5, 5);
            player.rotation = Matrix.CreateRotationY(MathHelper.ToRadians(45)) * Matrix.CreateRotationX(MathHelper.ToRadians(-45));
            AddEntityToScene(player);

            /*ChunkEntity chunk = new ChunkEntity(this, new Vector3(0, 0, 0));
            AddEntityToScene(chunk);
            chunk = new ChunkEntity(this, new Vector3(32, 0, 0));
            AddEntityToScene(chunk);
            chunk = new ChunkEntity(this, new Vector3(32, 0, 32));
            AddEntityToScene(chunk);
            chunk = new ChunkEntity(this, new Vector3(0, 0, 32));
            AddEntityToScene(chunk);*/

            BaseEntity testCube = new BaseEntity(this, "Cube");
            testCube.position = new Vector3(0, 2, 2);

            rendComp = new RenderComponent(testCube, GeometricPrimitiveType.Cube, Color.Red);

            AddEntityToScene(testCube);

            BaseEntity testCube2 = new BaseEntity(this, "Cube2");
            testCube2.position = new Vector3(2, 1, 0);

            rendComp = new RenderComponent(testCube2, GeometricPrimitiveType.Cube, Color.Plum);

            AddEntityToScene(testCube2);

            BaseEntity testTeapot = new BaseEntity(this, "Teapot");
            testTeapot.position = new Vector3(0, 2, 0);

            rendComp = new RenderComponent(testTeapot, GeometricPrimitiveType.Teapot);

            AddEntityToScene(testTeapot);

            BaseEntity groundPlane = new BaseEntity(this, "groundPlane");
            groundPlane.position = new Vector3(0, 0, 0);

            rendComp = new RenderComponent(groundPlane, GeometricPrimitiveType.Cube, Color.MonoGameOrange);
            rendComp.Scale = Matrix.CreateScale(50, 0.1f, 50);

            AddEntityToScene(groundPlane);
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
                throw new Exception("No entity named " + entity.Name + " exists in th scene.");
            return entity;
        }
    }
}
