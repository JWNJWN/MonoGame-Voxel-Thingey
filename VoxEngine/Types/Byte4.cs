
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.Types
{
    public struct Byte4
    {
        public Byte4(byte a, byte b, byte c, byte d)
        {
            x = a;
            y = b;
            z = c;
            w = d;
        }

        public Byte4(Byte2 a, Byte2 b)
        {
            x = a.x;
            y = a.y;
            z = b.x;
            w = b.y;
        }

        public Byte4(Byte3 a, byte b)
        {
            x = a.x;
            y = a.y;
            z = a.z;
            w = b;
        }
        public byte x, y, z, w;

        public static Byte4 Zero = new Byte4(0, 0, 0, 0);
        public static Byte4 One = new Byte4(1, 1, 1, 1);
        
    }
}
