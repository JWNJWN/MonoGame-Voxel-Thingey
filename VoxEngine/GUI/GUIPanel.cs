using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.GameComponents;
using VoxEngine.Managers;

namespace VoxEngine.GUI
{
    public class GUIPanel : GUIObject
    {
        private Dictionary<string, GUIObject> _children = new Dictionary<string, GUIObject>();
        public IDictionary<string, GUIObject> Children
        {
            get { return _children; }
        }

        public GUIPanel() : base() { }
        public GUIPanel(GUIPanel parent, float x, float y, float width, float height, string panelName)
            : base(parent, x, y, width, height)
        {
            parent.Children.Add(panelName, this);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GUIObject child in _children.Values)
                child.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GUIObject child in _children.Values)
                child.Update(gameTime);

        }

        public override void HandleInput(GameTime gameTime, Input input)
        {
            foreach (GUIObject child in _children.Values)
                child.HandleInput(gameTime, input);
        }
    }
}
