#region File Description
// Player.cs
// Control the player.
#endregion

using System;
using System.Collections.Generic;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Entities
{
    /// <summary>
    /// Handle player movement and animations.
    /// </summary>
    public class Player : GameObject
    {
        private const float DefaultGravity = 500f;
        private const float DebugGravity = 1f;
        private const float DebugJumpVelocity = -2000f;

        public static bool PressedButtonM { get; set; } = false;

        public bool IsDebugMode { get; set; } = false;

        // private float dashTime = 0f;
        // private float dashStrength = 200f;

        private struct FloatingCooldownText
        {
            public string Text;
            public Vector2 Position;
            public float Rotation;
            public float TimeRemaining;
            public Color TextColor;
        }


        private readonly List<FloatingCooldownText> _cooldownTexts = [];
        // Boolean
        public bool IsAlive { get; set; } = true;

        public bool IsOnGround { get; set; }

        // private Effect _shockWave;
        public bool GodMode { get; set; }

        public bool Flipped { get; set; }
        public bool HasAttackSystem { get; set; }
        public bool IsInvisible { get; set; }

        public bool IsInCheckPoint { get; set; } = false;

        // If player is on EndBossMap, MapIndexes should not be updated.
        public bool OnEndBossMap { get; set; }

        // If TechDemo is active, the statistics and achievements shouldn't be updated.
        public bool TechDemo { get; set; }

        // Integer
        public int MapIndex;
        private const int BorderOffsetHitBoxEnvironment = 8;
        private const int ThicknessHitBoxEnvironment = 8;
        private readonly int _maxHealth;

        // Float
        public float JumpVelocity { get; set; }
        public float TimeSinceLastShot { get; set; }
        public float TimeSinceLastHit { get; set; } = 1f;
        public float InvisibilityDuration { get; set; } = 5f; // 5 seconds invisibility
        public float InvisibilityCooldown { get; set; } = 10f;
        public float InvisibilityTimeLeft { get; set; }
        private const float CommentCooldown = 3f;
        public float TimeSinceLastComment { get; set; } = 3f;

        // Vector 2
        public Vector2 Direction { get; set; }
        private Vector2 _direction;
        private const float Acceleration = 800f;
        private const float MaxSpeed = 200f;
        private const float Deceleration = 50f;
        private const float AirAcceleration = 250f;
        private const float MaxSpeedInAir = 200f;
        private const float AirDeceleration = 30f;

        private Vector2 _velocity = Vector2.Zero;

        private readonly Rectangle _endBossPortal;

        private Vector2 _startPosition;

        // Animation
        private readonly Dictionary<string, Animation.Animation> _animations;
        public Animation.AnimationPlayer AnimationPlayer { get; }
        private Animation.Animation _currentAnimation;
        private bool _attackAnimationIsRunning;
        private float _attackAnimationTimer;
        private const float AnimationDurationMeleeAttackLongsword = 0.7f;
        private const float AnimationDurationMeleeAttackSaber = 0.75f;
        private const float AnimationDurationMeleeAttackSpear = 0.75f;
        private const float AnimationDurationRangedAttackGreatbow = 0.75f;
        private const float AnimationDurationRangedAttackCrossbow = 0.55f;
        private const float AnimationDurationCastSpell = 1.1f;

        // String
        public string Name { get; set; }
        private readonly List<String> _cooldownComments = ["i_dont_care", "no_comment", "no_means_no", "thats_way_above_my_pay_grade", "you_have_to_believe_me", "your_wrong_about_this", "youre_lying"];

        // Utilities
        public Shield Shield { get; }
        public GroundShield GroundShield { get; set; }
        private readonly Health _health;
        private readonly HealthBar _healthBar;
        private readonly InputManager _inputManager;
        private readonly AssetManager _assetManager;
        private readonly MapManager _mapManager;
        public GameObject ObjectCollidesWith;
        public Potion Potion { get; set; }
        public AttackSystem AttackSystem { get; }
        public readonly Weapon Weapon;
        private readonly SpeechBubble _speechBubble;
        private bool _isSpeechBubbleActive;
        private readonly string _ownerId;
        private const int PickUpDistance = 30;
        public Weapon.WeaponType LastUsedWeaponType;

        // Index: 0 -> top, 1 -> right, 2 -> bottom, 3 -> left
        private List<CollisionFieldEntity> _collisionFieldsEnvironment;

        /// <summary>
        /// Constructs the player.
        /// </summary>
        public Player(string ownerId, Vector2 startPosition, InputManager inputManager,
            AssetManager assetManager, Animation.AnimationPlayer animationPlayer,
            bool hasAttackSystem, CollisionDetection collisionDetection, Dictionary<string, Animation.Animation> animations,
            string name, Vector2 hitBoxPositionOffset, Vector2 hitBoxSize, ref List<Statistic> statistics, ref List<Achievement> achievements, Health health,
            HealthBar healthBar, Potion potion, MapManager mapManager, int maxHealthPoints, bool techDemo = false, bool onEndBossMap = false)
        {
            _ownerId = ownerId;
            Position = startPosition;
            _maxHealth = maxHealthPoints;
            GameObjectHealthPoints = maxHealthPoints;
            GodMode = false;

            _inputManager = inputManager;
            _assetManager = assetManager;
            _mapManager = mapManager;

            IsOnGround = true;
            JumpVelocity = 0;

            AnimationPlayer = animationPlayer;

            AttackSystem = new AttackSystem();
            HasAttackSystem = hasAttackSystem;
            Shield = new Shield(_assetManager, this, 15, 15,
                hitBoxSize + new Vector2(0, -45));
            GroundShield = new GroundShield(_assetManager, Shield, techDemo);

            _animations = animations;
            _currentAnimation = _animations["Idle"];
            AnimationPlayer.PlayAnimation(_currentAnimation);

            HitBoxPositionOffset = hitBoxPositionOffset;
            CreateHitBox(hitBoxSize);
            CollisionDetection = collisionDetection;
            CollisionDetection.AddGameObjectToCollisionDetection(this);
            CreateEnvironmentHitBox(Position + HitBoxPositionOffset, hitBoxSize);

            GameObjectHitBox.GetHitbox();

            Name = name;
            _direction = Vector2.Zero;
            Statistics = statistics;
            Achievements = achievements;
            _health = health;
            _healthBar = healthBar;

            Potion = potion;
            ObjectCollidesWith = new GameObject();
            ObjectCollidesWith = null;
            TechDemo = techDemo;

            Weapon = new Weapon(assetManager);
            // Currently accessible from map1 in the bottom left corner (there is a gate drawn).
            _endBossPortal = new Rectangle(1650, 170, 80, 100);
            OnEndBossMap = onEndBossMap;

            // SpeechBubble for the weapons.
            _speechBubble = new SpeechBubble("", _assetManager, Vector2.Zero);

            _attackAnimationIsRunning = false;
            _attackAnimationTimer = 0f;

            MapIndex = (int)(-1 * (Position.Y - 1080) / 1080);
        }

        // private int timeSinceLastDash = 0;
        // private bool hasDashed = false;
        // private float dashAcceleration = 5000;

        /// <summary>
        /// Updates the player and handles things like gravity, animations based on state etc...
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            if (Position.Y is <= -3380 and >= -4401 || Position.Y is <= -8850 and >= -9841)
            {
                IsInCheckPoint = true;
            }
            else
            {
                IsInCheckPoint = false;
            }

            HandleAttackAnimation(gameTime);
            ObjectCollidesWith = null;
            ApplyGravity(gameTime);
            TimeSinceLastComment += (float)gameTime.ElapsedGameTime.TotalSeconds;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var isInAir = !IsOnGround;

            if (_direction.X != 0 && !isInAir)
            {
                _velocity.X += _direction.X * Acceleration * deltaTime;

                if (Math.Abs(_velocity.X) > MaxSpeed)
                {
                    _velocity.X = Math.Sign(_velocity.X) * MaxSpeed;
                }
            }
            else if (_direction.X != 0 && isInAir)
            {
                _velocity.X += _direction.X * AirAcceleration * deltaTime;

                if (Math.Abs(_velocity.X) > MaxSpeedInAir)
                {
                    _velocity.X = Math.Sign(_velocity.X) * MaxSpeedInAir;
                }
            }
            else
            {
                _velocity.X = !isInAir ? MathHelper.Lerp(_velocity.X, 0, Deceleration * deltaTime) : MathHelper.Lerp(_velocity.X, 0, AirDeceleration * deltaTime);
            }
            UpdateAllHitBoxes();

            var proposedPosition = Position + _velocity * deltaTime;
            const int screenHeight = 1080;

            if (!OnEndBossMap)
            {
                MapIndex = (int)(-1 * (Position.Y - screenHeight) / screenHeight);
                // No more Maps
                if (MapIndex > _mapManager.Maps.Count - 1)
                {
                    MapIndex = _mapManager.Maps.Count - 1;
                }
                List<int> activeMaps = new List<int>();

                if (MapIndex - 1 >= 0)
                {
                    activeMaps.Add(MapIndex - 1);
                    var mapData = _mapManager.Maps[MapIndex - 1];
                    mapData.IsDrawActive = true;
                    mapData.IsCollisionActive = true;
                    _mapManager.Maps[MapIndex - 1] = mapData;
                }

                activeMaps.Add(MapIndex);
                var currentMapData = _mapManager.Maps[MapIndex];
                currentMapData.IsDrawActive = true;
                currentMapData.IsCollisionActive = true;
                _mapManager.Maps[MapIndex] = currentMapData;

                if (MapIndex + 1 < _mapManager.Maps.Count)
                {
                    activeMaps.Add(MapIndex + 1);
                    var nextMapData = _mapManager.Maps[MapIndex + 1];
                    nextMapData.IsDrawActive = true;
                    _mapManager.Maps[MapIndex + 1] = nextMapData;
                }

                // Deactivate maps not in the active list
                for (int i = 0; i < _mapManager.Maps.Count; i++)
                {
                    if (activeMaps.Contains(i)) continue;
                    _mapManager.Maps[i].IsDrawActive = false;
                    _mapManager.Maps[i].IsCollisionActive = false;
                }
            }
            else
            {
                var currentMapData = _mapManager.Maps[0];
                currentMapData.IsDrawActive = true;
                currentMapData.IsCollisionActive = true;
                _mapManager.Maps[0] = currentMapData;
            }

            ResolveCollisions(ref proposedPosition);
            ChangePlayerPosition(new Vector2(proposedPosition.X - Position.X, proposedPosition.Y - Position.Y));
            UpdateStateAndAnimation(_direction);
            _direction = Vector2.Zero;

            if (HasAttackSystem)
            {
                AttackSystem.Update(gameTime);
            }
            else
            {
                Shield.Update(gameTime);
            }

            UpdateAllHitBoxes();
            UpdateCooldowns(gameTime);
            // Handle invisibility and other effects
            HandleInvisibility(gameTime);
            HandleHealth();
            HandleGroundWeapon(false);
            HandleGroundShield(false);

            float deltaTimeCoolDown = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = _cooldownTexts.Count - 1; i >= 0; i--)
            {
                FloatingCooldownText ft = _cooldownTexts[i];
                ft.TimeRemaining -= deltaTimeCoolDown;
                ft.Position.Y -= 15f * deltaTimeCoolDown;
                _cooldownTexts[i] = ft;

                if (ft.TimeRemaining <= 0f)
                {
                    _cooldownTexts.RemoveAt(i);
                }
            }
        }

        private void HandleInvisibility(GameTime gameTime)
        {
            if (IsInvisible)
            {
                InvisibilityTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (InvisibilityTimeLeft <= 0f)
                {
                    IsInvisible = false;
                    InvisibilityTimeLeft = InvisibilityCooldown;
                }
            }
            else
            {
                if (InvisibilityTimeLeft > 0f)
                {
                    InvisibilityTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (InvisibilityTimeLeft < 0f)
                    {
                        InvisibilityTimeLeft = 0f;
                    }
                }
            }

            if (IsInvisible && ObjectCollidesWith is Enemy)
            {
                IsInvisible = false;
                InvisibilityTimeLeft = InvisibilityCooldown;
            }
        }

        private void HandleHealth()
        {
            if (HealthPoints <= 0)
            {
                if (_health.RemoveHeart() == 1)
                {
                    HealthPoints = _maxHealth;
                }

                if (HealthPoints > 0)
                {
                    _assetManager.GetSound("heartbeat");
                    _assetManager.PlaySound("heartbeat");
                }

                if (HealthPoints <= 0 && !GodMode)
                {
                    IsAlive = false;
                }
            }
            _healthBar.Update(HealthPoints);

            if (Potion.NumberOfPotions > 0 && !HasAttackSystem && (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["ConsumePotion"]) ||
                                                _inputManager.IsLeftTriggerPressed(PlayerIndex.One)))
            {
                foreach (var statistic in Statistics)
                {
                    if (statistic.Name == "Spells used")
                    {
                        statistic.Value += 1;
                    }
                }
                Potion.SubtractPotion();
                GameObjectHealthPoints = 250; // heal defensive player
                _healthBar.Update(HealthPoints);
                Potion.PlayerOffensive.GameObjectHealthPoints = 250; // heal offensive player
                Potion.PlayerOffensive._healthBar.Update(Potion.PlayerOffensive.GameObjectHealthPoints);
                _assetManager.GetSound("potion");
                _assetManager.PlaySound("potion");
            }
        }

        /// <summary>
        /// checks if the player has fallen into the abyss.
        /// is checked with the lower boundary calculated in the camera.cs.
        /// </summary>
        /// <param name="lowerBoundary"></param>
        public void HasFallenIntoTheAbyss(float lowerBoundary)
        {
            if (Position.Y > lowerBoundary)
            {
                IsAlive = false;
            }
        }

        /// <summary>
        /// Resolves the player position based on a proposed position.
        /// If there is no obstacle in the way, the player moves toward that position.
        /// </summary>
        /// <param name="proposedPosition"> The position the player wants to move towards. </param>
        private void ResolveCollisions(ref Vector2 proposedPosition)
        {
            IsOnGround = false;

            for (int x = 0; x < _mapManager.Maps.Count; x++)
            {
                MapData map = _mapManager.Maps[x];
                if (!map.IsCollisionActive) continue;

                Rectangle hitBox = HitBox.GetHitbox();
                var surroundingTiles = _mapManager.GetSurroundingTiles(map, new Rectangle(hitBox.X, hitBox.Y + x * map.Map.Height * 16, hitBox.Width, hitBox.Height));
                foreach (var (tileX, tileY, canCollide) in surroundingTiles)
                {
                    if (!canCollide) continue;

                    // Get the tile's collision and collectable rectangles
                    var tileCollision = _mapManager.GetTile(map.Map, "CollisionLayer", (ushort)tileX, (ushort)tileY);
                    var tileCollisionRect = new Rectangle(tileCollision.X * 16, tileCollision.Y * 16, 16, 16);

                    // Loop over the four directions
                    for (var i = 0; i < 4; i++)
                    {
                        // Get the HitBox of the current collision field
                        var fieldHitBox = _collisionFieldsEnvironment[i].GetHitbox();
                        var adjustedFieldHitBox = new Rectangle(
                            fieldHitBox.X,
                            fieldHitBox.Y + x * map.Map.Height * 16,
                            fieldHitBox.Width,
                            fieldHitBox.Height
                        );

                        if (!OnEndBossMap && !TechDemo)
                        {
                            if (MapIndex == 12 && adjustedFieldHitBox.Intersects(_endBossPortal) || _inputManager.IsKeyPressed(GlobalSettings.KeyBindings["EndBossActivate"]))
                            {
                                Potion.ResetForEndBossMap();
                                MenuControl.IsEndBossActive = true;
                                PressedButtonM = true;

                            }
                        }

                        if (tileCollisionRect.Intersects(adjustedFieldHitBox))
                        {
                            switch (i)
                            {
                                case 0: // Top
                                    proposedPosition = new Vector2(proposedPosition.X, Position.Y);
                                    JumpVelocity = 20;
                                    break;

                                case 1: // Right
                                    proposedPosition = new Vector2(Position.X - 1, proposedPosition.Y);
                                    break;

                                case 2: // Bottom
                                    IsOnGround = true;
                                    JumpVelocity = 0;
                                    break;

                                case 3: // Left
                                    proposedPosition = new Vector2(Position.X + 1, proposedPosition.Y);
                                    break;
                            }
                        }
                    }
                    // Potion Collision only for defensive player.
                    if (!HasAttackSystem)
                    {
                        foreach (var rect in MapManager.PotionRectangles)
                        {
                            if (hitBox.Intersects(rect))
                            {
                                if (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["PickUpPotion"]) || _inputManager.IsRightTriggerPressed(PlayerIndex.One))
                                {
                                    // make sure that the right potion is removed.
                                    int index = -1;
                                    for (int i = 0; i < MapManager.PotionRectangles.Count; i++)
                                    {
                                        if (MapManager.PotionRectangles[i].Equals(rect))
                                        {
                                            index = i;
                                            break;
                                        }
                                    }

                                    if (index != -1)
                                    {
                                        // remove rectangle corresponding to object layer.
                                        MapManager.PotionRectangles.RemoveAt(index);
                                        // remove the position of the list for the potion textures that are drawn.
                                        Potion.RemovePotionAt(index);
                                        Potion.AddPotion();
                                        _assetManager.GetSound("item_collect");
                                        _assetManager.PlaySound("item_collect");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void Jump()
        {
            if (!IsOnGround && !IsDebugMode) return;

            JumpVelocity = IsDebugMode ? DebugJumpVelocity : -400f;
            IsOnGround = false;
            _assetManager.PlaySound("jump");
        }


        public enum EDirection
        {
            Left,
            Right
        }

        public void Move(EDirection direction)
        {
            _direction.X = direction == EDirection.Left ? -1 : 1;
        }

        /// <summary>
        /// Apply gravity to the player.
        /// </summary>
        private void ApplyGravity(GameTime gameTime)
        {
            float currentGravity = IsDebugMode ? DebugGravity : DefaultGravity;

            if (!IsOnGround)
            {
                if (JumpVelocity < 300)
                {
                    JumpVelocity += currentGravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                var prevPos = Position;
                var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position.Y += JumpVelocity * deltaTime;

                UpdateAllHitBoxes();

                if (!CanPlayerMoveWithCollision())
                {
                    Position = prevPos;
                }
            }
            else
            {
                JumpVelocity = 0;
            }
            UpdateAllHitBoxes();
        }


        /// <summary>
        /// Updates the animation based on the current movement state and direction.
        /// </summary>
        /// <param name="direction"> The direction is player is currently heading towards. </param>
        private void UpdateStateAndAnimation(Vector2 direction)
        {
            if (!IsOnGround)
            {
                _currentAnimation = JumpVelocity < 0 ? _animations["Jump"] : _animations["Fall"];
            }
            else
            {
                if (direction is { X: > 0, Y: 0 })
                {
                    _currentAnimation = _animations["Run"];
                    Flipped = true;
                }
                else if (direction is { X: < 0, Y: 0 })
                {
                    _currentAnimation = _animations["Run"];
                    Flipped = false;
                }
                else
                {
                    if (!_attackAnimationIsRunning)
                    {
                        _currentAnimation = _animations["Idle"];
                    }
                }
            }
            _currentAnimation.IsFlipped = Flipped;
            AnimationPlayer.PlayAnimation(_currentAnimation);
        }

        /// <summary>
        /// Adds projectiles from a ranged weapon if the mouse button is pressed.
        /// There are different ranged weapons.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="position"></param>
        public void HandleRangedAttack(GameTime gameTime, Vector2 position)
        {
            Weapon.CurrentWeaponType = Weapon.WeaponType.Ranged;
            LastUsedWeaponType = Weapon.WeaponType.Ranged;
            if (position.X < Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f)
            {
                Flipped = false;
            }
            else
            {
                Flipped = true;
            }

            if (TimeSinceLastShot >= Weapon.ShotCooldown)
            {
                if (position.X < Position.X + HitBoxPositionOffset.X)
                {
                    var projectile = new Projectile(
                        new Vector2(Position.X + Weapon.Offset.X, Position.Y + Weapon.Offset.Y + 30),
                        position + new Vector2(-27, +30), //Offset des Assets muss dazugerechnet werden
                        Weapon.Speed,
                        Weapon.Damage,
                        Weapon.Lifespan,
                        CollisionDetection,
                        _assetManager,
                        Weapon.HitBoxSize,
                        ref Statistics,
                        ref Achievements,
                        _ownerId,
                        this,
                        _mapManager,
                        Weapon.CurrentWeaponType,
                        TechDemo
                    );
                    AttackSystem.AddProjectile(projectile);
                }
                else
                {


                    var projectile = new Projectile(
                        new Vector2(Position.X + Weapon.Offset.X, Position.Y + Weapon.Offset.Y - 30),
                        position + new Vector2(-27, -30), //Offset des Assets muss dazugerechnet werden
                        Weapon.Speed,
                        Weapon.Damage,
                        Weapon.Lifespan,
                        CollisionDetection,
                        _assetManager,
                        Weapon.HitBoxSize,
                        ref Statistics,
                        ref Achievements,
                        _ownerId,
                        this,
                        _mapManager,
                        Weapon.CurrentWeaponType,
                        TechDemo
                    );
                    AttackSystem.AddProjectile(projectile);
                }


                string soundName = "gun_sound";
                switch (Weapon.Name)
                {
                    case "Greatbow":
                        soundName = "greatbowsound";
                        break;
                    case "Crossbow":
                        soundName = "crossbowsound";
                        break;
                }
                _assetManager.GetSound(soundName);
                _assetManager.PlaySound(soundName);
                TimeSinceLastShot = 0;
                if (Weapon.Name == "Greatbow")
                {
                    _currentAnimation = _animations["RangedAttackGreatbow"];
                    _attackAnimationTimer = AnimationDurationRangedAttackGreatbow;
                    _attackAnimationIsRunning = true;
                }
                if (Weapon.Name == "Crossbow")
                {
                    _currentAnimation = _animations["RangedAttackCrossbow"];
                    _attackAnimationTimer = AnimationDurationRangedAttackCrossbow;
                    _attackAnimationIsRunning = true;
                }
                _currentAnimation.IsFlipped = Flipped;
                AnimationPlayer.PlayAnimation(_currentAnimation);
            }
            else
            {
                SayCooldownComment();
            }
        }

        /// <summary>
        /// Adds projectiles from a melee weapon if the mouse button is pressed.
        /// There are different melee weapons.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="position"></param>
        public void HandleMeleeAttack(GameTime gameTime, Vector2 position)
        {
            if (position.X < Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f)
            {
                Flipped = false;
            }
            else
            {
                Flipped = true;
            }
            Weapon.CurrentWeaponType = Weapon.WeaponType.Melee;
            LastUsedWeaponType = Weapon.WeaponType.Melee;
            if (Flipped) // Right side
            {
                _startPosition = new Vector2(
                    Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width,
                    Position.Y + Weapon.Offset.Y
                );
            }
            else // Left side
            {
                _startPosition = new Vector2(
                    Position.X + HitBoxPositionOffset.X - Weapon.HitBoxSize.X,
                    Position.Y + Weapon.Offset.Y
                );
            }

            if (TimeSinceLastHit >= Weapon.ShotCooldown)
            {
                var projectile = new Projectile(
                    _startPosition,
                    position,
                    Weapon.Speed,
                    Weapon.Damage,
                    Weapon.Lifespan,
                    CollisionDetection,
                    _assetManager,
                    Weapon.HitBoxSize,
                    ref Statistics,
                    ref Achievements,
                    _ownerId,
                    this,
                    _mapManager,
                    Weapon.CurrentWeaponType,
                    TechDemo
                );
                AttackSystem.AddProjectile(projectile);
                string soundName = "sabersound";
                switch (Weapon.Name)
                {
                    case "Saber":
                        soundName = "sabersound";
                        break;
                    case "Longsword":
                        soundName = "swordsound";
                        break;
                    case "Spear":
                        soundName = "spearsound";
                        break;
                }
                _assetManager.GetSound(soundName);
                _assetManager.PlaySound(soundName);
                TimeSinceLastHit = 0;

                if (Weapon.Name == "Longsword")
                {
                    _currentAnimation = _animations["MeleeAttackLongsword"];
                    _attackAnimationTimer = AnimationDurationMeleeAttackLongsword;
                    _attackAnimationIsRunning = true;
                }
                if (Weapon.Name == "Saber")
                {
                    _currentAnimation = _animations["MeleeAttackSaber"];
                    _attackAnimationTimer = AnimationDurationMeleeAttackSaber;
                    _attackAnimationIsRunning = true;
                }
                if (Weapon.Name == "Spear")
                {
                    _currentAnimation = _animations["MeleeAttackSpear"];
                    _attackAnimationTimer = AnimationDurationMeleeAttackSpear;
                    _attackAnimationIsRunning = true;
                }
                _currentAnimation.IsFlipped = Flipped;
                AnimationPlayer.PlayAnimation(_currentAnimation);
            }
            else
            {
                SayCooldownComment();
            }
        }

        /// <summary>
        /// collects the new weapon on the ground if player is nearby.
        /// </summary>
        public void HandleGroundWeapon(bool collect)
        {
            if (HasAttackSystem)
            {
                foreach (var weapon in Weapon.RangedWeapons)
                {
                    if (weapon.IsActive) continue;
                    var distance = Vector2.Distance(Position, weapon.Position);
                    if (distance <= PickUpDistance)
                    {
                        if (collect)
                        {
                            int index = Weapon.RangedWeapons.IndexOf(weapon);
                            Weapon.SwitchRangedWeapon(index);
                            break;
                        }

                        Weapon.CurrentWeaponType = Weapon.WeaponType.Ranged;
                        float speed = weapon.ShotCooldown - Weapon.ShotCooldown;
                        speed = (float)Math.Round(speed, 2);
                        string speedString = "Speed: " + (speed > 0 ? "+" : "") + speed.ToString();
                        int damage = weapon.Damage - Weapon.Damage;
                        string damageString = "Damage: " + (damage > 0 ? "+" : "") + damage.ToString();
                        int range = weapon.Lifespan - Weapon.Lifespan;
                        string rangeString = "Range: " + (range > 0 ? "+" : "") + range.ToString();

                        string message = weapon.Name + "\n" + speedString + "\n" + damageString + "\n" + rangeString;

                        Vector2 speechbubbleOffset = new Vector2(20, -200);
                        _speechBubble.Update(message, weapon.Position + speechbubbleOffset);
                        _isSpeechBubbleActive = true;
                    }
                }

                foreach (var weapon in Weapon.MeleeWeapons)
                {
                    if (weapon.IsActive) continue;
                    var distance = Vector2.Distance(Position, weapon.Position);
                    if (distance <= PickUpDistance)
                    {
                        if (collect)
                        {
                            int index = Weapon.MeleeWeapons.IndexOf(weapon);
                            Weapon.SwitchMeleeWeapon(index);
                            break;
                        }
                        Weapon.CurrentWeaponType = Weapon.WeaponType.Melee;
                        float speed = weapon.ShotCooldown - Weapon.ShotCooldown;
                        speed = (float)Math.Round(speed, 2);
                        string speedString = "Speed: " + (speed > 0 ? "+" : "") + speed.ToString();
                        int damage = weapon.Damage - Weapon.Damage;
                        string damageString = "Damage: " + (damage > 0 ? "+" : "") + damage.ToString();
                        float range = weapon.HitBoxSize.X - Weapon.HitBoxSize.X;
                        range = (float)Math.Round(range, 2);
                        string rangeString = "Range: " + (range > 0 ? "+" : "") + range.ToString();

                        string message = weapon.Name + "\n" + speedString + "\n" + damageString + "\n" +
                                         rangeString;

                        Vector2 speechbubbleOffset = new Vector2(20, -200);
                        _speechBubble.Update(message, weapon.Position + speechbubbleOffset);
                        _isSpeechBubbleActive = true;

                    }
                }
            }
            Weapon.CurrentWeaponType = LastUsedWeaponType;
        }

        /// <summary>
        /// logic of the ground shield. same as the weapons.
        /// </summary>
        /// <param name="collect"></param>
        public void HandleGroundShield(bool collect)
        {
            if (!HasAttackSystem)
            {
                var distance = Vector2.Distance(Position, GroundShield.Position);
                if (distance <= PickUpDistance)
                {
                    if (collect)
                    {
                        GroundShield.SwitchShield();
                    }

                    var type = GroundShield.Type;
                    string typeString = "blocks damage\nfrom " + "\n" + type.ToString() + "enemies.";

                    string message = GroundShield.Name + ":\n" + typeString;

                    var speechBubbleOffset = new Vector2(20, -200);
                    _speechBubble.Update(message, GroundShield.Position + speechBubbleOffset);
                    _isSpeechBubbleActive = true;
                }
            }
        }
        public void ActivateShield()
        {
            if (!Shield.IsActive && Shield.TimeSinceLastActivation >= Shield.CoolDown)
            {
                Shield.SetActive();
                _currentAnimation = _animations["CastSpell"];
                _attackAnimationTimer = AnimationDurationCastSpell;
                _attackAnimationIsRunning = true;
                _currentAnimation.IsFlipped = Flipped;
                AnimationPlayer.PlayAnimation(_currentAnimation);

            }
            else
            {
                SayCooldownComment();
            }
        }

        public void ActivateInvisibility()
        {
            if (!IsInvisible && InvisibilityTimeLeft <= 0f)
            {
                IsInvisible = true;
                InvisibilityTimeLeft = InvisibilityDuration;
                if (this.Name == "PlayerDefensive")
                {
                    _currentAnimation = _animations["CastSpell"];
                    _attackAnimationTimer = AnimationDurationCastSpell;
                    _attackAnimationIsRunning = true;
                    _currentAnimation.IsFlipped = Flipped;
                    AnimationPlayer.PlayAnimation(_currentAnimation);
                }
            }
        }

        public bool CanActivateInvisibility()
        {
            if (!IsInvisible && InvisibilityTimeLeft <= 0f)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the animated player and an attack system, if the player has one.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraTransform"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Matrix cameraTransform)
        {
            if (HasAttackSystem)
            {
                spriteBatch.Begin(transformMatrix: cameraTransform);
                AttackSystem.Draw(spriteBatch);
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(transformMatrix: cameraTransform);
                Shield.Draw(spriteBatch);
                spriteBatch.End();
            }

            spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
            if (!OnEndBossMap)
            {
                if (HasAttackSystem)
                {
                    Weapon.Draw(spriteBatch);
                }
                else
                {
                    GroundShield.Draw(spriteBatch);
                }

                if (_isSpeechBubbleActive)
                {
                    _speechBubble.Draw(spriteBatch);
                    _isSpeechBubbleActive = false;
                }
            }

            foreach (var ft in _cooldownTexts)
            {
                spriteBatch.DrawString(
                    _assetManager.GetFont("MainMenu"),
                    ft.Text,
                    ft.Position,
                    ft.TextColor,
                    ft.Rotation,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );
            }
            spriteBatch.End();
        }

        public void DrawPlayer(GameTime gameTime, SpriteBatch spriteBatch, Matrix cameraTransform)
        {
            spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
            float alpha = IsInvisible ? 0.5f : 1.0f;
            AnimationPlayer.Draw(gameTime, spriteBatch, Position, alpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Moves the player by the given vector if there is no collsion detected.
        /// </summary>
        /// <param name="positionOffset"></param>
        private void ChangePlayerPosition(Vector2 positionOffset)
        {
            UpdateAllHitBoxes(positionOffset);

            if (CanPlayerMoveWithCollision())
            {
                Position.X += positionOffset.X;
                Position.Y += positionOffset.Y;
            }
            UpdateAllHitBoxes();
        }

        /// <summary>
        /// Uses collision detection to check whether a movement can be carried out. When a collision is detected, 
        /// it is checked whether it is an object that the player can walk through. (projectile, shield, ...)
        /// </summary>
        /// <returns></returns>
        private bool CanPlayerMoveWithCollision()
        {
            GameObject collisionObject = CollisionDetection.RunCollisionDetectionForSpecificObject(this);
            if (collisionObject is null or Projectile or Equipment.Shield)
            {
                return true;
            }
            ObjectCollidesWith = collisionObject;
            return false;
        }

        private void UpdateCooldowns(GameTime gameTime)
        {
            TimeSinceLastHit += (float)gameTime.ElapsedGameTime.TotalSeconds;
            TimeSinceLastShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Gets the position of the upper left corner and the size of the total hit box.
        /// It creates 4 small hit boxes with gaps to the borders for the map collision.
        /// </summary>
        /// <param name="positionUpperLeftCorner"></param>
        /// <param name="sizeOfTotalHitBox"></param>
        private void CreateEnvironmentHitBox(Vector2 positionUpperLeftCorner, Vector2 sizeOfTotalHitBox)
        {
            _collisionFieldsEnvironment =
            [
                new CollisionFieldEntity(
                    new Vector2(positionUpperLeftCorner.X + BorderOffsetHitBoxEnvironment, positionUpperLeftCorner.Y),
                    new Vector2(sizeOfTotalHitBox.X - BorderOffsetHitBoxEnvironment * 2, ThicknessHitBoxEnvironment)),
                // Right hit box
                new CollisionFieldEntity(
                    new Vector2(positionUpperLeftCorner.X + sizeOfTotalHitBox.X - ThicknessHitBoxEnvironment,
                        positionUpperLeftCorner.Y + BorderOffsetHitBoxEnvironment),
                    new Vector2(ThicknessHitBoxEnvironment, sizeOfTotalHitBox.Y - BorderOffsetHitBoxEnvironment * 2)),
                // Lower hit box
                new CollisionFieldEntity(
                    new Vector2(positionUpperLeftCorner.X + BorderOffsetHitBoxEnvironment,
                        positionUpperLeftCorner.Y + sizeOfTotalHitBox.Y - ThicknessHitBoxEnvironment),
                    new Vector2(sizeOfTotalHitBox.X - BorderOffsetHitBoxEnvironment * 2, ThicknessHitBoxEnvironment)),
                // Left hit box
                new CollisionFieldEntity(
                    new Vector2(positionUpperLeftCorner.X, positionUpperLeftCorner.Y + BorderOffsetHitBoxEnvironment),
                    new Vector2(ThicknessHitBoxEnvironment, sizeOfTotalHitBox.Y - BorderOffsetHitBoxEnvironment * 2))
            ];
        }

        // Updates all hit boxes from the player
        private void UpdateAllHitBoxes()
        {
            HitboxPositionUpdate();

            _collisionFieldsEnvironment[0].UpdatePosition(new Vector2(Position.X + HitBoxPositionOffset.X + BorderOffsetHitBoxEnvironment, Position.Y + HitBoxPositionOffset.Y));
            _collisionFieldsEnvironment[1].UpdatePosition(new Vector2(Position.X + HitBoxPositionOffset.X + (_collisionFieldsEnvironment[0].GetHitbox().Width + 2 * BorderOffsetHitBoxEnvironment) - ThicknessHitBoxEnvironment, Position.Y + HitBoxPositionOffset.Y + BorderOffsetHitBoxEnvironment));
            _collisionFieldsEnvironment[2].UpdatePosition(new Vector2(Position.X + HitBoxPositionOffset.X + BorderOffsetHitBoxEnvironment, Position.Y + HitBoxPositionOffset.Y + (_collisionFieldsEnvironment[1].GetHitbox().Height + 2 * BorderOffsetHitBoxEnvironment) - ThicknessHitBoxEnvironment));
            _collisionFieldsEnvironment[3].UpdatePosition(new Vector2(Position.X + HitBoxPositionOffset.X, Position.Y + HitBoxPositionOffset.Y + BorderOffsetHitBoxEnvironment));
        }

        private void UpdateAllHitBoxes(Vector2 proposedPosition)
        {
            HitboxPositionUpdate(proposedPosition);

            _collisionFieldsEnvironment[0].UpdatePosition(new Vector2(proposedPosition.X + BorderOffsetHitBoxEnvironment, proposedPosition.Y + HitBoxPositionOffset.Y));
            _collisionFieldsEnvironment[1].UpdatePosition(new Vector2(proposedPosition.X + (_collisionFieldsEnvironment[0].GetHitbox().Width + 2 * BorderOffsetHitBoxEnvironment) - ThicknessHitBoxEnvironment, proposedPosition.Y + BorderOffsetHitBoxEnvironment));
            _collisionFieldsEnvironment[2].UpdatePosition(new Vector2(proposedPosition.X + BorderOffsetHitBoxEnvironment, proposedPosition.Y + (_collisionFieldsEnvironment[1].GetHitbox().Height + 2 * BorderOffsetHitBoxEnvironment) - ThicknessHitBoxEnvironment));
            _collisionFieldsEnvironment[3].UpdatePosition(new Vector2(proposedPosition.X, proposedPosition.Y + BorderOffsetHitBoxEnvironment));
        }

        public void SetCollisionDetection(CollisionDetection collisionDetection)
        {
            CollisionDetection = collisionDetection;
            CollisionDetection.AddGameObjectToCollisionDetection(this);
        }

        private void HandleAttackAnimation(GameTime gameTime)
        {
            if (_attackAnimationTimer >= 0)
            {
                _attackAnimationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                _attackAnimationTimer = 0f;
                _attackAnimationIsRunning = false;
            }

            if (this.Name == "PlayerOffensive" && !(_currentAnimation == _animations["MeleeAttackLongsword"] ||
                                                   _currentAnimation == _animations["MeleeAttackSaber"] ||
                                                   _currentAnimation == _animations["MeleeAttackSpear"] ||
                                                   _currentAnimation == _animations["RangedAttackGreatbow"] ||
                                                   _currentAnimation == _animations["RangedAttackCrossbow"]))
            {
                _attackAnimationIsRunning = false;
            }
            else if (this.Name == "PlayerDefensive" && _currentAnimation != _animations["CastSpell"])
            {
                _attackAnimationIsRunning = false;
            }
        }

        public void WriteBlockedText(string text)
        {
            _cooldownTexts.Clear();
            Random rnd = new Random();
            float randomOffsetX = (float)(rnd.NextDouble() * 20f - 10f);
            float randomOffsetY = (float)(rnd.NextDouble() * 10f - 20f);
            Vector2 baseHeadPos = new Vector2(Position.X, Position.Y - 30);
            Vector2 randomPos = baseHeadPos + new Vector2(randomOffsetX, randomOffsetY);

            float randomRotation = (float)(rnd.NextDouble() * MathHelper.ToRadians(30) - MathHelper.ToRadians(15));

            FloatingCooldownText floatingText = new FloatingCooldownText
            {
                Text = text,
                Position = randomPos,
                Rotation = randomRotation,
                TimeRemaining = 1.0f,
                TextColor = Color.Red
            };
            _cooldownTexts.Add(floatingText);
        }

        public void SayCooldownComment()
        {
            if (TimeSinceLastComment >= CommentCooldown)
            {
                var random = new Random();
                var position = random.Next(0, _cooldownComments.Count);
                _assetManager.GetSound(_cooldownComments[position]);
                _assetManager.PlaySound(_cooldownComments[position]);
                TimeSinceLastComment = 0;
            }
        }

        public override void DealDamage(int damage)
        {
            if (!GodMode)
            {
                GameObjectHealthPoints -= damage;
            }
        }
    }
}
