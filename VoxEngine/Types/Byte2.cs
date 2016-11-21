using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace VoxEngine.Types
{
    public struct Byte2
    {
        public Byte2(byte a, byte b)
        {
            x = a;
            y = b;
        }

        public byte x, y;

        public static Byte2 Zero = new Byte2(0, 0);
        public static Byte2 One = new Byte2(1, 1);

        #region Operator Overloads
        public static Byte2 operator *(Byte2 a, byte b)
        {
            return new Byte2((byte)(a.x * b), (byte)(a.y * b));
        }
        public static Byte2 operator /(Byte2 a, byte b)
        {
            return new Byte2((byte)(a.x / b), (byte)(a.y / b));
        }
        public static Byte2 operator *(Byte2 a, Byte2 b)
        {
            return new Byte2((byte)(a.x * b.x), (byte)(a.y * b.y));
        }
        #endregion

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }
    }
}
