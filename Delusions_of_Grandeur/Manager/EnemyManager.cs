#region File Description
// EnemyManager.cs
// Manages the enemies.
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Menu;
using Delusions_of_Grandeur.Animation;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Delusions_of_Grandeur.Manager;

public class EnemyManager
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly AssetManager _assetManager;
    private readonly CollisionDetection _collisionDetection;
    private List<Statistic> _statistics;
    private List<Achievement> _achievements;

    public Dictionary<string, Animation.Animation> MeleeEnemyAnimations { get; set; }
    public Dictionary<string, Animation.Animation> RangedEnemyAnimations { get; set; }

    private readonly Player _playerDefensive;
    private readonly Player _playerOffensive;
    private readonly MapManager _mapManager;

    private const int BaseEnemies = 0; 
    private readonly Random _random = new();

    private bool[] _hasSpawnedEnemiesForMap;

    public Pathfinding.Grid Grid { get; private set; }

    public List<MeleeEnemy> MeleeEnemies { get; } = new();
    public List<RangedEnemy> RangedEnemies { get; } = new();

    private Vector2[][] _meleeEnemyPositions = new Vector2[][]
    {
        // map 0 
        new Vector2[] {
            new Vector2(940, 800), 
            new Vector2(1350, 815), 
            new Vector2(1560, 831), 
            new Vector2(1200, 320), 
            new Vector2(400, -80),
            new Vector2(271, -176), 
            new Vector2(663, -208), 
            new Vector2(930, -288), 
            new Vector2(1261, -210), 
            new Vector2(1509, -193), 
            new Vector2(336, -372),
            new Vector2(958, -496),
            new Vector2(1172, -560),
            new Vector2(415, -595),
            new Vector2(99, -629),
            new Vector2(883, -864),
            new Vector2(1140, -928),
            new Vector2(478, -948),
            new Vector2(206, -945),
            new Vector2(1202, -1073),
            new Vector2(900, -1105),
            new Vector2(521,  -1300),
            new Vector2(712, -1360),
            new Vector2(916, -1409),
            new Vector2(1085, -1477),
            new Vector2(1236, -1574),
            new Vector2(1303, -1683),
            new Vector2(910, -1764),
            new Vector2(1035, -1719),
            new Vector2(741, -1842),
            new Vector2(561, -1907),
            new Vector2(403, -1972),
            new Vector2(234, -2051),
            new Vector2(435, -2115),
            new Vector2(57, -2129),
            new Vector2(850, -2151),
            new Vector2(1200,  -2295),
            new Vector2(1300, -2435),
            new Vector2(884, -2626),
            new Vector2(974, -3159),
            new Vector2(1029, -3347),
            new Vector2(780,  -4677),
            new Vector2(1125, -4708),
            new Vector2(1355, -4660),
            new Vector2(413, -4679),
            new Vector2(1152, -4931),
            new Vector2(211, -4934),
            new Vector2(1359, -5027),
            new Vector2(598, -4933),
            new Vector2(831, -5444),
            new Vector2(444, -5719),
            new Vector2(1100, -5667),
            new Vector2(1300, -5783),
            new Vector2(1467, -5893),
            new Vector2(1280, -6117),
            new Vector2(822, -6100),
            new Vector2(915- 100,  -6532),
            new Vector2(129- 100, -6791),
            new Vector2(434- 100, -6854),
            new Vector2(799- 100, -6903),
            new Vector2(1127- 100, -6949),
            new Vector2(1212- 100, -7106),
            new Vector2(1016- 100, -7171),
            new Vector2(661- 100, -7236),
            new Vector2(441- 100, -7363),
            new Vector2(188- 100, -7379),
            new Vector2(244- 100, -7236),
            new Vector2(341- 100, -7586),
            new Vector2(1035- 100, -7606),
            new Vector2(1312- 100, -7605),
            new Vector2(661- 100, -7666),
            new Vector2(1235- 100, -7830),
            new Vector2(307- 100, -7811),
            new Vector2(1197- 100, -8182),
            new Vector2(1261- 100, -8311),
            new Vector2(1400- 100, -8523),
            new Vector2(394- 100, -8708),
            new Vector2(499- 100, -8820),
            new Vector2(222,  -9987),
            new Vector2(597, -10101),
            new Vector2(1000, -10022),
            new Vector2(1290, -10021),
            new Vector2(1081, -10213),
            new Vector2(570, -10341),
            new Vector2(1021, -10439),
            new Vector2(538, -10706),
            new Vector2(966, -10919),
            new Vector2(1171, -10871),
            new Vector2(634, -10885),
            new Vector2(425, -10930),
            new Vector2(125, -10947),
            new Vector2(801,  -11617),
            new Vector2(1415, -11457),
            new Vector2(1080, -11591),
            new Vector2(900, -11735),
            new Vector2(300, -11650),
            new Vector2(829, -12083),
            new Vector2(1168, -11926),
            new Vector2(1241, -12071),
            new Vector2(862, -12243),
            new Vector2(632, -12244),
            new Vector2(360, -12388),
            new Vector2(120, -12209),
            new Vector2(694, -12771),
            new Vector2(1084, -12674),
            new Vector2(863, -13011),
            new Vector2(462, -12915),
            
                
        }
            
    };

        
    public static readonly (int Bottom, int Top)[] MapYRanges = new (int Bottom, int Top)[]
    {
        // Map 0
        (1088, 0),
        // Map 1 
        (0, -1088),
        // Map 2
        (-1088, -1088 * 2),
        // Map 2
            
        // Map 4
        (-1088 * 4 - 1088/2, -1088 * 5),
        // Map 5
        (-1088 * 5, -1088 * 6),
        // Map 7
        (-1088 * 6, -1088 * 7),
        // Map 8
            
        // Map 9
        (-1088 * 8 - 1088/2, -1088 * 9),
        // Map 10
        (-1088 * 9, -1088 * 10),
        // Map 11
        (-1088 * 10, -1088 * 11),
        // Map 12
        (-1088 * 11, -1088 * 12)
    };

    public EnemyManager(
        GraphicsDevice graphicsDevice,
        AssetManager assetManager,
        CollisionDetection collisionDetection,
        ref List<Statistic> statistics,
        ref List<Achievement> achievements,
        Dictionary<string, Animation.Animation> meleeEnemyAnimations,
        Dictionary<string, Animation.Animation> rangedEnemyAnimations,
        Pathfinding.Grid grid,
        Player playerDefensive,
        Player playerOffensive,
        MapManager mapManager
    )
    {
        _graphicsDevice = graphicsDevice;
        _assetManager = assetManager;
        _collisionDetection = collisionDetection;
        _statistics = statistics;
        _achievements = achievements;
        MeleeEnemyAnimations = meleeEnemyAnimations;
        RangedEnemyAnimations = rangedEnemyAnimations;
        _playerDefensive = playerDefensive;
        _playerOffensive = playerOffensive;
        _mapManager = mapManager;
        Grid = grid;
        _hasSpawnedEnemiesForMap = new bool[_mapManager.Maps.Count];

            
        for (int i = 0; i < _mapManager.Maps.Count; i++)
        {
            SpawnEnemiesForMap(_mapManager.Maps[i]);
            _hasSpawnedEnemiesForMap[i] = true;
        }
    }

    private int GetMaxEnemiesForMap(int mapIndex)
    {
        return BaseEnemies + (3 * mapIndex);
        //return 0;
    }

    private void SpawnEnemiesForMap(MapData map)
    {
        int mapIndex = _mapManager.Maps.IndexOf(map);
        if (mapIndex < 0) return;

        // Melee enemies
        if (mapIndex < _meleeEnemyPositions.Length)
        {
            var positions = _meleeEnemyPositions[mapIndex];
            foreach (var pos in positions)
            {
                var meleeEnemy = new MeleeEnemy(
                    pos,
                    30f,
                    new AnimationPlayer(),
                    _collisionDetection,
                    MeleeEnemyAnimations,
                    ref _statistics,
                    ref _achievements,
                    _mapManager,
                    _playerDefensive,
                    _playerOffensive,
                    _graphicsDevice,
                    _assetManager
                );
                MeleeEnemies.Add(meleeEnemy);
            }
        }

        // Ranged enemies
        var maxEnemies = GetMaxEnemiesForMap(mapIndex);
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnRandomRangedEnemyInMap(map);
        }
    }

    private void SpawnRandomRangedEnemyInMap(MapData map)
    {
        const int maxSpawnTryCount = 50;
        int tryCount = 0;
        bool foundSafeSpot = false;
        RangedEnemy rangedEnemy = null;

        int mapIndex = _mapManager.Maps.IndexOf(map);
        if (mapIndex == 4 || mapIndex == 9)
        {
            // Checkpoint maps
            return;
        }
        if (mapIndex < 0 || mapIndex >= MapYRanges.Length)
        {
            return;
        }

        var mapPixelWidth = map.Map.Width * map.Map.TileWidth;
        int safeLeft = (int)map.Position.X;
        int safeRight = (int)(map.Position.X + mapPixelWidth);

        var yMin = MapYRanges[mapIndex].Bottom;
        var yMax = MapYRanges[mapIndex].Top;

        while (tryCount < maxSpawnTryCount)
        {
            float x = _random.Next(safeLeft, safeRight);

            var lowY = Math.Min(yMin, yMax);
            var highY = Math.Max(yMin, yMax);
            float y = _random.Next(lowY, highY);

            rangedEnemy = new RangedEnemy(
                _graphicsDevice,
                new Vector2(x, y),
                50f,
                _assetManager,
                new AnimationPlayer(),
                true,
                _collisionDetection,
                RangedEnemyAnimations,
                ref _statistics,
                ref _achievements,
                _playerOffensive,
                _playerDefensive,
                Grid,
                _mapManager,
                new Vector2(15, 26)
            );

                
            if (!IsEnemyCollidingWithEnvironment(rangedEnemy))
            {
                foundSafeSpot = true;
                break;
            }

                
            _collisionDetection.DeleteGameObjectFromCollisionDetection(rangedEnemy);
            tryCount++;
        }

        if (foundSafeSpot && rangedEnemy != null)
        {
            RangedEnemies.Add(rangedEnemy);
        }
    }

    private void CheckAndSpawnNextMaps()
    {
        var playerAverageY = (_playerDefensive.Position.Y + _playerOffensive.Position.Y) / 2f;

        for (int i = 0; i < _mapManager.Maps.Count - 1; i++)
        {
            if (_hasSpawnedEnemiesForMap[i] && !_hasSpawnedEnemiesForMap[i + 1])
            {
                var currentMap = _mapManager.Maps[i];
                var nextMap = _mapManager.Maps[i + 1];

                var currentMapBottom = currentMap.Position.Y + currentMap.Map.Height * currentMap.Map.TileHeight;
                var threshold = currentMapBottom - 300f;

                if (!(playerAverageY < threshold)) continue;
                    
                _mapManager.Maps[i].IsCollisionActive = false;
                _mapManager.Maps[i + 1].IsCollisionActive = true;

                SpawnEnemiesForMap(nextMap);
                _hasSpawnedEnemiesForMap[i + 1] = true;
            }
        }
    }

    private bool IsEnemyCollidingWithEnvironment(Enemy enemy)
    {
        var enemyHitBox = enemy.HitBox.GetHitbox();
        foreach (var map in _mapManager.Maps)
        {
            if (!map.IsCollisionActive) continue;

            var surroundingTiles = _mapManager.GetSurroundingTilesRangedEnemies(map, enemyHitBox);
            foreach (var (_, _, canCollide) in surroundingTiles)
            {
                if (canCollide)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public void ClearMeleeEnemies()
    {
        foreach (var enemy in MeleeEnemies)
        {
            _collisionDetection.DeleteGameObjectFromCollisionDetection(enemy);
        }
        MeleeEnemies.Clear();
    }

    public void ClearRangedEnemies()
    {
        foreach (var enemy in RangedEnemies)
        {
            _collisionDetection.DeleteGameObjectFromCollisionDetection(enemy);
        }
        RangedEnemies.Clear();
    }
    
    public void Update(GameTime gameTime)
    {
        CheckAndSpawnNextMaps();

        // Melee update
        for (var i = MeleeEnemies.Count - 1; i >= 0; i--)
        {
            MeleeEnemies[i].Update(gameTime);
        }

            
        for (var i = RangedEnemies.Count - 1; i >= 0; i--)
        {
            RangedEnemies[i].Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        foreach (var meleeEnemy in MeleeEnemies)
        {
            meleeEnemy.Draw(gameTime, spriteBatch);
        }

        foreach (var rangedEnemy in RangedEnemies)
        {
            rangedEnemy.Draw(gameTime, spriteBatch);
        }
    }
}