using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxEngine.SceneObject
{
    public class Material
    {
        private string _shader;
        public string Shader
        {
            get { return _shader; }
            set { _shader = value; }
        }

        private List<string> _textureList;
        public List<string> TextureList
        {
            get { return _textureList; }
            set { _textureList = value; }
        }
    }
}
