using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Voxel.Engine.Physics
{
    public class Collision
    {
        //AABB Point
        public static Hit Intersect(AABB a, Vector3 b)
        {
            Vector3 difference = b - a.Min;

            Vector3 p = a.LengthFromCenter - new Vector3(Math.Abs(difference.X), Math.Abs(difference.Y), Math.Abs(difference.Z));

            if (p.X <= 0 || p.Y <= 0 || p.Z <= 0)
                return null;

            Hit hit = new Hit(a);
            if(p.X < p.Z)
            {
                int x = Math.Sign(difference.X);
                hit.Delta.X = p.X * x;
                hit.Normal.X = x;
                hit.Position.X = a.Min.X + a.LengthFromCenter.X * x;
                hit.Position.Y = b.Y;
                hit.Position.Z = b.Z;
            }else if(p.Z < p.Y)
            {
                int z = Math.Sign(difference.Z);
                hit.Delta.Z = p.Z * z;
                hit.Normal.Z = z;
                hit.Position.Z = a.Min.Z + a.LengthFromCenter.Z * z;
                hit.Position.X = b.Z;
                hit.Position.Y = b.Y;
            }
            else
            {
                int y = Math.Sign(difference.Y);
                hit.Delta.Y = p.Y * y;
                hit.Normal.Y = y;
                hit.Position.Y = a.Min.Y + a.LengthFromCenter.Y * y;
                hit.Position.X = b.X;
                hit.Position.Z = b.Z;
            }

            return hit;
        }
        //AABB AABB
        public static Hit Intersect(AABB a, AABB b)
        {
            Vector3 d = a.Center - b.Center;
            Vector3 p = (a.LengthFromCenter + b.LengthFromCenter) - new Vector3(Math.Abs(d.X), Math.Abs(d.Y), Math.Abs(d.Z));
            
            if (p.X <= 0 || p.Y <= 0 || p.Z <= 0)
                return null;

            Hit hit = new Hit(a);
            if (p.X < p.Z && p.X < p.Y)
            {
                int x = Math.Sign(d.X);
                hit.Delta.X = p.X * x;
                hit.Normal.X = x;
                hit.Position.X = b.Min.X + (b.LengthFromCenter.X * x);
                hit.Position.Y = a.Min.Y;
                hit.Position.Z = a.Min.Z;
            } else if(p.Z < p.Y) {
                int z = Math.Sign(d.Z);
                hit.Delta.Z = p.Z * z;
                hit.Normal.Z = z;
                hit.Position.X = a.Min.X;
                hit.Position.Y = a.Min.Y;
                hit.Position.Z = b.Min.Z + (b.LengthFromCenter.Z * z);
            } else {
                int y = Math.Sign(d.Y);
                hit.Delta.Y = p.Y * y;
                hit.Normal.Y = y;
                hit.Position.X = a.Min.X;
                hit.Position.Y = b.Min.Y + (b.LengthFromCenter.Y * y);
                hit.Position.Z = a.Min.Z;
            }
            return hit;
        }
        //AABB AABBList
        public static Hit Intersect(AABB a, List<AABB> b)
        {
            Vector3 delta = Vector3.Zero;
            Hit hit = new Hit(a);
            foreach (AABB b1 in b)
            {
                Hit tempHit = Intersect(a, b1);
                if (tempHit != null && tempHit.Delta.Length() > delta.Length())
                {
                    delta = tempHit.Delta;
                    hit.Delta += tempHit.Delta;
                    hit.Normal = tempHit.Normal;
                    hit.Position = tempHit.Position;
                }
            }
            return hit;
        }
        //AABB Segment
        public static Hit Intersect(AABB b, Segment a)
        {
            Vector3 Scale;
            Vector3 Sign;
            Vector3 NearTime;
            Vector3 FarTime;

            Scale = Vector3.One / a.Delta;
            Sign = new Vector3(Math.Sign(Scale.X), Math.Sign(Scale.Y), Math.Sign(Scale.Z));
            NearTime = (b.Min - Sign * (b.LengthFromCenter + a.Padding) - a.Position) * Scale;
            FarTime = (b.Min - Sign * (b.LengthFromCenter + a.Padding) - a.Position) * Scale;

            if (NearTime.X > FarTime.X || NearTime.Y > FarTime.Y || NearTime.Z > FarTime.Z)
                return null;

            float nearTime, farTime;
            nearTime = NearTime.X > NearTime.Z ? NearTime.X : NearTime.Z > NearTime.Y? NearTime.Z : NearTime.Y;
            farTime = FarTime.X > FarTime.Z ? FarTime.X : FarTime.Z > FarTime.Y ? FarTime.Z : FarTime.Y;

            if (nearTime >= 1 || farTime <= 0)
                return null;

            Hit hit = new Hit(a);
            hit.time = MathHelper.Clamp(nearTime, 0, 1);
            if(NearTime.X > NearTime.Z)
            {
                hit.Normal.X = -Sign.X;
                hit.Normal.Y = 0;
                hit.Normal.Z = 0;
            }else if(NearTime.Z > NearTime.Y)
            {
                hit.Normal.X = 0;
                hit.Normal.Y = 0;
                hit.Normal.Z = -Sign.Z;
            }else
            {
                hit.Normal.X = 0;
                hit.Normal.Y = -Sign.Y;
                hit.Normal.Z = 0;
            }
            hit.Delta = hit.time * a.Delta;

            hit.Position = a.Position * hit.Delta;

            return hit;
        }

        //AABB Point
        public static bool Contains(AABB a, Vector3 b)
        {
            bool greatMin = a.Min.X < b.X && a.Min.Y < b.Y && a.Min.Z < b.Z;
            bool LessMax = a.Max.X > b.X && a.Max.Y > b.Y && a.Max.Z > b.Z;
            return greatMin && LessMax;
        }
        //AABB AABB
        public static bool Contains(AABB a, AABB b)
        {
            bool greatMin = a.Min.X < b.Min.X && a.Min.Y < b.Min.Y && a.Min.Z < b.Min.Z;
            bool LessMax = a.Max.X > b.Max.X && a.Max.Y > b.Max.Y && a.Max.Z > b.Max.Z;
            return greatMin && LessMax;
        }

       /* public static bool Intersect(List<AABB> a, AABB b)
        {
            foreach(AABB box in a)
                if (Intersect(b, box)) return true;
            return false;
        }
        public static Vector3 IntersectD(List<AABB> a, AABB b)
        {
            Vector3 distance = Vector3.Zero;
            foreach (AABB box in a)
            {
                if (Intersect(box, b))
                    return IntersectD(box, b);
            }
            return distance;
        }*/
        
    }
}
