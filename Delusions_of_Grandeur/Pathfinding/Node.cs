#region File Description
// Node.cs
// Represent a Node for the pathfinding algorithm.
#endregion

using System;

namespace Delusions_of_Grandeur.Pathfinding;

public record struct Vector2(int X, int Y)
{
    public readonly int X = X;
    public readonly int Y = Y;
}

/// <summary>
/// Represents a node in the pathfinding grid.
/// </summary>
public class Node
{
    /// <summary>
    /// Indicates whether the node is walkable.
    /// </summary>
    public bool Walkable { get; init; } = true;

    public int Index = Int32.MaxValue;
    public Node Parent { get; set; }
    /// <summary>
    /// Position of the node in screen-space.
    /// </summary>
    public Vector2 Position { get; init; }

    /// <summary>
    /// Distance from the starting node.
    /// </summary>
    public int GCost { get; set; }

    /// <summary>
    /// Heuristic = Distance from the end node.
    /// </summary>
    public int HCost { get; set; }

    /// <summary>
    /// F cost = G cost + H cost.
    /// </summary>
    public int FCost => GCost + HCost;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Node() { }

    /// <summary>
    /// Constructor to initialize a node with a position and walkability.
    /// </summary>
    /// <param name="position">The position of the node in screen-space.</param>
    /// <param name="walkable">Whether the node is walkable.</param>
    public Node(Vector2 position, bool walkable)
    {
        Position = position;
        Walkable = walkable;
        GCost = 0;
        HCost = 0;
    }
}