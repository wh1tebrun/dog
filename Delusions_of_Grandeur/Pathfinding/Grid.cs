#region File Description
// Grid.cs
// The grid for the pathfinding algorithm.
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Pathfinding;

public class Grid
{
    public readonly int Width;
    public readonly int Height;

    private readonly Texture2D _texture;

    private readonly Manager.MapManager _mapManager;
    public List<bool> Tiles { get; }

    private int _previousMapIndex = -1;
    private int _currentMapIndex;
    
    /// <summary>
    /// Constructor that initializes the grid.
    /// </summary>
    public Grid(GraphicsDevice graphicsDevice, Manager.MapManager mapManager)
    {
        Width = Consts.ScreenWidth / Consts.TileSize;
        Height = Consts.ScreenHeight / Consts.TileSize;
        Tiles = [];
        _mapManager = mapManager;

        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData([Color.DarkSlateGray]);
    }
    public void Update(Manager.MapData map, int mapIndex)
    {
        _currentMapIndex = mapIndex; 
        if (_previousMapIndex == _currentMapIndex) return;
        _previousMapIndex = _currentMapIndex;
        
        Tiles.Clear();

        for (var col = 0; col < Width; col++)
        {
            for (var row = 0; row < Height; row++)
            {
                var walkable = !map.RangedEnemies[col, row];
                Tiles.Add(walkable);
            }
        }
    }
}
