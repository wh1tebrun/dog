#region File Description
// RangedEnemy.cs
// Control the ranged enemy.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.Menu;
using Delusions_of_Grandeur.Pathfinding;
using Delusions_of_Grandeur.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Delusions_of_Grandeur.Entities;

/// <summary>
/// Handle RangedEnemy movement and animations.
/// </summary>
public class RangedEnemy : Enemy
{
    // Boolean
    public bool HasAttackSystem { get; set; }
    public bool TechDemo { get; set; }

    // Float
    public float TimeSinceLastShot { get; set; }
    public float ShotCooldown { get; init; } = 1f;
    private const float AttackRange = 700f;

    // Int
    private static int _updateCounter;

    private Player _targetPlayer;

    public Vector2 PrevPosition { get; set; } // For parallelize the collisions detection
    public Vector2 StartPosition { get; set; }

    // Utilities
    public AttackSystem AttackSystem { get; }
    private readonly Manager.AssetManager _assetManager;
    private readonly Manager.MapManager _mapManager;
    public double TimeSinceLastUpdate { get; set; }

    // for debbuging
    private GraphicsDevice _graphicsDevice;
    private Texture2D _whiteTexture;

    /// <summary>
    /// Constructs the ranged enemy.
    /// </summary>
    /// <param name="graphicsDevice"></param>
    /// <param name="startPosition"></param>
    /// <param name="enemyRangedSpeed"></param>
    /// <param name="assetManager"></param>
    /// <param name="animationEnemy2"></param>
    /// <param name="hasAttackSystem"></param>
    /// <param name="collisionDetection"></param>
    /// <param name="animations"></param>
    /// <param name="statistics"></param>
    /// <param name="achievements"></param>
    /// <param name="mapManager"></param>
    /// <param name="hitboxOffset"></param>
    /// <param name="techDemo"></param>
    /// <param name="offensivePlayer"></param>
    /// <param name="defensivePlayer"></param>
    /// <param name="grid"></param>
    public RangedEnemy(GraphicsDevice graphicsDevice, Vector2 startPosition,
        float enemyRangedSpeed,
        Manager.AssetManager assetManager,
        Animation.AnimationPlayer animationEnemy2,
        bool hasAttackSystem,
        CollisionDetection collisionDetection,
        Dictionary<string, Animation.Animation> animations,
        ref List<Statistic> statistics, ref List<Achievement> achievements,
        Player offensivePlayer,
        Player defensivePlayer,
        Grid grid,
        Manager.MapManager mapManager,
        Vector2 hitboxOffset,
        bool techDemo = false)

    : base(graphicsDevice, grid, offensivePlayer, defensivePlayer)
    {
        Position = startPosition;
        StartPosition = startPosition;
        Speed = enemyRangedSpeed;
        GameObjectHealthPoints = 150;

        AnimationPlayer = animationEnemy2;

        var currentAnimation = animations["Idle"];
        AnimationPlayer.PlayAnimation(currentAnimation);

        HitBoxPositionOffset = hitboxOffset;
        CreateHitBox(new Vector2(30, 30));
        CollisionDetection = collisionDetection;
        CollisionDetection.AddGameObjectToCollisionDetection(this);

        GameObjectHitBox.GetHitbox();
        Statistics = statistics;
        Achievements = achievements;

        AttackSystem = new AttackSystem();
        HasAttackSystem = hasAttackSystem;

        _assetManager = assetManager;
        _mapManager = mapManager;

        TechDemo = techDemo;
        TimeSinceLastUpdate = 100;
        MapIndex = (int)(-1 * (Position.Y - 1080) / 1080);

        // for debugging 
        _graphicsDevice = graphicsDevice;
        _whiteTexture = new Texture2D(_graphicsDevice, 1, 1);
        _whiteTexture.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Updates the ranged enemy and handles things like movement, animations based on state etc...
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        const int aggroRange = 1000;
        MapIndex = (int)(-1 * (Position.Y - 1080) / 1080);

        // Only Update the pathfinding if players are in range.
        if (Math.Abs(Position.Y - PlayerDefensive.Position.Y) > aggroRange) return;

        if (MapIndex >= 0 && MapIndex < _mapManager.Maps.Count)
        {
            Grid.Update(_mapManager.Maps[MapIndex], MapIndex);
        }
        else
        {
            Position = StartPosition;
        }

        // Default target
        ChooseTarget();

        if (!_targetPlayer.IsInvisible)
        {
            if (!_targetPlayer.IsInCheckPoint)
            {
                FollowPath(gameTime, _targetPlayer);
            }
        }


        GameObjectHitBox.GetHitbox();

        HitboxPositionUpdate();

        if (HealthPoints <= 0 && HitBoxIsActive)
        {
            _assetManager.GetSound("rangedDeath");
            _assetManager.PlayDeathSoundRangedEnemy("rangedDeath");
            DeactivateHitBox();
            foreach (var projectile in AttackSystem.Projectiles)
            {
                projectile.DeactivateHitBox();
            }
            IncreaseStatisticDefeatedEnemies();
            IncreaseAchievementAttackExpert();
        }

        if (HasAttackSystem)
        {
            HandleAttack(gameTime, _targetPlayer);
        }
    }

    /// <summary>
    /// Handles the attack logic of the ranged enemies.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="target"></param>
    private void HandleAttack(GameTime gameTime, Player target)
    {

        TimeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (TimeSinceLastShot >= ShotCooldown)
        {
            var targetPosition = target.Position;
            targetPosition.X += target.HitBoxPositionOffset.X + target.HitBox.GetHitbox().Width / 2f;
            targetPosition.Y += target.HitBoxPositionOffset.Y + target.HitBox.GetHitbox().Height / 2f;

            if (Vector2.Distance(targetPosition, Position) <= AttackRange && !target.IsInvisible && !target.IsInCheckPoint)
            {
                var projectile = new Projectile(
                    new Vector2(Position.X + HitBoxPositionOffset.X, Position.Y + HitBoxPositionOffset.Y),
                    targetPosition,
                    80,
                    5,
                    5,
                    CollisionDetection,
                    _assetManager,
                    new Vector2(10, 10),
                    ref Statistics,
                    ref Achievements,
                    "Enemy",
                    PlayerDefensive,
                    _mapManager,
                    Weapon.WeaponType.Ranged,
                    TechDemo
                    );
                AttackSystem.AddProjectile(projectile);
                _assetManager.GetSound("gun_sound");
                _assetManager.PlaySoundRangedEnemy("gun_sound");
                TimeSinceLastShot = 0;
            }
        }
        AttackSystem.Update(gameTime);
    }

    private void FollowPath(GameTime gameTime, Player player)
    {
        TimeSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;

        var distanceToPlayer = Vector2.Distance(Position, player.Position);

        if (distanceToPlayer > 300 && TechDemo)
        {
            Vector2 movementTarget = new Vector2(
                player.Position.X + HitBoxPositionOffset.X,
                player.Position.Y + HitBoxPositionOffset.Y
            );

            if (MapIndex == player.MapIndex)
            {
                if (_updateCounter % 1000 == 0 && TimeSinceLastUpdate > 3.0)
                {
                    AStar.Update(
                        new Vector2(Position.X + HitBox.GetHitbox().Width / 2f,
                            Position.Y + HitBox.GetHitbox().Height / 2f + MapIndex * Consts.ScreenHeight),
                        new Vector2(player.Position.X,
                            player.Position.Y + MapIndex * Consts.ScreenHeight)
                    );
                    TimeSinceLastUpdate = 0;
                }

                _updateCounter++;
                if (_updateCounter >= 20000) _updateCounter = 0;

                if (AStar.Path is { Count: > 0 })
                {
                    var node = AStar.Path.First();
                    movementTarget = new Vector2(
                        node.Position.X * Consts.TileSize,
                        node.Position.Y * Consts.TileSize - MapIndex * Consts.ScreenHeight
                    );

                    if (Vector2.Distance(Position, movementTarget) <= 0.5f)
                    {
                        AStar.Path.Remove(node);
                    }
                }
            }

            var direction = Vector2.Normalize(movementTarget - Position);
            PrevPosition = Position;
            Position += direction * 30 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            HitboxPositionUpdate();
            CheckCollision();
        }else if (distanceToPlayer > 150 && !TechDemo)
        {
            Vector2 movementTarget = new Vector2(
                player.Position.X + HitBoxPositionOffset.X,
                player.Position.Y + HitBoxPositionOffset.Y
            );

            if (MapIndex == player.MapIndex)
            {
                if (TimeSinceLastUpdate > 3.0)
                {
                    AStar.Update(
                        new Vector2(Position.X + HitBox.GetHitbox().Width / 2f,
                            Position.Y + HitBox.GetHitbox().Height / 2f + MapIndex * Consts.ScreenHeight),
                        new Vector2(player.Position.X,
                            player.Position.Y + MapIndex * Consts.ScreenHeight)
                    );
                    TimeSinceLastUpdate = 0;
                }

                _updateCounter++;
                if (_updateCounter >= 20000) _updateCounter = 0;

                if (AStar.Path is { Count: > 0 })
                {
                    var node = AStar.Path.First();
                    movementTarget = new Vector2(
                        node.Position.X * Consts.TileSize,
                        node.Position.Y * Consts.TileSize - MapIndex * Consts.ScreenHeight
                    );

                    if (Vector2.Distance(Position, movementTarget) <= 0.5f)
                    {
                        AStar.Path.Remove(node);
                    }
                }
            }

            var direction = Vector2.Normalize(movementTarget - Position);
            PrevPosition = Position;
            Position += direction * 30 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            HitboxPositionUpdate();
            CheckCollision();
        }
    }

    /// <summary>
    /// Draws the ranged enemies onto the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (HasAttackSystem)
        {
            AttackSystem.Draw(spriteBatch);
        }
        AnimationPlayer.Draw(gameTime, spriteBatch, Position, 1f);

        if (GameControl.DebugModeActive)
        {
            AStar.Draw(spriteBatch);
        }
    }

    private void ChooseTarget()
    {
        Player playerWithHighHealth;
        Player playerWithLowHealth;
        if (PlayerDefensive.HealthPoints <= PlayerOffensive.HealthPoints)
        {
            playerWithLowHealth = PlayerDefensive;
            playerWithHighHealth = PlayerOffensive;
        }
        else
        {
            playerWithLowHealth = PlayerOffensive;
            playerWithHighHealth = PlayerDefensive;
        }
        // Decide for a target 
        _targetPlayer = Vector2.Distance(new Vector2(Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f, Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height / 2f),
                            new Vector2(playerWithLowHealth.Position.X + playerWithLowHealth.HitBoxPositionOffset.X + playerWithLowHealth.HitBox.GetHitbox().Width / 2f,
                                playerWithLowHealth.Position.Y + playerWithLowHealth.HitBoxPositionOffset.Y + playerWithLowHealth.HitBox.GetHitbox().Height / 2f)) * 1.5 >
                        Vector2.Distance(new Vector2(Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f, Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height / 2f),
                            new Vector2(playerWithHighHealth.Position.X + playerWithHighHealth.HitBoxPositionOffset.X + playerWithHighHealth.HitBox.GetHitbox().Width / 2f,
                                playerWithHighHealth.Position.Y + playerWithHighHealth.HitBoxPositionOffset.Y + playerWithHighHealth.HitBox.GetHitbox().Height / 2f)) ? playerWithHighHealth : playerWithLowHealth;

    }

    private void CheckCollision()
    {
        if (CollisionDetection.RunCollisionDetectionForSpecificObject(this) is not null)
        {
            Position = PrevPosition;
            HitboxPositionUpdate();
        }
        MapIndex = (int)(-1 * (Position.Y - Consts.ScreenHeight) / Consts.ScreenHeight);
    }

    public void SearchPath()
    {
        MapIndex = (int)(-1 * (Position.Y - Consts.ScreenHeight) / Consts.ScreenHeight);
        if (MapIndex == PlayerDefensive.MapIndex)
        {
            if (MapIndex >= 0 && MapIndex < _mapManager.Maps.Count)
            {
                Grid.Update(_mapManager.Maps[MapIndex], MapIndex);
            }
            else
            {
                Position = StartPosition;
            }

            AStar.Update(
                new Vector2(Position.X + HitBox.GetHitbox().Width / 2f,
                    Position.Y + HitBox.GetHitbox().Height / 2f + MapIndex * Consts.ScreenHeight),
                new Vector2(PlayerDefensive.Position.X,
                    PlayerDefensive.Position.Y + MapIndex * Consts.ScreenHeight)
            );

        }
    }
}
