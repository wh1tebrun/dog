#region File Description
// AStar.cs
// Implementation of the A* algorithm.
#endregion

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Pathfinding;

public class AStar
{
	/*
    N.W    N    N.E
     \     |     /
      \    |    /
  W------Cell-------E
      /    |    \
     /     |     \
    S.W    S     S.E

    N   --> North (cost 10)
    S   --> South (cost 10)
    E   --> East (cost 10)
    W   --> West (cost 10)

    Use 10 and 14 as cost.

    Along one node we can traverse horizontally / vertically (cost 10)
    Along one node we can traverse diagonally (cost 14)
    Use 10 and 14 as cost because    1 * 10 = 10 and sqrt(2) * 10 is approx. 1.4 * 10 = 14

    N.W --> North-West (cost 14)
    N.E --> North-East (cost 14)
    S.W --> South-West (cost 14)
    S.E --> South-East (cost 14)

    |-----------------------------------------------------------------------------|
    | Pseudo-Code:                                                                |
    |-----------------------------------------------------------------------------|
    | OPEN   // set of nodes to be evaluated                                      |
    | CLOSED // set of nodes already evaluated                                    |
    | add the start node to OPEN                                                  |
    |                                                                             |
    | loop                                                                        |
    |    current = node in OPEN with the lowest f_cost                            |
    |    remove current from OPEN                                                 |
    |    add current to CLOSED                                                    |
    |                                                                             |
    |    if current is the target node // path has been found                     |
    |        return                                                               |
    |                                                                             |
    |    foreach neighbour of the current node                                    |
    |        if neighbour is not traversable or neighbour is in CLOSED            |
    |            skip to the next neighbour                                       |
    |                                                                             |
    |        if new path to neighbour is shorter OR neighbour is not in OPEN      |
    |            set f_cost of neighbour to current                               |
    |            if neighbour is not in OPEN                                      |
    |                add neighbour to OPEN                                        |
    |-----------------------------------------------------------------------------|
    */

	private Node _startNode;
	private Node _targetNode;
	private const int TileSize = 16;
	private readonly Texture2D _texture;

	private Dictionary<int, Node> _open;
	private HashSet<int> _closedInd;
	public List<Node> Path;

	private readonly Grid _grid;

	/// <summary>
	/// Initializes the AStar algorithm.
	/// </summary>
	/// <param name="graphicsDevice"></param>
	/// <param name="grid"> The grid on which the search operates. </param>
	public AStar(GraphicsDevice graphicsDevice, Grid grid)
	{
		_grid = grid;
		_texture = new Texture2D(graphicsDevice, 1, 1);
		_texture.SetData([Color.Aqua]);

		_closedInd = [];
		Path = [];
	}

	/// <summary>
	/// Run the AStar algorithm.
	/// </summary>
	public void Update(Microsoft.Xna.Framework.Vector2 startPosition, Microsoft.Xna.Framework.Vector2 targetPosition)
	{
		if ((int)(-1 * (startPosition.Y - 1080) / 1080) != (int)(-1 * (targetPosition.Y - 1080) / 1080))
		{
			return;
		}
		const int offset = 64;
		var maxNodesToSearch = _grid.Width * _grid.Height / 4;
		int nodesProcessed = 0;

		_startNode = new Node
		{
			Position = new Vector2((int)startPosition.X / TileSize, (int)startPosition.Y / TileSize)
		};
		_startNode.Index = _startNode.Position.X * _grid.Height + _startNode.Position.Y;

		var targetX = (int)(targetPosition.X + offset) / TileSize;
		var targetY = (int)(targetPosition.Y + offset) / TileSize;
		_targetNode = new Node
		{
			Position = new Vector2(targetX, targetY)
		};
		_targetNode.Index = _targetNode.Position.X * _grid.Height + _targetNode.Position.Y;

		_open = new Dictionary<int, Node> { { _startNode.Index, _startNode } };
		_closedInd = new HashSet<int>();
		Path = new List<Node>();

		while (_open.Count > 0)
		{
			if (nodesProcessed >= maxNodesToSearch)
			{
				Path = null;
				return;
			}

			var (key, current) = _open.MinBy(n => n.Value.FCost);
			if (current.Position == _targetNode.Position)
			{
				_targetNode.Parent = current;
				TracePath();
				return;
			}

			_open.Remove(key);
			_closedInd.Add(current.Index);
			nodesProcessed++;

			foreach (var neighbour in GetNeighbours(current))
			{
				if (!neighbour.Walkable || _closedInd.Contains(neighbour.Index))
					continue;

				var tentativeGCost = current.GCost + CalculateDistance(current, neighbour);
				if (tentativeGCost >= neighbour.GCost && _open.ContainsKey(neighbour.Index))
					continue;

				if (_open.ContainsKey(neighbour.Index))
					_open.Remove(neighbour.Index);

				neighbour.GCost = tentativeGCost;
				neighbour.HCost = CalculateDistance(neighbour, _targetNode);
				neighbour.Parent = current;
				_open.Add(neighbour.Index, neighbour);
			}
		}
	}

	/// <summary>
	/// Trace the path from the target node to the start node.
	/// The current node starts from the _targetNode and uses the parent to determine where it came from.
	/// </summary>
	private void TracePath()
	{
		for (var current = _targetNode; current != null; current = current.Parent)
			Path.Insert(0, current);
	}

	/// <summary>
	/// Calculate the chebyshev distance between two nodes.
	/// </summary>
	/// <param name="current"> The current node. </param>
	/// <param name="target"> The target node. </param>
	/// <returns> The distance between those nodes. </returns>
	private static int CalculateDistance(Node current, Node target)
	{
		var dx = Math.Abs(target.Position.X - current.Position.X);
		var dy = Math.Abs(target.Position.Y - current.Position.Y);
		return 14 * Math.Min(dx, dy) + 10 * Math.Abs(dx - dy);
	}

	private static readonly (int dx, int dy)[] NeighborOffsets =
	{
	(-1, -1), (-1, 0), (-1, 1),
	(0, -1),           (0, 1),
	(1, -1),   (1, 0), (1, 1)
	};

	/// <summary>
	/// Retrieve the neighbours from the given node.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	private List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new();
		foreach (var (dx, dy) in NeighborOffsets)
		{
			var neighbourX = node.Position.X + dx;
			var neighbourY = node.Position.Y + dy;

			if (neighbourX < 0 || neighbourX >= _grid.Width || neighbourY < 0 || neighbourY >= _grid.Height)
				continue;

			int index = neighbourX * _grid.Height + neighbourY;
			if (_closedInd.Contains(index) || !_grid.Tiles[index])
				continue;

			neighbours.Add(new Node
			{
				Position = new Vector2(neighbourX, neighbourY),
				Walkable = true,
				Index = index
			});
		}
		return neighbours;
	}

	/// <summary>
	/// Draw the path between the start_node and the end_node by following the traced path.
	/// </summary>
	/// <param name="spriteBatch"></param>
	public void Draw(SpriteBatch spriteBatch)
	{
		if (Path == null) return;
		// Draw path if available
		foreach (var node in Path)
		{
			spriteBatch.Draw(_texture, new Rectangle(node.Position.X * TileSize, node.Position.Y * TileSize, TileSize, TileSize), Color.White);
		}
	}
}
