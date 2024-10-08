
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGB283Vector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public IGB283Vector3(float X, float Y, float Z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                default: throw new System.IndexOutOfRangeException("Invalid index for Vector3");
            }
        }
        set
        {
            switch (index)
            {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                default: throw new System.IndexOutOfRangeException("Invalid index for Vector3");
            }
        }
    }

    public static IGB283Vector3 operator *(IGB283Vector3 a, float b)
    {
        return new IGB283Vector3(a.x * b, a.y * b, a.z * b);
    }


    public static IGB283Vector3 operator *(float scalar, IGB283Vector3 v)
    {
        return new IGB283Vector3(v.x * scalar, v.y * scalar, v.z * scalar);
    }


    public static IGB283Vector3 operator +(IGB283Vector3 a, IGB283Vector3 b)
    {
        return new IGB283Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }


    public static IGB283Vector3 operator +(IGB283Vector3 a, Vector3 b)
    {
        return new IGB283Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }


    public static IGB283Vector3 operator +(Vector3 a, IGB283Vector3 b)
    {
        return new IGB283Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static IGB283Vector3 operator -(IGB283Vector3 a, IGB283Vector3 b)
    {
        return new IGB283Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }


    public static IGB283Vector3 operator -(IGB283Vector3 a, Vector3 b)
    {
        return new IGB283Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }


    public static IGB283Vector3 operator -(Vector3 a, IGB283Vector3 b)
    {
        return new IGB283Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }


    public static IGB283Vector3 operator -(IGB283Vector3 a)
    {
        return new IGB283Vector3(-a.x, -a.y, -a.z);
    }


    public static float Dot(IGB283Vector3 a, IGB283Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }


    public static IGB283Vector3 Cross(IGB283Vector3 a, IGB283Vector3 b)
    {
        return new IGB283Vector3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );
    }

    public float Magnitude()
    {
        return Mathf.Sqrt(x * x + y * y + z * z);
    }


    public IGB283Vector3 Normalize()
    {
        float magnitude = Magnitude();
        return new IGB283Vector3(x / magnitude, y / magnitude, z / magnitude);
    }

    public IGB283Vector3 normalized
    {
        get
        {
            return Normalize();
        }
    }

    public static float Distance(IGB283Vector3 a, IGB283Vector3 b)
    {
        float num = a.x - b.x;
        float num2 = a.y - b.y;
        float num3 = a.z - b.z;
        return (float)Mathf.Sqrt(num * num + num2 * num2 + num3 * num3);
    }


    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }


    public static implicit operator Vector3(IGB283Vector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static implicit operator IGB283Vector3(Vector3 v)
    {
        return new IGB283Vector3(v.x, v.y, v.z);
    }

    public static IGB283Vector3 Up
    {
        get
        {
            return new IGB283Vector3(0, 1, 0);
        }
    }
    public static IGB283Vector3 operator /(IGB283Vector3 a, float b)
    {
        return new IGB283Vector3(a.x / b, a.y / b, a.z / b);
    }


    public static IGB283Vector3 Lerp(IGB283Vector3 a, IGB283Vector3 b, float t)
    {
        t = Mathf.Clamp01(t);
        return new IGB283Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    }
}
