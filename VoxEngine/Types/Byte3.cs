
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.Types
{
    public struct Byte3
    {
        public Byte3(byte a, byte b, byte c)
        {
            x = a;
            y = b;
            z = c;
        }

        public byte x, y, z;

        #region Operator Overloads
        public static Byte3 operator -(Byte3 a, Byte3 b)
        {
            return new Byte3((byte)Math.Abs(a.x - b.x), (byte)Math.Abs(a.y - b.y), (byte)Math.Abs(a.z - b.z));
        }
        #endregion

        public static Byte3 Zero = new Byte3(0, 0, 0);
        public static Byte3 One = new Byte3(1, 1, 1);


        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
