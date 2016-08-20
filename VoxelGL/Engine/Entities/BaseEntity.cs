using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voxel.Engine.Render;
using Voxel.Engine.Managers;
using Voxel.Engine.Entities.Components;

namespace Voxel.Engine.Entities
{
    public class BaseEntity
    {
        private SceneManager manager;
        private string name;
        private Dictionary<string, BaseComponent> components;

        public Vector3 position = Vector3.Zero;
        public Matrix rotation = Matrix.Identity;

        public BaseEntity(SceneManager sceneManager, string entityName)
        {
            this.components = new Dictionary<string, BaseComponent>();

            this.manager = sceneManager;
            this.name = entityName;
        }

        public SceneManager Manager
        {
            get { return manager; }
        }

        public string Name
        {
            get { return name; }
        }

        public void AddComponent(BaseComponent component)
        {
            BaseComponent checkComponent = null;
            if (components.TryGetValue(component.Name, out checkComponent))
                throw new Exception("Component type " + component.Name + " already exists on this object: " + name);
            components.Add(component.Name, component);
        }

        public void RemoveComponent(BaseComponent component)
        {
            BaseComponent checkComponent = null;
            if (!components.TryGetValue(component.Name, out checkComponent))
                throw new Exception("Component Type " + component.Name + " doesn't exist on this object: " + name);
            components.Remove(component.Name);
        }

        public BaseComponent GetComponent(string componentName)
        {
            BaseComponent component = null;
            if(!components.TryGetValue(componentName, out component))
                throw new Exception("Component Type " + component.Name + " doesn't exist on this object: " + name);
            return component;
        }

        public void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, BaseComponent> pair in components)
                (pair.Value).Update(gameTime);
        }

        public void Draw(GameTime gameTime, List<RenderDescription> renderDescriptions)
        {
            foreach (KeyValuePair<string, BaseComponent> pair in components)
                (pair.Value).Draw(gameTime, renderDescriptions);
        }
    }
}
