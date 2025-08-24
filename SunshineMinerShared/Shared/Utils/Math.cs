using System;
using System.Collections;
using System.Collections.Generic;

public class Vec2
{
    public float x;
    public float y;

    public Vec2(float x_ = 0f, float y_ = 0f)
    {
        x = x_;
        y = y_;
    }

    bool Equal(Vec2 o)
    {
        if (Math.Abs(x - o.x) > 0.0001f) return false;
        if (Math.Abs(y - o.y) > 0.0001f) return false;
        return true;
    }
}

public class Vec3
{
    public float x;
    public float y;
    public float z;

    public Vec3(float x_ = 0f, float y_ = 0f, float z_ = 0f)
    {
        x = x_;
        y = y_;
        z = z_;
    }

    bool Equal(Vec3 o)
    {
        if (Math.Abs(x - o.x) > 0.0001f) return false;
        if (Math.Abs(y - o.y) > 0.0001f) return false;
        if (Math.Abs(z - o.z) > 0.0001f) return false;
        return true;
    }
}
