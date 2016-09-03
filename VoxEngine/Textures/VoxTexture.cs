using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.Managers;

namespace VoxEngine.Textures
{
    public class VoxTexture : IVoxTexture
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private Texture _baseTexture;
        public Texture BaseTexture
        {
            get { return _baseTexture; }
        }

        private bool _readyToRender = false;
        public bool ReadyToRender
        {
            get { return _readyToRender; }
        }

        public VoxTexture() { }

        public VoxTexture(string fileName)
        {
            _fileName = fileName;
        }

        public void LoadContent()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                _baseTexture = EngineManager.ContentManager.Load<Texture>("Content/Textures/" + _fileName);
                _readyToRender = true;
            }
        }

        public void UnloadContent()
        {
            _baseTexture.Dispose();
        }
    }
}
