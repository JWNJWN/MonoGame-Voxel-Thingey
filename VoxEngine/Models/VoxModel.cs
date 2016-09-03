using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using VoxEngine.Interfaces;
using VoxEngine.Managers;

namespace VoxEngine.Models
{
    public class VoxModel : IVoxModel
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private Model _baseModel;
        public Model BaseModel
        {
            get { return _baseModel; }
        }

        private bool _readyToRender = false;
        public bool ReadyToRender
        {
            get { return _readyToRender; }
        }

        public VoxModel() { }
        public VoxModel(string fileName)
        {
            _fileName = fileName;
        }

        public void LoadContent()
        {
            if(!string.IsNullOrEmpty(_fileName))
            {
                _baseModel = EngineManager.ContentManager.Load<Model>("Content/Models/" + _fileName);
                _readyToRender = true;
            }
        }
    }
}
