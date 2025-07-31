#region File Description
// MeleeEnemy.cs
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Entities
{
    public class MeleeEnemy : Enemy
    {
        private Animation.Animation _currentAnimation;
        private readonly Dictionary<string, Animation.Animation> _animations;

        private Animation.Animation _exclamationMarkAnimation;
        private Animation.AnimationPlayer _exclamationMarkPlayer;

        public bool Flipped { get; set; }

        private const float ActivationRadius = 300;
        public bool AnimationWasFlippedBecauseOfPlayer { get; set; }

        public bool IsExploding { get; set; }
        public float ExplosionAnimationTimer { get; set; }

        public int Damage { get; set; } = 90;
        public bool ProjectileSpawned { get; set; }

        public float ExplosionDelay { get; set; }
        public float ExplosionShortFactor { get; set; } = 1f / 3f;

        private const int BorderOffsetHitBoxEnvironment = 8;
        private const int ThicknessHitBoxEnvironment = 8;
        private List<CollisionFieldEntity> _collisionFieldsEnvironment;
        private readonly MapManager _mapManager;
        private readonly AssetManager _assetManager;

        public AttackSystem AttackSystem { get; }
        

        public MeleeEnemy(
            Vector2 startPosition,
            float speed,
            Animation.AnimationPlayer animationMeleeEnemy,
            CollisionDetection collisionDetection,
            Dictionary<string, Animation.Animation> animations,
            ref List<Statistic> statistics,
            ref List<Achievement> achievements,
            MapManager mapManager,
            Player playerDefensive,
            Player playerOffensive,
            GraphicsDevice graphicsDevice,
            AssetManager assetManager
        )
            : base(graphicsDevice, null, playerOffensive, playerDefensive)
        {
            Position = startPosition;
            Speed = speed;
            GameObjectHealthPoints = 100;

            _assetManager = assetManager;
            IsExploding = false;
            ExplosionAnimationTimer = 0f;

            // Collision setup
            HitBoxPositionOffset = new Vector2(130, 100);

            CreateHitBox(new Vector2(30, 60));
            CollisionDetection = collisionDetection;
            CollisionDetection.AddGameObjectToCollisionDetection(this);

            _exclamationMarkAnimation = new Animation.Animation();

            _exclamationMarkAnimation = Animation.AnimationFactory.CreateAnimation(_assetManager.GetTexture("ExclamationMark"), 8, 1, 0.2f, true, false, 0, 7, 0.05f);

            CreateEnvironmentHitBox(Position + HitBoxPositionOffset, new Vector2(30, 60));

            AnimationPlayer = animationMeleeEnemy;
            _exclamationMarkPlayer = new Animation.AnimationPlayer();
            _animations = animations;
            _currentAnimation = _animations["Idle"];
            AnimationPlayer.PlayAnimation(_currentAnimation);

            Statistics = statistics;
            Achievements = achievements;
            _mapManager = mapManager;
            PlayerDefensive = playerDefensive;
            PlayerOffensive = playerOffensive;

            MoveDirectionX = 0;

            AttackSystem = new AttackSystem();
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsExploding)
            {


                float futureX = Position.X + MoveDirectionX * Speed * dt;
                Vector2 testPos = new Vector2(futureX, Position.Y);

                bool collidedEnv = CheckEnvironmentCollision(testPos);
                bool playerFlip = HaveFlipToRunToPlayer();
                if (collidedEnv && playerFlip)
                {

                }
                else
                {
                    if (collidedEnv)
                    {
                        FlipHorizontalDirection();

                    }
                    else
                    {
                        Position.X = futureX;
                    }
                }

                // Player collision
                var enemyBox = HitBox.GetHitbox();
                var offBox = PlayerOffensive.HitBox.GetHitbox();
                var defBox = PlayerDefensive.HitBox.GetHitbox();

                bool collideOff = enemyBox.Intersects(offBox);
                bool collideDef = enemyBox.Intersects(defBox);


                if (PlayerOffensive.ObjectCollidesWith == this || collideOff)
                {
                    Explode(PlayerOffensive);
                    return;
                }
                if (PlayerDefensive.ObjectCollidesWith == this || collideDef)
                {
                    Explode(PlayerDefensive);
                    return;
                }

                if (MoveDirectionX > 0)
                {
                    if (!AnimationWasFlippedBecauseOfPlayer)
                    {
                        Flipped = true;
                    }

                    _currentAnimation = _animations["Run"];

                }
                else if (MoveDirectionX < 0)
                {
                    if (!AnimationWasFlippedBecauseOfPlayer)
                    {
                        Flipped = false;
                    }

                    _currentAnimation = _animations["Run"];
                }
                else
                {

                    _currentAnimation = _animations["Idle"];
                }

                AnimationPlayer.PlayAnimation(_currentAnimation);
            }
            else
            {

                ExplosionAnimationTimer += dt;

                _currentAnimation = _animations["Attack"];

                AnimationPlayer.PlayAnimation(_currentAnimation);

                float shortExplosionTime = _currentAnimation.TotalDuration * ExplosionShortFactor;
                if (ExplosionAnimationTimer >= shortExplosionTime)
                {
                    HealthPoints = 0;
                    AttackSystem.DeleteAllProjectiles();

                }
            }

            if (HealthPoints <= 0)
            {
                DeactivateHitBox();
                IsAlive = false;
                if (!IsExploding)
                {
                    _assetManager.GetSound("rangedDeath");
                    _assetManager.PlayDeathSoundRangedEnemy("rangedDeath");
                    IncreaseStatisticDefeatedEnemies();
                    IncreaseAchievementAttackExpert();
                }
            }

            AttackSystem.Update(gameTime);
            UpdateAllHitBoxes();
            UpdateMapIndex();
        }

        private bool CheckEnvironmentCollision(Vector2 testPosition)
        {

            var testRect = new Rectangle(
                (int)(testPosition.X + HitBoxPositionOffset.X),
                (int)(Position.Y + HitBoxPositionOffset.Y),
                HitBox.GetHitbox().Width,
                HitBox.GetHitbox().Height
            );

            for (int m = 0; m < _mapManager.Maps.Count; m++)
            {
                var map = _mapManager.Maps[m];
                if (!map.IsCollisionActive)
                    continue;

                var adjustedRect = new Rectangle(
                    testRect.X,
                    testRect.Y + m * map.Map.Height * 16,
                    testRect.Width,
                    testRect.Height
                );

                var surroundingTiles = _mapManager.GetSurroundingTiles(map, adjustedRect, true);
                foreach (var (tileX, tileY, canCollide) in surroundingTiles)
                {
                    if (!canCollide) continue;

                    var tileRect = new Rectangle(tileX * 16, tileY * 16, 16, 16);
                    if (adjustedRect.Intersects(tileRect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void FlipHorizontalDirection()
        {

            MoveDirectionX = -MoveDirectionX;
        }

        public void Explode(Player player)
        {
            if (IsExploding) return;

            _assetManager.GetSound("explosion");
            _assetManager.PlaySound("explosion");
            IsExploding = true;
            ExplosionAnimationTimer = 0f;
            ProjectileSpawned = false;
            //DeactivateHitBox();
            if (PlayerDefensive.Shield.Type == Weapon.WeaponType.Melee && Vector2.Distance(
                    new Vector2(player.Position.X + player.HitBoxPositionOffset.X + player.HitBox.GetHitbox().Width / 2f,
                        player.Position.Y + player.HitBoxPositionOffset.Y + player.HitBox.GetHitbox().Height / 2f),
                    PlayerDefensive.Shield.Centre) <= PlayerDefensive.Shield.Radius && PlayerDefensive.Shield.IsActive)
            {
                var damage = PlayerDefensive.Shield.SetDamageToProjectile();
                if (damage == 0)
                {
                    return;
                }
                else if (damage != 1f)
                {
                    Damage = (int)(Damage * damage);

                }
            }
            player.DealDamage(Damage);
        }

        private void UpdateMapIndex()
        {
            MapIndex = (int)(-1 * (Position.Y - 1080) / 1080);
            if (MapIndex < 0)
                MapIndex = 0;
            if (MapIndex >= _mapManager.Maps.Count)
                MapIndex = _mapManager.Maps.Count - 1;
        }

        private void CreateEnvironmentHitBox(Vector2 posUpperLeft, Vector2 sizeOfTotalHitBox)
        {
            _collisionFieldsEnvironment = new List<CollisionFieldEntity>
            {
                // top
                new CollisionFieldEntity(
                    new Vector2(posUpperLeft.X + BorderOffsetHitBoxEnvironment,
                                posUpperLeft.Y),
                    new Vector2(sizeOfTotalHitBox.X - BorderOffsetHitBoxEnvironment * 2,
                                ThicknessHitBoxEnvironment)
                ),
                // right
                new CollisionFieldEntity(
                    new Vector2(posUpperLeft.X + sizeOfTotalHitBox.X - ThicknessHitBoxEnvironment,
                                posUpperLeft.Y + BorderOffsetHitBoxEnvironment),
                    new Vector2(ThicknessHitBoxEnvironment,
                                sizeOfTotalHitBox.Y - BorderOffsetHitBoxEnvironment * 2)
                ),
                // bottom
                new CollisionFieldEntity(
                    new Vector2(posUpperLeft.X + BorderOffsetHitBoxEnvironment,
                                posUpperLeft.Y + sizeOfTotalHitBox.Y - ThicknessHitBoxEnvironment),
                    new Vector2(sizeOfTotalHitBox.X - BorderOffsetHitBoxEnvironment * 2,
                                ThicknessHitBoxEnvironment)
                ),
                // left
                new CollisionFieldEntity(
                    new Vector2(posUpperLeft.X,
                                posUpperLeft.Y + BorderOffsetHitBoxEnvironment),
                    new Vector2(ThicknessHitBoxEnvironment,
                                sizeOfTotalHitBox.Y - BorderOffsetHitBoxEnvironment * 2)
                )
            };
        }

        private void UpdateAllHitBoxes()
        {
            HitboxPositionUpdate();


            _collisionFieldsEnvironment[0].UpdatePosition(
                new Vector2(
                    Position.X + HitBoxPositionOffset.X + BorderOffsetHitBoxEnvironment,
                    Position.Y + HitBoxPositionOffset.Y
                )
            );
            _collisionFieldsEnvironment[1].UpdatePosition(
                new Vector2(
                    Position.X + HitBoxPositionOffset.X +
                    (_collisionFieldsEnvironment[0].GetHitbox().Width + 2 * BorderOffsetHitBoxEnvironment)
                    - ThicknessHitBoxEnvironment,
                    Position.Y + HitBoxPositionOffset.Y + BorderOffsetHitBoxEnvironment
                )
            );
            _collisionFieldsEnvironment[2].UpdatePosition(
                new Vector2(
                    Position.X + HitBoxPositionOffset.X + BorderOffsetHitBoxEnvironment,
                    Position.Y + HitBoxPositionOffset.Y +
                    (_collisionFieldsEnvironment[1].GetHitbox().Height + 2 * BorderOffsetHitBoxEnvironment)
                    - ThicknessHitBoxEnvironment
                )
            );
            _collisionFieldsEnvironment[3].UpdatePosition(
                new Vector2(
                    Position.X + HitBoxPositionOffset.X,
                    Position.Y + HitBoxPositionOffset.Y + BorderOffsetHitBoxEnvironment
                )
            );
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*
            var _font = _assetManager.GetFont("MainMenu");
            spriteBatch.DrawString(_font, $"{Position}", new Vector2(Position.X, Position.Y - 30), Color.Purple);
            */
            if (CanColideWithDefensivePlayer() || CanCollideWithOffensivePlayer())
            {
                _exclamationMarkPlayer.PlayAnimation(_exclamationMarkAnimation);
                _exclamationMarkPlayer.IsDraw = true;
            }
            else
            {
                _exclamationMarkPlayer.IsDraw = false;
            }
            _currentAnimation.IsFlipped = Flipped;
            _exclamationMarkPlayer.Draw(gameTime, spriteBatch, new Vector2(Position.X + 120, Position.Y), 1f);
            AnimationPlayer.Draw(gameTime, spriteBatch, Position, 1f);
        }

        private bool HaveFlipToRunToPlayer()
        {

            if (CanColideWithDefensivePlayer() && CanCollideWithOffensivePlayer())
            {
                if (Vector2.Distance(Position + HitBoxPositionOffset,
                        PlayerDefensive.Position + PlayerDefensive.HitBoxPositionOffset) > Vector2.Distance(
                        Position + HitBoxPositionOffset,
                        PlayerOffensive.Position + PlayerOffensive.HitBoxPositionOffset))
                {
                    // Player offensive is closer to the enemy


                    if (Position.X + HitBoxPositionOffset.X <
                        PlayerOffensive.Position.X + PlayerOffensive.HitBoxPositionOffset.X)
                    {
                        MoveDirectionX = 1;
                        Flipped = true;
                    }
                    else
                    {
                        MoveDirectionX = -1;
                        Flipped = false;
                    }
                    AnimationWasFlippedBecauseOfPlayer = true;
                    return true;


                }
                else
                {
                    // Player defensive is closer to the enemy

                    if (Position.X + HitBoxPositionOffset.X <
                        PlayerDefensive.Position.X + PlayerDefensive.HitBoxPositionOffset.X)
                    {
                        MoveDirectionX = 1;
                        Flipped = true;
                    }
                    else
                    {
                        MoveDirectionX = -1;
                        Flipped = false;
                    }
                    AnimationWasFlippedBecauseOfPlayer = true;
                    return true;

                }

            }
            else if (CanColideWithDefensivePlayer())
            {

                if (Position.X + HitBoxPositionOffset.X <
                    PlayerDefensive.Position.X + PlayerDefensive.HitBoxPositionOffset.X)
                {
                    MoveDirectionX = 1;
                    Flipped = true;
                }
                else
                {
                    MoveDirectionX = -1;
                    Flipped = false;
                }
                AnimationWasFlippedBecauseOfPlayer = true;
                return true;

            }
            else if (CanCollideWithOffensivePlayer())
            {

                if (Position.X + HitBoxPositionOffset.X <
                    PlayerOffensive.Position.X + PlayerOffensive.HitBoxPositionOffset.X)
                {
                    MoveDirectionX = 1;
                    Flipped = true;
                }
                else
                {
                    MoveDirectionX = -1;
                    Flipped = false;
                }
                AnimationWasFlippedBecauseOfPlayer = true;
                return true;

            }
            return false;
        }

        private bool CanColideWithDefensivePlayer()
        {
            if (PlayerDefensive.IsInvisible)
            {
                AnimationWasFlippedBecauseOfPlayer = false;
                Speed = 30;
                return false;
            }

            // When the player is on the same level as the opponent and can potentially collide 
            if (PlayerDefensive.Position.Y + PlayerDefensive.HitBoxPositionOffset.Y +
                PlayerDefensive.HitBox.GetHitbox().Height < Position.Y + HitBoxPositionOffset.Y ||
                PlayerDefensive.Position.Y + PlayerDefensive.HitBoxPositionOffset.Y >
                Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height || Vector2.Distance(
                    new Vector2(
                        PlayerDefensive.Position.X + PlayerDefensive.HitBoxPositionOffset.X +
                        PlayerDefensive.HitBox.GetHitbox().Width / 2f,
                        PlayerDefensive.Position.Y + PlayerDefensive.HitBoxPositionOffset.Y +
                        PlayerDefensive.HitBox.GetHitbox().Height / 2f),
                    new Vector2(Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f,
                        Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height / 2f)) > ActivationRadius)
            {
                AnimationWasFlippedBecauseOfPlayer = false;
                Speed = 30;
                return false;
            }

            Speed = 60;
            return true;
        }

        private bool CanCollideWithOffensivePlayer()
        {
            if (PlayerOffensive.IsInvisible)
            {
                AnimationWasFlippedBecauseOfPlayer = false;
                Speed = 30;
                return false;
            }

            // When the player is on the same level as the opponent and can potentially collide 
            if (PlayerOffensive.Position.Y + PlayerOffensive.HitBoxPositionOffset.Y +
                PlayerOffensive.HitBox.GetHitbox().Height < Position.Y + HitBoxPositionOffset.Y ||
                PlayerOffensive.Position.Y + PlayerOffensive.HitBoxPositionOffset.Y >
                Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height || Vector2.Distance(
                    new Vector2(
                        PlayerOffensive.Position.X + PlayerOffensive.HitBoxPositionOffset.X +
                        PlayerOffensive.HitBox.GetHitbox().Width / 2f,
                        PlayerOffensive.Position.Y + PlayerOffensive.HitBoxPositionOffset.Y +
                        PlayerOffensive.HitBox.GetHitbox().Height / 2f),
                    new Vector2(Position.X + HitBoxPositionOffset.X + HitBox.GetHitbox().Width / 2f,
                        Position.Y + HitBoxPositionOffset.Y + HitBox.GetHitbox().Height / 2f)) > ActivationRadius)
            {
                AnimationWasFlippedBecauseOfPlayer = false;
                Speed = 30;
                return false;
            }

            Speed = 60;
            return true;
        }
    }
}
