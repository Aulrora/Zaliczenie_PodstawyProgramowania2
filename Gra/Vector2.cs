namespace ClassesIntro;

public struct Vector2 : IEquatable<Vector2>
{
    public int x;
    public int y;

    public Vector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.x + b.x, a.y + b.y);
    }

    public bool Equals(Vector2 other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}
