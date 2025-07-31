using System;
using System.Collections.Generic;
using System.Linq;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Entities;

/// <summary>
/// A class for one projectile.
/// </summary>
public class Projectile : GameObject
{
    public Vector2 Direction { get; set; }

    public int Speed { get; }

    public int Damage { get; private set; }

    public float Lifespan { get; private set; }

    //public string OwnerId { get; }

    public Vector2 TargetPosition { get; }

    private int _hitOffensiveCooldown = 10;
    private int _hitDefensiveCooldown = 10;

    private readonly Player _defensivePlayer;

    private readonly bool _techDemo;
    private bool _detectedByShield = false;

    public static bool HitOffensive;
    public static bool HitDefensive;

    private float angleInRadians;
    private float angleInDegrees;

    //private Effect _impactEffect;

    private AssetManager _assetManager;

    private readonly Texture2D _bulletPlayerOffensive;
    private readonly Texture2D _bulletRangedEnemy;
    private readonly Texture2D _bulletEndBoss;

    private readonly MapManager _mapManager;

    public Weapon.WeaponType Type { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="speed"></param>
    /// <param name="damage"></param>
    /// <param name="lifespan"></param>
    /// <param name="collisionDetection"></param>
    /// <param name="assetManager"></param>
    /// <param name="size"></param>
    /// <param name="statistics"></param>
    /// <param name="achievements"></param>
    /// <param name="ownerId"></param>
    /// // <param name="techDemo (is false per default)"></param>
    /// <param name="defensivePlayer"></param>
    /// <param name="mapManager"></param>
    /// <param name="projectileType"></param>
    /// <param name="techDemo"></param>
    public Projectile(
        Vector2 startPosition,
        Vector2 targetPosition,
        int speed, int damage,
        int lifespan,
        CollisionDetection collisionDetection,
        AssetManager assetManager,
        Vector2 size,
        ref List<Statistic> statistics,
        ref List<Achievement> achievements,
        string ownerId,
        Player defensivePlayer,
        MapManager mapManager,
        Weapon.WeaponType projectileType = Weapon.WeaponType.Ranged,
        bool techDemo = false
        )
    {
        TargetPosition = targetPosition;
        Position = startPosition;
        Direction = targetPosition - Position;
        if (Direction != Vector2.Zero)
        {
            Direction = Vector2.Normalize(Direction);
        }
        Speed = speed;
        Damage = damage;
        Lifespan = lifespan;
        _assetManager = assetManager;
        _bulletPlayerOffensive = _assetManager.GetTexture("01");
        _bulletRangedEnemy = _assetManager.GetTexture("24");
        _bulletEndBoss = _assetManager.GetTexture("14");
        if ((ownerId == "Player1" || ownerId == "Player2") && projectileType == Weapon.WeaponType.Ranged)
        {
            angleInRadians = (float)Math.Atan2(Direction.Y, Direction.X);
            angleInDegrees = MathHelper.ToDegrees(angleInRadians);

            switch (angleInDegrees)
            {
                case <= 0 and > -15:
                    HitBoxPositionOffset = new Vector2(25, 20);
                    break;
                case <= -15 and > -30:
                    HitBoxPositionOffset = new Vector2(40, 10);
                    break;
                case <= -30 and > -45:
                    HitBoxPositionOffset = new Vector2(40, 0);
                    break;
                case <= -45 and > -60:
                    HitBoxPositionOffset = new Vector2(37, -20);
                    break;
                case <= -60 and > -75:
                    HitBoxPositionOffset = new Vector2(40, -35);
                    break;
                case <= -75 and > -90:
                    HitBoxPositionOffset = new Vector2(25, -40);
                    break;
                case <= -90 and > -105:
                    HitBoxPositionOffset = new Vector2(15, -40);
                    break;
                case <= -105 and > -120:
                    HitBoxPositionOffset = new Vector2(13, -40);
                    break;
                case <= -120 and > -135:
                    HitBoxPositionOffset = new Vector2(10, -35);
                    break;
                case <= -135 and > -150:
                    HitBoxPositionOffset = new Vector2(-10, -45);
                    break;
                case <= -150 and > -165:
                    HitBoxPositionOffset = new Vector2(-15, -40);
                    break;
                case <= -165 and > -180:
                    HitBoxPositionOffset = new Vector2(-30, -40);
                    break;
                case <= 15 and > 0:
                    HitBoxPositionOffset = new Vector2(25, 28);
                    break;
                case <= 30 and > 15:
                    HitBoxPositionOffset = new Vector2(20, 30);
                    break;
                case <= 45 and > 30:
                    HitBoxPositionOffset = new Vector2(17, 45);
                    break;
                case <= 60 and > 45:
                    HitBoxPositionOffset = new Vector2(7, 60);
                    break;
                case <= 75 and > 60:
                    HitBoxPositionOffset = new Vector2(0, 65);
                    break;
                case <= 90 and > 75:
                    HitBoxPositionOffset = new Vector2(-35, 12);
                    break;
                case <= 105 and > 90:
                    HitBoxPositionOffset = new Vector2(-47, 10);
                    break;
                case <= 120 and > 105:
                    HitBoxPositionOffset = new Vector2(-50, 10);
                    break;
                case <= 135 and > 120:
                    HitBoxPositionOffset = new Vector2(-60, 10);
                    break;
                case <= 150 and > 135:
                    HitBoxPositionOffset = new Vector2(-70, 10);
                    break;
                case <= 165 and > 150:
                    HitBoxPositionOffset = new Vector2(-65, -20);
                    break;
                case <= 180 and > 165:
                    HitBoxPositionOffset = new Vector2(-65, -25);
                    break;
                default:
                    HitBoxPositionOffset = new Vector2(0, 0);
                    break;

            }
        }
        else
        {
            HitBoxPositionOffset = Vector2.Zero;
        }
        CreateHitBox(size);
        CollisionDetection = collisionDetection;
        CollisionDetection.AddGameObjectToCollisionDetection(this);
        Statistics = statistics;
        Achievements = achievements;
        OwnerId = ownerId;
        _defensivePlayer = defensivePlayer;
        _mapManager = mapManager;
        Type = projectileType;
        _techDemo = techDemo;
    }

    /// <summary>
    /// Update the projectile lifespan, and position based on the direction and the speed of the projectile.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        angleInRadians = (float)Math.Atan2(Direction.Y, Direction.X);
        angleInDegrees = MathHelper.ToDegrees(angleInRadians);

        Position += Direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Type == Weapon.WeaponType.Ranged)
        {
            for (int mapCount = 0; mapCount < _mapManager.Maps.Count; mapCount++)
            {
                MapData currentMap = _mapManager.Maps[mapCount];
                if (!currentMap.IsDrawActive) continue;

                var hitBox = HitBox.GetHitbox();
                var currentTile = _mapManager.GetCurrentTile(currentMap,
                    new Rectangle(hitBox.X, hitBox.Y + mapCount * currentMap.Map.Height * 16, hitBox.Width,
                        hitBox.Height));

                var (_, _, canCollide) = currentTile;

                if (!canCollide) continue;

                Lifespan = 0;

                // _impactEffect = _assetManager.GetEffect("BulletHit");
                // _impactEffect.Parameters["impactPosition"].SetValue(hitBox.Center.ToVector2());
            }
        }

        GameObjectHitBox.UpdatePosition(Position + HitBoxPositionOffset);
        ReduceLifespan(gameTime);

        if (_defensivePlayer.Shield.CollidesWithProjectile(Position, Type) &&
            (OwnerId == "Enemy" || OwnerId == "EndBossRange" || OwnerId == "EndBossMelee") && !_detectedByShield)
        {
            _detectedByShield = true;
            int damageBeforeShield = Damage;

            float blockedDamage = _defensivePlayer.Shield.SetDamageToProjectile();
            if (blockedDamage == 0)
            {
                // All damage is blocked.
                SetLifespan(0);

                if (!_techDemo)
                {
                    foreach (Statistic statistic in Statistics)
                    {
                        if (statistic.Name == "Blocked damage")
                        {
                            statistic.Value += Damage;
                        }
                    }

                    foreach (Achievement achievement in Achievements)
                    {
                        if (achievement.Name == "Master of defense")
                        {
                            achievement.IncreaseValue(Damage);
                        }
                    }
                }
            }
            else
            {
                Damage = (int)(Damage * blockedDamage);

                if (!_techDemo)
                {
                    foreach (var statistic in Statistics.Where(statistic => statistic.Name == "Blocked damage"))
                    {
                        if (damageBeforeShield != Damage)
                        {
                            statistic.Value += damageBeforeShield - Damage;
                        }
                        else
                        {
                            statistic.Value += Damage;
                        }
                    }

                    foreach (var achievement in Achievements.Where(achievement =>
                                 achievement.Name == "Master of defense"))
                    {
                        if (damageBeforeShield != Damage)
                        {
                            achievement.IncreaseValue(damageBeforeShield - Damage);
                        }
                        else
                        {
                            achievement.IncreaseValue(Damage);
                        }
                    }
                }
            }
        }
        
        if (Lifespan > 0)
        {
            GameObject hitObject = CollisionDetection.RunCollisionDetectionForSpecificObject(this);
            if (hitObject is not null)
            {
                if ((hitObject is Enemy && OwnerId == "Player1") ||
                    hitObject is Player && OwnerId is "Enemy" or "EndBossRange" or "EndBossMelee")
                {
                    // Player does not get damage in tech demo.
                    if (_techDemo && OwnerId == "Enemy")
                    {
                        Damage = 0;
                    }
                    HitOffensive = true;

                    //hitObject.HealthPoints -= Damage;
                    hitObject.DealDamage(Damage);
                    SetLifespan(0);
                }
                if ((hitObject is Enemy && OwnerId == "Player2") ||
                    hitObject is Player && OwnerId is "Enemy" or "EndBossRange" or "EndBossMelee")
                {
                    // Player does not get damage in tech demo.
                    if (_techDemo && OwnerId == "Enemy")
                    {
                        Damage = 0;
                    }
                    HitDefensive = true;

                    //hitObject.HealthPoints -= Damage;
                    hitObject.DealDamage(Damage);
                    SetLifespan(0);
                }

                if (hitObject is Enemy && OwnerId is "Player1" or "Player2" && !_techDemo)
                {
                    foreach (Statistic statistic in Statistics)
                    {
                        if (statistic.Name == "Damaged dealt")
                        {
                            statistic.Value += Damage;
                        }
                    }
                }
                if (hitObject is Player && OwnerId is "Enemy" or "EndBossRange" or "EndBossMelee" && !_techDemo)
                {
                    foreach (Statistic statistic in Statistics)
                    {
                        if (statistic.Name == "Damage endured")
                        {
                            statistic.Value += Damage;
                        }
                    }
                    foreach (Achievement achievement in Achievements)
                    {
                        if (achievement.Name == "Endurance")
                        {
                            achievement.IncreaseValue(Damage);
                        }
                    }
                }
            }

            _hitOffensiveCooldown--;
            if (_hitOffensiveCooldown == 0)
            {
                _hitOffensiveCooldown = 10;
                HitOffensive = false;
            }

            _hitDefensiveCooldown--;

            if (_hitDefensiveCooldown != 0) return;

            _hitDefensiveCooldown = 10;
            HitDefensive = false;
        }
    }

    /// <summary>
    /// Draws a projectile at his position. At this moment the projectile is just a white rectangle.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (Type == Weapon.WeaponType.Melee) return;

        const int size = 20;

        if (Lifespan == 0)
        {

        }

        switch (OwnerId)
        {
            case "Player1":
            case "Player2":
                spriteBatch.Draw(_assetManager.GetTexture("diamond_arrow"), Position, new Rectangle(0, 0, 64, 64), Color.White, angleInRadians, Vector2.Zero, 1f, SpriteEffects.None, 0);
                /*
                spriteBatch.Draw(
                    _bulletPlayerOffensive,
                    new Rectangle((int)Position.X, (int)Position.Y, size, size),
                    Color.White
                );
                */
                break;
            case "Enemy":
                // Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
                spriteBatch.Draw(
                    _bulletRangedEnemy,
                    new Rectangle((int)Position.X, (int)Position.Y, size, size),
                    Color.White
                );
                break;
            case "EndBossRange":
                spriteBatch.Draw(
                    _bulletEndBoss,
                    new Rectangle((int)Position.X, (int)Position.Y, size, size),
                    Color.White
                );
                break;
            case "EndBossMelee":
                /*
                    spriteBatch.Draw(
                        _bulletTexture4,
                        new Rectangle((int)Position.X, (int)Position.Y, size, size),
                        Color.White
                    );
                    */
                break;
        }
    }

    /// <summary>
    /// Reduces the Lifespan by one point. If the Lifespan is under or equal to 0 than the projectile is deleted from the collision detection.
    /// </summary>
    private void ReduceLifespan(GameTime gameTime)
    {
        Lifespan -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (Lifespan <= 0)
        {
            CollisionDetection.DeleteGameObjectFromCollisionDetection(this);
        }
    }

    /// <summary>
    /// Set the Lifespan to the given value. If the Lifespan is under or equal to 0 than the projectile is deleted from the collision detection.
    /// </summary>
    /// <param name="lifespan"></param>
    private void SetLifespan(int lifespan)
    {
        Lifespan = lifespan;
        if (Lifespan <= 0)
        {
            DeactivateHitBox();
        }
    }

    public void DeleteProjectile()
    {
        CollisionDetection.DeleteGameObjectFromCollisionDetection(this);
    }
}
