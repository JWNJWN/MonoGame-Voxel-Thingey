using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoxEngine.GameComponents;

namespace VoxEngine.SceneObject.SceneGraph
{
    public class Node : IComparable
    {
        protected NodeList _nodes;
        public NodeList Nodes
        {
            get { return _nodes; }
        }

        public void Sort()
        {
            _nodes.Sort();
        }

        public Node()
        {
            _nodes = new NodeList();
        }

        public void AddNode(Node newNode)
        {
            _nodes.Add(newNode);
        }

        int IComparable.CompareTo(object obj)
        {
            SceneObjectNode node1 = (SceneObjectNode)this;
            SceneObjectNode node2 = (SceneObjectNode)obj;

            if (node1.SceneObject.Distance < node2.SceneObject.Distance)
                return -1;
            else if (node1.SceneObject.Distance > node2.SceneObject.Distance)
                return 1;
            else
                return 0;
        }

        public virtual void HandleInput(GameTime gameTime, Input input)
        {
            _nodes.ForEach(delegate (Node node)
            {
                node.HandleInput(gameTime, input);
            });
        }

        public virtual void Update(GameTime gameTime)
        {
            _nodes.ForEach(delegate (Node node)
            {
                node.Update(gameTime);
            });
        }

        public virtual void UnloadContent()
        {
            _nodes.ForEach(delegate (Node node)
            {
                node.UnloadContent();
            });
            _nodes.Clear();
        }

        public virtual void LoadContent()
        {
            _nodes.ForEach(delegate (Node node)
            {
                node.LoadContent();
            });
        }

        public virtual void Draw(GameTime gameTime)
        {
            _nodes.ForEach(delegate (Node node)
            {
                node.Draw(gameTime);
            });
        }

        public virtual void DrawCulling(GameTime gameTime)
        {
            _nodes.ForEach(delegate (Node node)
            {
                node.DrawCulling(gameTime);
            });
        }
    }
}
