using System;
using UnityEngine;

[Serializable]
public struct HexCoordinates : IEquatable<HexCoordinates>
{
    [SerializeField] private int q;
    [SerializeField] private int r;

    public int Q => q;
    public int R => r;
    public int S => -q - r;

    public HexCoordinates(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public static readonly HexCoordinates[] Directions = new HexCoordinates[]
    {
        new HexCoordinates(+1, 0),  // East
        new HexCoordinates(+1, -1), // Northeast
        new HexCoordinates(0, -1),  // Northwest
        new HexCoordinates(-1, 0),  // West
        new HexCoordinates(-1, +1), // Southwest
        new HexCoordinates(0, +1),  // Southeast
    };

    public static HexCoordinates Zero => new HexCoordinates(0, 0);

    public HexCoordinates GetNeighbor(int direction)
    {
        return this + Directions[direction % 6];
    }

    public HexCoordinates[] GetAllNeighbors()
    {
        var neighbors = new HexCoordinates[6];
        for (int i = 0; i < 6; i++)
            neighbors[i] = GetNeighbor(i);
        return neighbors;
    }

    public int DistanceTo(HexCoordinates other)
    {
        return (Math.Abs(Q - other.Q) + Math.Abs(R - other.R) + Math.Abs(S - other.S)) / 2;
    }

    public static HexCoordinates operator +(HexCoordinates a, HexCoordinates b)
        => new HexCoordinates(a.q + b.q, a.r + b.r);

    public static HexCoordinates operator -(HexCoordinates a, HexCoordinates b)
        => new HexCoordinates(a.q - b.q, a.r - b.r);

    public bool Equals(HexCoordinates other) => q == other.q && r == other.r;
    public override bool Equals(object obj) => obj is HexCoordinates other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(q, r);
    public static bool operator ==(HexCoordinates a, HexCoordinates b) => a.Equals(b);
    public static bool operator !=(HexCoordinates a, HexCoordinates b) => !a.Equals(b);

    public override string ToString() => $"({Q}, {R})";
}
