using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
namespace RemoteXDataLibary.Mathf
{
    public struct Vector2
    {
        public float x;
        public float y;
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }
        public static Vector2 Zero
        {
            get
            {
                return new Vector2(0, 0);
            }
        }
        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }
        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }
        public static Vector2 operator *(Vector2 lhs, float rhs)
        {
            return new Vector2(lhs.x * rhs, lhs.y * rhs);
        }
        public static Vector2 operator *(float lhs, Vector2 rhs)
        {
            return new Vector2(lhs * rhs.x, lhs * rhs.y);
        }
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (lhs.x == rhs.x) && (lhs.y == rhs.y);
        }
        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return (lhs.x != rhs.x) || (lhs.y != rhs.y);
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        static public implicit operator SKPoint(Vector2 v)
        {
            return new SKPoint(v.x, v.y);
        }

        static public implicit operator Vector2(SKPoint value)
        {
            return new Vector2(value.X, value.Y);
        }

    }

    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float[] values)
        {
            this.x = values[0];
            this.y = values[1];
            this.z = values[2];
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }


        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static Vector3 operator *(Vector3 lhs, float rhs)
        {
            return new Vector3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }
        public static Vector3 operator *(float lhs, Vector3 rhs)
        {
            return new Vector3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
        }
        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }

        public float[] Array
        {
            get
            {
                return new float[3] { x, y, z };
            }
        }
    }
}
