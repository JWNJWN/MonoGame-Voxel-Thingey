using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.Managers;
using VoxEngine.GameComponents;

namespace VoxEngine.GUI
{
    public class GUIObject
    {
        private GUIObject _parent;
        public GUIObject Parent
        {
            get { return _parent; }
        }

        private Rectangle _bounds;
        public Rectangle Bounds
        {
            get { return _bounds; }
        }

        public GUIObject()
        {
            _bounds = new Rectangle(0, 0, EngineManager.Width, EngineManager.Height);
        }

        public GUIObject(GUIObject parent, int x, int y, int width, int height)
        {
            _parent = parent;
            _bounds = new Rectangle(x, y, width, height);
        }

        public GUIObject(GUIObject parent, float relativeX, float relativeY, float relativeWidth, float relativeHeight)
            : this(parent, (int)Math.Floor(relativeX * parent.Bounds.Width) + parent.Bounds.X, (int)Math.Floor(relativeY * parent.Bounds.Height) + parent.Bounds.Y, (int)Math.Floor(relativeWidth * parent.Bounds.Width), (int)Math.Floor(relativeHeight * parent.Bounds.Height)) { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void HandleInput(GameTime gameTime, Input input) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
