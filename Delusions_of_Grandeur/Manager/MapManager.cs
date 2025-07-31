// MapManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Delusions_of_Grandeur.Manager;

public class MapManager
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private TiledMapRenderer _mapRenderer;
    public readonly List<MapData> Maps;

    private const int TileSize = 16;
    private bool[,] _meleeEnemy;

    public static List<Rectangle> PotionRectangles { get; set; }
    public static List<Rectangle> WeaponRectangles { get; set; }


    /// <summary>
    /// Constructs a MapManager object.
    /// </summary>
    /// <param name="graphicsDevice"> Used for rendering to the screen. </param>
    /// <param name="content"> Used for loading a tilemap. </param>
    public MapManager(GraphicsDevice graphicsDevice, ContentManager content)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        Maps = [];
        PotionRectangles = new List<Rectangle>();
        WeaponRectangles = new List<Rectangle>();
    }

    /// <summary>
    /// Load a map from a .tmx-file.
    /// </summary>
    public void LoadMaps()
    {
        Maps.Clear();
        WeaponRectangles.Clear();

        var filePath = Path.Combine("Content", "Maps", "tower.world");
        var jsonContent = File.ReadAllText(filePath);

        var worldData = JsonConvert.DeserializeObject<WorldData>(jsonContent);

        foreach (var map in worldData.Maps)
        {
            var currentMap = _content.Load<TiledMap>(Path.Combine("Maps", map.FileName));
            currentMap.GetLayer<TiledMapTileLayer>("Collision");
            currentMap.GetLayer<TiledMapTileLayer>("MeleeEnemy");
            currentMap.GetLayer<TiledMapTileLayer>("Collectable");
            currentMap.GetLayer<TiledMapTileLayer>("rangedEnemies");
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, currentMap);
            var potionLayer = currentMap.GetLayer<TiledMapObjectLayer>("PotionLayer");
            var weaponLayer = currentMap.GetLayer<TiledMapObjectLayer>("Weapon");

            Maps.Add(new MapData(currentMap, _mapRenderer, new Vector2(map.X, map.Y),
                new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height],
                new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], false, false));
            if (potionLayer != null)
            {
                foreach (var potion in potionLayer.Objects)
                {
                    var rect = new Rectangle((int)potion.Position.X + map.X, (int)potion.Position.Y + map.Y, (int)potion.Size.Width, (int)potion.Size.Height);
                    PotionRectangles.Add(rect);
                }
            }
                
            if (weaponLayer != null)
            {
                foreach (var weapon in weaponLayer.Objects)
                {
                    var rect = new Rectangle((int)weapon.Position.X + map.X, (int)weapon.Position.Y + map.Y, (int)weapon.Size.Width, (int)weapon.Size.Height);
                    WeaponRectangles.Add(rect);
                }
            }
        }

        // Set collision on first map.
        Maps[0].IsCollisionActive = true;

        foreach (var map in Maps)
        {
            var collisionLayers = map.Map.TileLayers.Where(tl =>
                tl.Properties.ContainsKey("Collision") && Convert.ToBoolean(tl.Properties["Collision"]));
            var rangedEnemiesLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("rangedEnemies") && Convert.ToBoolean(tl.Properties["rangedEnemies"]));

            for (ushort x = 0; x < map.Map.Width; x++)
            {
                for (ushort y = 0; y < map.Map.Height; y++)
                {
                    foreach (var layer in collisionLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        // Add the collidable tile to the array.
                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.Collidable[x, y] = true;
                        }
                    }
                    
                    foreach (var layer in rangedEnemiesLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.RangedEnemies[x, y] = true;
                        }
                    }
                }
            }
        }

        foreach (var map in Maps)
        {
            var meleeEnemyLayers = map.Map.TileLayers.Where(tl =>
                tl.Properties.ContainsKey("MeleeEnemy") && Convert.ToBoolean(tl.Properties["MeleeEnemy"]));
            _meleeEnemy = new bool[map.Map.Width, map.Map.Height];

            for (ushort x = 0; x < map.Map.Width; x++)
            {
                for (ushort y = 0; y < map.Map.Height; y++)
                {
                    foreach (var layer in meleeEnemyLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.MeleeEnemy[x, y] = true;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Load the tech demo map from a .tmx-file.
    /// </summary>
    public void LoadTechDemoMap()
    {
        Maps.Clear();
            
        var filePath = Path.Combine("Content", "Maps", "techdemo.world");
        var jsonContent = File.ReadAllText(filePath);

        var worldData = JsonConvert.DeserializeObject<WorldData>(jsonContent);

        foreach (var map in worldData.Maps)
        {
            var currentMap = _content.Load<TiledMap>(Path.Combine("Maps", map.FileName));
            currentMap.GetLayer<TiledMapTileLayer>("Collision");
            currentMap.GetLayer<TiledMapTileLayer>("Collectable");
            currentMap.GetLayer<TiledMapTileLayer>("rangedEnemies");
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, currentMap);
            var potionLayer = currentMap.GetLayer<TiledMapObjectLayer>("PotionLayer");
            var weaponLayer = currentMap.GetLayer<TiledMapObjectLayer>("Weapon");
                
                
            Maps.Add(new MapData(currentMap, _mapRenderer, new Vector2(map.X, map.Y), new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], false, false));
            if (potionLayer != null)
            {
                foreach (var potion in potionLayer.Objects)
                {
                    var rect = new Rectangle((int)potion.Position.X + map.X, (int)potion.Position.Y + map.Y, (int)potion.Size.Width, (int)potion.Size.Height);
                    PotionRectangles.Add(rect);
                }
            }
                
            if (weaponLayer != null)
            {
                WeaponRectangles.Clear();
                foreach (var weapon in weaponLayer.Objects)
                {
                    var rect = new Rectangle((int)weapon.Position.X + map.X, (int)weapon.Position.Y + map.Y, (int)weapon.Size.Width, (int)weapon.Size.Height);
                    WeaponRectangles.Add(rect);
                }
            }
        }

        // Set collision on first map.
        foreach (var map in Maps)
        {
            map.IsCollisionActive = true;
        }

        foreach (var map in Maps)
        {
            var collisionLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("Collision") && Convert.ToBoolean(tl.Properties["Collision"]));
            var collectableLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("Collectable") && Convert.ToBoolean(tl.Properties["Collectable"]));
            var rangedEnemiesLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("rangedEnemies") && Convert.ToBoolean(tl.Properties["rangedEnemies"]));

            for (ushort x = 0; x < map.Map.Width; x++)
            {
                for (ushort y = 0; y < map.Map.Height; y++)
                {
                    foreach (var layer in collisionLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.Collidable[x, y] = true;
                        }
                    }

                    foreach (var layer in collectableLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.Collectable[x, y] = true;
                        }
                    }
                    
                    foreach (var layer in rangedEnemiesLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.RangedEnemies[x, y] = true;
                        }
                    }
                }
            }
        }
    }
        
    /// <summary>
    /// Load the endboss map from a .tmx-file.
    /// </summary>
    public void LoadEndbossMap()
    {
        Maps.Clear();
            
        var filePath = Path.Combine("Maps", "endboss_map");

        //foreach (var map in worldData.Maps)
        {
            var currentMap = _content.Load<TiledMap>(filePath);
            currentMap.GetLayer<TiledMapTileLayer>("Collision");
            currentMap.GetLayer<TiledMapTileLayer>("rangedEnemies");
            currentMap.GetLayer<TiledMapTileLayer>("Collectable");
            _mapRenderer = new TiledMapRenderer(_graphicsDevice, currentMap);
            var potionLayer = currentMap.GetLayer<TiledMapObjectLayer>("PotionLayer");
            Maps.Add(new MapData(currentMap, _mapRenderer, new Vector2(0, 0), new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], new bool[currentMap.Width, currentMap.Height], false, false));
            if (potionLayer != null)
            {
                foreach (var potion in potionLayer.Objects)
                {
                    var rect = new Rectangle((int)potion.Position.X, (int)potion.Position.Y, (int)potion.Size.Width, (int)potion.Size.Height);
                    PotionRectangles.Add(rect);
                }
            }
        }

        // Set collision on first map.
        foreach (var map in Maps)
        {
            map.IsCollisionActive = true;
        }

        foreach (var map in Maps)
        {
            var collisionLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("Collision") && Convert.ToBoolean(tl.Properties["Collision"]));
            var rangedEnemiesLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("rangedEnemies") && Convert.ToBoolean(tl.Properties["rangedEnemies"]));
            var collectableLayers = map.Map.TileLayers.Where(tl => tl.Properties.ContainsKey("Collectable") && Convert.ToBoolean(tl.Properties["Collectable"]));

            for (ushort x = 0; x < map.Map.Width; x++)
            {
                for (ushort y = 0; y < map.Map.Height; y++)
                {
                    foreach (var layer in collisionLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.Collidable[x, y] = true;
                        }
                    }

                    foreach (var layer in collectableLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.Collectable[x, y] = true;
                        }
                    }
                    
                    foreach (var layer in rangedEnemiesLayers)
                    {
                        if (layer.Width <= x || layer.Height <= y) continue;

                        if (!layer.GetTile(x, y).IsBlank)
                        {
                            map.RangedEnemies[x, y] = true;
                        }
                    }
                }
            }
        }
    }
        

    private bool IsTileCollidable(MapData map, int tileX, int tileY)
    {
        // Out of bounds is treated as non-collidable.
        if (tileX < 0 || tileY < 0 || tileX >= map.Collidable.GetLength(0) || tileY >= map.Collidable.GetLength(1)) return false;

        return map.Collidable[tileX, tileY];
    }
    
    private bool IsTileRangedEnemy(MapData map, int tileX, int tileY)
    {
        // Out of bounds is treated as non-collidable.
        if (tileX < 0 || tileY < 0 || tileX >= map.RangedEnemies.GetLength(0) || tileY >= map.RangedEnemies.GetLength(1)) return false;

        return map.RangedEnemies[tileX, tileY];
    }

    private bool IsTileMeleeEnemy(MapData map, int tileX, int tileY)
    {
        // Out of bounds is treated as non-collidable.
        if (tileX < 0 || tileY < 0 || tileX >= _meleeEnemy.GetLength(0) || tileY >= _meleeEnemy.GetLength(1)) return false;
        return map.MeleeEnemy[tileX, tileY];
    }

    /// <summary>
    /// Compute surrounding tiles on the fly.
    /// </summary>
    public List<(int TileX, int TileY, bool isCollidable)> GetSurroundingTiles(MapData map, Rectangle playerBoundingBox, bool meleeEnemy = false)
    {
        var surroundingTiles = new List<(int, int, bool)>();

        var minX = playerBoundingBox.Left / TileSize;
        var minY = playerBoundingBox.Top / TileSize;
        var maxX = playerBoundingBox.Right / TileSize;
        var maxY = playerBoundingBox.Bottom / TileSize;

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                surroundingTiles.Add(!meleeEnemy
                    ? (x, y, IsTileCollidable(map, x, y))
                    : (x, y, IsTileMeleeEnemy(map, x, y)));
            }
        }

        return surroundingTiles;
    }
    
    /// <summary>
    /// Compute surrounding tiles on the fly.
    /// </summary>
    public List<(int TileX, int TileY, bool isCollidable)> GetSurroundingTilesRangedEnemies(MapData map, Rectangle playerBoundingBox)
    {
        var surroundingTiles = new List<(int, int, bool)>();

        var minX = playerBoundingBox.Left / TileSize;
        var minY = playerBoundingBox.Top / TileSize;
        var maxX = playerBoundingBox.Right / TileSize;
        var maxY = playerBoundingBox.Bottom / TileSize;

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                surroundingTiles.Add((x, y, IsTileRangedEnemy(map, x, y)));
            }
        }

        return surroundingTiles;
    }

    public (int TileX, int TileY, bool isCollidable) GetCurrentTile(MapData map, Rectangle playerBoundingBox)
    {
        var minX = playerBoundingBox.X / TileSize;
        var minY = playerBoundingBox.Y / TileSize;

        return (minX, minY, IsTileCollidable(map, minX, minY));
    }

    /// <summary>
    /// Representation of the .world file.
    /// </summary>
    public class WorldData
    {
        public List<MapInformation> Maps { get; }

        public WorldData(List<MapInformation> maps)
        {
            Maps = maps;
        }

        public class MapInformation(int x, int y, string fileName)
        {
            public string FileName { get; } = fileName;
            public int X { get; } = x;
            public int Y { get; } = y;
        }
    }

    /// <summary>
    /// Retrieve the tiled map tile at the given location on the layer with the given name.
    /// </summary>
    public TiledMapTile GetTile(TiledMap map, string layerName, ushort x, ushort y)
    {
        var layer = map.GetLayer<TiledMapTileLayer>(layerName);
        return layer?.GetTile(x, y) ?? default;
    }

    public void Update(GameTime gameTime)
    {
        foreach (var map in Maps)
        {
            map.MapRenderer?.Update(gameTime);
        }
    }

    /// <summary>
    /// Handles the rendering of the tilemap.
    /// </summary>
    public void Draw(Matrix transformMatrix)
    {
        foreach (var map in Maps)
        {
            var offsetMatrix = Matrix.CreateTranslation(map.Position.X, map.Position.Y, 0);
            if (map.IsDrawActive)
            {
                map.MapRenderer.Draw(offsetMatrix * transformMatrix);
            }
        }
    }
}

/// <summary>
/// Class to store data related to each map.
/// </summary>
public class MapData
{
    public TiledMap Map { get; }
    public TiledMapRenderer MapRenderer { get; }
    public Vector2 Position { get; }
    public bool[,] Collidable { get; }
    public bool[,] Collectable { get; }
    public bool[,] MeleeEnemy { get; }
    public bool[,] RangedEnemies { get; }
    public bool IsDrawActive { get; set; }
    public bool IsCollisionActive { get; set; }
    public Pathfinding.Grid MapGrid { get; set; }

    public MapData(TiledMap map, TiledMapRenderer mapRenderer, Vector2 position, bool[,] collidable, bool[,] collectable, bool[,] meleeEnemy, bool[,] rangedEnemies, bool isDrawActive, bool isCollisionActive)
    {
        Map = map;
        MapRenderer = mapRenderer;
        Position = position;
        Collidable = collidable;
        Collectable = collectable;
        RangedEnemies = rangedEnemies;
        MeleeEnemy = meleeEnemy;
        IsDrawActive = isDrawActive;
        IsCollisionActive = isCollisionActive;
    }
}