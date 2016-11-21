using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxEngine.Helpers
{
    public class MathsHelper
    {
        public static float mod(float val, int mod)
        {
            return ((val % mod) + mod) % mod;
        }
    }
}
