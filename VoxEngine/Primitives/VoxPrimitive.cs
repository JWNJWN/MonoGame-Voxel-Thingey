﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VoxEngine.Interfaces;
using VoxEngine.SceneObject;
using VoxEngine.Managers;
using VoxEngine.Textures;

namespace VoxEngine.Primitives
{
    public class VoxPrimitive : VoxSceneObject, IVoxPrimitive, IVoxLoadable
    {
        private List<VertexPositionColor> _vertices = new List<VertexPositionColor>();
        public List<VertexPositionColor> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        private List<uint> _indices = new List<uint>();
        public List<uint> Indices
        {
            get { return _indices; }
            set { _indices = value; }
        }

        private VertexBuffer _vertexBuffer;
        public VertexBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
            set { _vertexBuffer = value; }
        }

        private IndexBuffer _indexBuffer;
        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
            set { _indexBuffer = value; }
        }

        private PrimitiveType _primitiveType;
        public PrimitiveType PrimitiveType
        {
            get { return _primitiveType; }
            set { _primitiveType = value; }
        }
        
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private uint _currentVertex;
        public uint CurrentVertex
        {
            get { return _currentVertex; }
        }

        public VoxPrimitive() { }
        public VoxPrimitive(Color color)
        {
            _color = color;
            Material = new Material();
            Material.Shader = "primitive";
        }

        protected void AddVertex(Vector3 position, Color color)
        {
            _vertices.Add(new VertexPositionColor(position, color));
            _currentVertex++;
        }

        protected void AddIndex(uint index)
        {
            _indices.Add(index);
        }

        public virtual void LoadContent()
        {
            _vertexBuffer = new VertexBuffer(EngineManager.Device, VertexPositionColor.VertexDeclaration, _vertices.Count, BufferUsage.None);
            _vertexBuffer.SetData(_vertices.ToArray());
            _indexBuffer = new IndexBuffer(EngineManager.Device, IndexElementSize.ThirtyTwoBits, _indices.Count, BufferUsage.None);
            _indexBuffer.SetData(_indices.ToArray());
            ReadyToRender = true;
        }

        public void UnloadContent()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        public override void Draw(GameTime gameTime)
        {
            if (ReadyToRender)
            {
                EngineManager.Device.DepthStencilState = DepthStencilState.Default;

                Effect effect = ShaderManager.GetShader("primitive").BaseEffect;
                effect.Parameters["ModelTexture"].SetValue(TextureManager.GetTexture("missing").BaseTexture);

                effect.Parameters["World"].SetValue(World);
                effect.Parameters["View"].SetValue(CameraManager.ActiveCamera.View);
                effect.Parameters["Projection"].SetValue(CameraManager.ActiveCamera.Projection);
                effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(World)));
                
                effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(0.5f, -0.7f, 0.6f));

                effect.Parameters["DiffuseIntensity"].SetValue(0.25f);

                effect.Parameters["AmbientColor"].SetValue(Color.White.ToVector3());
                effect.Parameters["AmbientIntensity"].SetValue(0.1f);
                
                EngineManager.Device.SetVertexBuffer(_vertexBuffer);
                EngineManager.Device.Indices = _indexBuffer;

                effect.CurrentTechnique.Passes[0].Apply();
                EngineManager.Device.DrawIndexedPrimitives(_primitiveType, 0, 0, _vertices.Count, 0, (_primitiveType == PrimitiveType.LineList || _primitiveType == PrimitiveType.LineStrip) ? _indices.Count / 2 : _indices.Count / 3);
            }
        }
    }
}
