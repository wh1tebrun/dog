using System;
using System.Linq;
using System.Collections.Generic;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Menu;
using Delusions_of_Grandeur.Pathfinding;
using Delusions_of_Grandeur.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Delusions_of_Grandeur.Entities;

public class EndBoss : Enemy
{
    public double TimeSinceLastUpdate {get; set;}
    public float TimeSinceLastAttack { get; set; }
    public float TimeSinceLastImmune { get; set; }
    public float TimeSinceLastHealing { get; set; }
    public float TimeSinceLastHit { get; set; }
    public float EbHealingDuration { get; set; }
    public float EbImmuneDuration { get; set; }
    public float TimeSinceLastGlobalSkill { get; set; }
    public float TimeSinceDamageBuffActivation { get; set; }
    public float TimeSinceDeath { get; set; }
    
    // Integer
    private readonly int _maxHealthPoints;
    public int HealingValue { get; set; } // Healing per second


    // Boolean
    public bool IsHealing { get; set; }
    public bool IsImmune { get; set; }
    public bool GlobalSkillIsActive { get; set; }
    public bool DamageBuffIsActive { get; set; }
    public bool DamageBuffAnimationRunning { get; set; }
    public bool RangeAttackActive { get; set; }
    public bool MeleeAttackActive { get; set; }
    public bool FlipAnimation { get; set; }
    public bool EbIsAlive { get; set; }
    public bool DeathAnimationRunning { get; set; }

    // Vector2
    private readonly Vector2 _offsetProjectileRangeAttack = new Vector2(100, 0);
    private readonly Vector2 _offsetProjectileMeleeAttack = new Vector2(50, 20);
    private readonly Vector2 _hitBoxSizeMeleeProjectile = new Vector2(30, 60);

    // Projectile
    private Projectile _nextProjectile;
    
    // Utilities
    public AttackSystem AttackSystem { get; }
    private readonly AssetManager _assetManager;
    private readonly HealthBar _healthBar;
    private readonly Random _randomNumberGenerator;
    private readonly MapManager _mapManager;
    private Player _targetPlayer;

    /// <summary>
    /// Constructs the final boss.
    /// </summary>
    /// <param name="graphicsDevice"></param>
    /// <param name="startPosition"></param>
    /// <param name="animationEndBoss"></param>
    /// <param name="collisionDetection"></param>
    /// <param name="animations"></param>
    /// <param name="statistics"></param>
    /// <param name="achievements"></param>
    /// <param name="playerDefensive"></param>
    /// <param name="playerOffensive"></param>
    /// <param name="healthBar"></param>
    /// <param name="assetManager"></param>
    /// <param name="grid"></param>
    /// <param name="mapManager"></param>
    /// <param name="maxHealthPoints"></param>
    public EndBoss(GraphicsDevice graphicsDevice, Vector2 startPosition, Animation.AnimationPlayer animationEndBoss, CollisionDetection collisionDetection,
        Dictionary<string, Animation.Animation> animations, ref List<Statistic> statistics, ref List<Achievement> achievements,
        Player playerDefensive, Player playerOffensive, AssetManager assetManager, Grid grid, MapManager mapManager, int maxHealthPoints, HealthBar healthBar)
    : base(graphicsDevice, grid, playerOffensive, playerDefensive)
    {
        Position = startPosition;
        _maxHealthPoints = maxHealthPoints;
        GameObjectHealthPoints = maxHealthPoints;

        HitBoxPositionOffset = new Vector2(100, 90);
        CreateHitBox(new Vector2(50, 75));
        CollisionDetection = collisionDetection;
        CollisionDetection.AddGameObjectToCollisionDetection(this);

        Statistics = statistics;
        Achievements = achievements;

        PlayerDefensive = playerDefensive;
        PlayerOffensive = playerOffensive;

        AttackSystem = new AttackSystem();

        AnimationPlayer = animationEndBoss;
        Animations = animations;

        _mapManager = mapManager;

        FlipAnimation = PlayerOffensive.Position.X + PlayerOffensive.HitBoxPositionOffset.X +
                        PlayerOffensive.HitBox.GetHitbox().Width / 2f <=
                        Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f;

        var animation = Animations["Idle"];
        animation.IsFlipped = FlipAnimation;
        AnimationPlayer.PlayAnimation(animation);

        TimeSinceLastHealing = 0f;
        EbHealingDuration = 0f;
        TimeSinceLastHit = 0f;
        IsHealing = false;

        TimeSinceLastImmune = 0f;
        EbImmuneDuration = 0f;
        IsImmune = false;

        TimeSinceLastAttack = 0f;
        RangeAttackActive = false;
        GlobalSkillIsActive = false;

        TimeSinceDamageBuffActivation = 0f;
        DamageBuffAnimationRunning = false;
        DamageBuffIsActive = false;

        _randomNumberGenerator = new Random();

        _assetManager = assetManager;
        

        FlipAnimation = false;

        EbIsAlive = true;
        DeathAnimationRunning = false;

        _healthBar = healthBar;
        Grid.Update(_mapManager.Maps[0], 0);
    }

    /// <summary>
    /// Updates the final boss and handles things like healing, attack, etc...
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        if (!EbIsAlive) return;

        UpdateTimers(gameTime);
        HandleDeath();
        HandleDamageBuff();
        ChooseTarget();
        HandleAttack();
        HandleHealing();
        HandleImmune();
        HitboxPositionUpdate();
        if (HealthPoints <= 0 && HitBoxIsActive)
        {
            DeactivateHitBox();
            foreach (var projectile in AttackSystem.Projectiles)
            {
                projectile.DeactivateHitBox();
            }

            IncreaseStatisticDefeatedEnemies();
            IncreaseAchievementAttackExpert();
        }

        DeactivateHealing();
        DeactivateImmune();
        DeactivateDamageBuffAnimation();
        DeactivateRangedAttackAnimation();
        DeactivateMeleeAttackAnimation();
        DeleteEndBoss();
        AttackSystem.Update(gameTime);
        _healthBar.Update(HealthPoints);

        if (!_targetPlayer.IsInvisible)
        {
            FollowPath(gameTime, _targetPlayer);
        }
    }

    /// <summary>
    /// Draw the final boss and his attack system
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (!EbIsAlive) return;

        AnimationPlayer.Draw(gameTime, spriteBatch, Position, 1f);
        AttackSystem.Draw(spriteBatch);
        //AStar.Draw(spriteBatch);
        //Grid.Draw(spriteBatch);
    }

    /// <summary>
    /// Updates all timers of the actions the final boss can do.
    /// </summary>
    /// <param name="gameTime"></param>
    private void UpdateTimers(GameTime gameTime)
    {
        TimeSinceLastHealing += (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeSinceLastImmune += (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeSinceLastAttack += (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeSinceLastGlobalSkill += (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeSinceLastHit += (float)gameTime.ElapsedGameTime.TotalSeconds;
        TimeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (IsHealing)
        {
            EbHealingDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (IsImmune)
        {
            EbImmuneDuration += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (DamageBuffAnimationRunning)
        {
            TimeSinceDamageBuffActivation += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        if (DeathAnimationRunning)
        {
            TimeSinceDeath += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    /// <summary>
    /// Handle all normal attacks. Decides whether the boss makes a melee attack or a ranged attack. 
    /// </summary>
    private void HandleAttack()
    {
        const int maxMeleeRange = 113;

        if (GlobalSkillIsActive || (TimeSinceLastAttack < Consts.AttackCooldown) || _targetPlayer.IsInvisible) return;
        // Decides if animation needs to be flipped
        FlipAnimation = _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                        _targetPlayer.HitBox.GetHitbox().Width / 2f <=
                        Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f;

        // Decide if the attack is a melee or range attack
        if (Vector2.Distance(new Vector2(Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f, Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height / 2f),
                new Vector2(_targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X + _targetPlayer.HitBox.GetHitbox().Width / 2f, _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y + _targetPlayer.HitBox.GetHitbox().Height / 2f)) > maxMeleeRange)
        {
            if (!DamageBuffIsActive)
            {
                if (!FlipAnimation)
                {

                    _nextProjectile = new Projectile(
                        new Vector2(Position.X + HitBoxPositionOffset.X + _offsetProjectileRangeAttack.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileRangeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        160,
                        Consts.RangeDamage,
                        10,
                        CollisionDetection,
                        _assetManager,
                        new Vector2(10, 10),
                        ref Statistics,
                        ref Achievements,
                        "EndBossRange",
                        PlayerDefensive,
                        _mapManager
                    );

                }
                else
                {
                    _nextProjectile = new Projectile(
                        new Vector2(Position.X + HitBoxPositionOffset.X - _offsetProjectileRangeAttack.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileRangeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        160,
                        Consts.RangeDamage,
                        10,
                        CollisionDetection,
                        _assetManager,
                        new Vector2(10, 10),
                        ref Statistics,
                        ref Achievements,
                        "EndBossRange",
                        PlayerDefensive,
                        _mapManager
                    );
                }
            }
            else
            {
                if (!FlipAnimation)
                {
                    _nextProjectile = new Projectile(
                        new Vector2(Position.X + HitBoxPositionOffset.X + _offsetProjectileRangeAttack.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileRangeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        80 * Consts.DamageBuff,
                        Consts.RangeDamage * Consts.DamageBuff,
                        10,
                        CollisionDetection,
                        _assetManager,
                        new Vector2(10, 10),
                        ref Statistics,
                        ref Achievements,
                        "EndBossRange",
                        PlayerDefensive,
                        _mapManager
                    );
                }
                else
                {
                    _nextProjectile = new Projectile(
                        new Vector2(Position.X + HitBoxPositionOffset.X - _offsetProjectileRangeAttack.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileRangeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        80 * Consts.DamageBuff,
                        Consts.RangeDamage * Consts.DamageBuff,
                        10,
                        CollisionDetection,
                        _assetManager,
                        new Vector2(10, 10),
                        ref Statistics,
                        ref Achievements,
                        "EndBossRange",
                        PlayerDefensive,
                        _mapManager
                    );
                }
            }


            TimeSinceLastAttack = 0f;
            RangeAttackActive = true;
            GlobalSkillIsActive = true;

            var animation = Animations["RangeAttack"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
            _assetManager.PlaySound("boss_range");
        }
        else // Melee Attack
        {


            if (!DamageBuffIsActive)
            {
                if (!FlipAnimation)
                {
                    _nextProjectile = new Projectile(
                        new Vector2(
                            Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width +
                            _offsetProjectileMeleeAttack.X - _hitBoxSizeMeleeProjectile.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileMeleeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        0,
                        Consts.MeleeDamage,
                        1,
                        CollisionDetection,
                        _assetManager,
                        _hitBoxSizeMeleeProjectile,
                        ref Statistics,
                        ref Achievements,
                        "EndBossMelee",
                        PlayerDefensive,
                        _mapManager,
                        Weapon.WeaponType.Melee
                    );
                }
                else
                {
                    _nextProjectile = new Projectile(
                        new Vector2(Position.X + HitBoxPositionOffset.X - _offsetProjectileMeleeAttack.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileMeleeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        0,
                        Consts.MeleeDamage,
                        1,
                        CollisionDetection,
                        _assetManager,
                        _hitBoxSizeMeleeProjectile,
                        ref Statistics,
                        ref Achievements,
                        "EndBossMelee",
                        PlayerDefensive,
                        _mapManager,
                        Weapon.WeaponType.Melee
                    );
                }
            }
            else
            {
                if (!FlipAnimation)
                {
                    _nextProjectile = new Projectile(
                        new Vector2(
                            Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width +
                            _offsetProjectileMeleeAttack.X - _hitBoxSizeMeleeProjectile.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileMeleeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        0,
                        Consts.MeleeDamage * Consts.DamageBuff,
                        1,
                        CollisionDetection,
                        _assetManager,
                        _hitBoxSizeMeleeProjectile,
                        ref Statistics,
                        ref Achievements,
                        "EndBossMelee",
                        PlayerDefensive,
                        _mapManager,
                        Weapon.WeaponType.Melee
                    );
                }
                else
                {
                    _nextProjectile = new Projectile(
                        new Vector2(Position.X + HitBoxPositionOffset.X - _offsetProjectileMeleeAttack.X,
                            Position.Y + HitBoxPositionOffset.Y + _offsetProjectileMeleeAttack.Y),
                        new Vector2(
                            _targetPlayer.Position.X + _targetPlayer.HitBoxPositionOffset.X +
                            _targetPlayer.HitBox.GetHitbox().Width / 2f,
                            _targetPlayer.Position.Y + _targetPlayer.HitBoxPositionOffset.Y +
                            _targetPlayer.HitBox.GetHitbox().Height / 2f),
                        0,
                        Consts.MeleeDamage * Consts.DamageBuff,
                        1,
                        CollisionDetection,
                        _assetManager,
                        _hitBoxSizeMeleeProjectile,
                        ref Statistics,
                        ref Achievements,
                        "EndBossMelee",
                        PlayerDefensive,
                        _mapManager,
                        Weapon.WeaponType.Melee
                    );
                }
            }

            TimeSinceLastAttack = 0f;
            MeleeAttackActive = true;
            GlobalSkillIsActive = true;
            if (_nextProjectile is not null)
            {
                AttackSystem.AddProjectile(_nextProjectile);
            }

            var animation = Animations["MeleeAttack"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
            _assetManager.PlaySound("boss_melee");
        }
    }

    private void HandleDamageBuff()
    {
        var buffThreshold = _maxHealthPoints / 3f;

        if (!DamageBuffIsActive && GameObjectHealthPoints <= buffThreshold && !GlobalSkillIsActive)
        {
            DamageBuffIsActive = true;
            TimeSinceDamageBuffActivation = 0f;
            DamageBuffAnimationRunning = true;
            GlobalSkillIsActive = true;
            var animation = Animations["Glowing"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
            _assetManager.PlaySound("boss_damage_buff");
        }
    }

    private void HandleImmune()
    {
        if (!IsImmune && TimeSinceLastImmune >= Consts.ImmuneCooldown && !GlobalSkillIsActive && TimeSinceLastGlobalSkill >= Consts.GlobalSkillCooldown)
        {
            var number = _randomNumberGenerator.Next(1, 1000);
            const int immuneWindow = 50;

            if (number <= immuneWindow)
            {
                IsImmune = true;
                GlobalSkillIsActive = true;
                var animation = Animations["Immune"];
                animation.IsFlipped = FlipAnimation;
                AnimationPlayer.PlayAnimation(animation);
                _assetManager.PlaySound("boss_immune");
                TimeSinceLastImmune = 0f;
            }
        }

    }

    private void HandleHealing()
    {
        if (!IsHealing && TimeSinceLastHealing >= Consts.HealingCooldown && !GlobalSkillIsActive && TimeSinceLastGlobalSkill >= Consts.GlobalSkillCooldown && TimeSinceLastHit >= Consts.TimeAfterHitToActivateHealing && GameObjectHealthPoints < _maxHealthPoints)
        {
            var number = _randomNumberGenerator.Next(1, 500);
            const int healingWindow = 50;

            if (number <= healingWindow)
            {
                HealingValue = _randomNumberGenerator.Next(1, 10);
                IsHealing = true;
                GlobalSkillIsActive = true;
                var animation = Animations["Healing"];
                animation.IsFlipped = FlipAnimation;
                AnimationPlayer.PlayAnimation(animation);
                _assetManager.PlaySound("boss_healing");
                TimeSinceLastHealing = 0f;
            }
        }
    }

    private void HandleDeath()
    {
        if (GameObjectHealthPoints <= 0 && EbIsAlive && !DeathAnimationRunning && !GlobalSkillIsActive)
        {
            GlobalSkillIsActive = true;
            DeathAnimationRunning = true;
            TimeSinceDeath = 0f;
            var animation = Animations["Death"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
            _assetManager.PlaySound("boss_death");

        }
    }

    private void DeleteEndBoss()
    {
        if (DeathAnimationRunning && TimeSinceDeath >= Consts.DeathAnimationDuration)
        {
            EbIsAlive = false;
            DeactivateHitBox();
            foreach (var projectile in AttackSystem.Projectiles)
            {
                projectile.DeactivateHitBox();
            }
            // AnimationPlayer.PlayAnimation(Animations["Delete"]);
        }
    }

    private void DeactivateHealing()
    {
        if (IsHealing && EbHealingDuration >= Consts.HealingDuration)
        {
            IsHealing = false;
            if (GameObjectHealthPoints + HealingValue >= _maxHealthPoints)
            {
                GameObjectHealthPoints = _maxHealthPoints;
            }
            else
            {
                GameObjectHealthPoints += HealingValue;
            }
            EbHealingDuration = 0f;
            GlobalSkillIsActive = false;
            TimeSinceLastGlobalSkill = 0f;
            var animation = Animations["Idle"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
        }
    }

    private void DeactivateImmune()
    {
        if (IsImmune && EbImmuneDuration >= Consts.ImmuneDuration)
        {
            IsImmune = false;
            EbImmuneDuration = 0f;
            GlobalSkillIsActive = false;
            TimeSinceLastGlobalSkill = 0f;
            var animation = Animations["Idle"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
        }
    }

    private void DeactivateDamageBuffAnimation()
    {
        if (DamageBuffAnimationRunning && TimeSinceDamageBuffActivation >= Consts.DamageBuffAnimationDuration)
        {
            DamageBuffAnimationRunning = false;
            GlobalSkillIsActive = false;
            var animation = Animations["Idle"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
        }
    }

    private void DeactivateRangedAttackAnimation()
    {
        if (RangeAttackActive && TimeSinceLastAttack >= Consts.RangedAttackAnimationDuration)
        {
            if (_nextProjectile is not null)
            {
                AttackSystem.AddProjectile(_nextProjectile);
            }

            RangeAttackActive = false;
            GlobalSkillIsActive = false;
            var animation = Animations["Idle"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
        }
    }

    private void DeactivateMeleeAttackAnimation()
    {
        if (MeleeAttackActive && TimeSinceLastAttack >= Consts.MeleeAttackAnimationDuration)
        {
            MeleeAttackActive = false;
            GlobalSkillIsActive = false;
            var animation = Animations["Idle"];
            animation.IsFlipped = FlipAnimation;
            AnimationPlayer.PlayAnimation(animation);
        }
    }

    public override void DealDamage(int damage)
    {
        if (!IsImmune)
        {
            GameObjectHealthPoints -= damage;
        }
    }

    private void FollowPath(GameTime gameTime, Player player)
    {
        if (Vector2.Distance(Position + HitBoxPositionOffset,
                player.Position + player.HitBoxPositionOffset) > 300)
        {
            Vector2 movementTarget = new Vector2(
                player.Position.X + HitBoxPositionOffset.X,
                player.Position.Y + HitBoxPositionOffset.Y
            );

            if (MapIndex == player.MapIndex)
            //if (false)
            {
                if (TimeSinceLastUpdate > 2.5f)
                {
                    AStar.Update(
                        new Vector2(Position.X + HitBoxPositionOffset.X, Position.Y + HitBoxPositionOffset.Y + MapIndex * Consts.ScreenHeight),
                        new Vector2(player.Position.X, player.Position.Y + player.HitBoxPositionOffset.Y + MapIndex * Consts.ScreenHeight - 100)
                    );
                    TimeSinceLastUpdate = 0;
                }

                if (AStar.Path is { Count: > 0 })
                {
                    var node = AStar.Path.First();

                    movementTarget = new Vector2(
                        node.Position.X * Consts.TileSize,
                        node.Position.Y * Consts.TileSize - MapIndex * Consts.ScreenHeight
                    );

                    if (Vector2.Distance(new Vector2(Position.X + HitBoxPositionOffset.X, Position.Y + HitBoxPositionOffset.Y),
                            new Vector2(node.Position.X * Consts.TileSize,
                                node.Position.Y * Consts.TileSize - MapIndex * Consts.ScreenHeight)) <= 0.5f)
                    {
                        AStar.Path.Remove(node);
                    }
                }
                else
                {
                    movementTarget = new Vector2(
                        player.Position.X + HitBoxPositionOffset.X,
                        player.Position.Y + HitBoxPositionOffset.Y
                    );
                }
            }

            var direction = Vector2.Normalize(movementTarget - (Position + HitBoxPositionOffset));
            var prevPosition = Position;
            Position += direction * 60 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            HitboxPositionUpdate();
            if (CollisionDetection.RunCollisionDetectionForSpecificObject(this) is not null)
            {
                Position = prevPosition;
                HitboxPositionUpdate();
            }
            MapIndex = (int)(-1 * (Position.Y - Consts.ScreenHeight) / Consts.ScreenHeight);
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
                                playerWithLowHealth.Position.Y + playerWithLowHealth.HitBoxPositionOffset.Y + playerWithLowHealth.HitBox.GetHitbox().Height / 2f)) * 2 >
                        Vector2.Distance(new Vector2(Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f, Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height / 2f),
                            new Vector2(playerWithHighHealth.Position.X + playerWithHighHealth.HitBoxPositionOffset.X + playerWithHighHealth.HitBox.GetHitbox().Width / 2f,
                                playerWithHighHealth.Position.Y + playerWithHighHealth.HitBoxPositionOffset.Y + playerWithHighHealth.HitBox.GetHitbox().Height / 2f)) ? playerWithHighHealth : playerWithLowHealth;

    }
}
