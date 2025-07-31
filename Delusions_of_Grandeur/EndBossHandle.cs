using Delusions_of_Grandeur.Animation;
using System;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Menu;
using Delusions_of_Grandeur.Pathfinding;
using Delusions_of_Grandeur.Utilities;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Delusions_of_Grandeur;

public class EndBossHandle
{
    // Utilities
    private readonly GraphicsDevice _graphicsDevice;
    private readonly InputManager _inputManager;
    private readonly MapManager _mapManager;
    private readonly AssetManager _assetManager;
    private readonly CollisionDetection _collisionDetection;
    private Debugging _debugging;
    private readonly Timer _timer;

    public List<Line> Lines { get; set; } = [];
    private float _lineSpawnTimer;
    // Time in seconds to spawn a new line
    private const float LineSpawnInterval = 3f;

    public static bool IsShaking;
    private static float _shakeDuration;
    private static float _shakeTimer;
    private static float _shakeIntensity;
    private readonly Random _random = new();
    private float _durationCounter;
    private readonly bool _pressedButtonM;

    private readonly SpriteFont _font;

    private readonly Grid _grid;

    // Entities
    public EndBoss EndBoss { get; private set; }
    private readonly Player _playerOffensive;
    private readonly Player _playerDefensive;

    // HUD / Abilities
    public  Potion Potion;

    private readonly Healthbars _healthBars;

    private List<Statistic> _statistics;
    private List<Achievement> _achievements;

    // Boolean
    private bool _contentLoaded;
    private bool _debugModeActive;

    // Integer
    private const int MaxHealthEndBoss = 10000;

    public EndBossHandle(GraphicsDevice graphicsDevice, AssetManager assetManager, InputManager inputManager, MapManager mapManager, Healthbars healthBars, Potion potion, ref List<Statistic> statistics, ref List<Achievement> achievements, Player playerOffensive, Player playerDefensive, Timer timer, bool pressedButtonM)
    {
        _durationCounter = _random.Next(10, 20);
        _graphicsDevice = graphicsDevice;
        _contentLoaded = false;

        _assetManager = assetManager;
        _inputManager = inputManager;
        _mapManager = mapManager;
        _timer = timer;

        _pressedButtonM = pressedButtonM;
        _collisionDetection = new CollisionDetection(10, _graphicsDevice);

        _healthBars = healthBars;
        Potion = potion;
        
        _grid = new Grid(graphicsDevice, _mapManager);

        _statistics = new List<Statistic>();
        _statistics = statistics;

        _achievements = new List<Achievement>();
        _achievements = achievements;

        _font = _assetManager.GetFont("MainMenu");

        _playerDefensive = playerDefensive;
        _playerOffensive = playerOffensive;
        _playerDefensive.AttackSystem.DeleteAllProjectiles();
        _playerOffensive.AttackSystem.DeleteAllProjectiles();
        _playerDefensive.AttackSystem.Projectiles.Clear();
        _playerOffensive.AttackSystem.Projectiles.Clear();
    }

    /// <summary>
    /// Load content for the game.
    /// </summary>
    public void LoadContent()
    {
        if (!_contentLoaded)
        {
            _contentLoaded = true;

            _mapManager.LoadEndbossMap();
            _grid.Update(_mapManager.Maps[0], 0);

            if (_pressedButtonM)
            {
                var offensivePlayerStartPosition = new Vector2(400, 850);
                var defensivePlayerStartPosition = new Vector2(300, 850);

                _playerDefensive.Position = defensivePlayerStartPosition;
                _playerOffensive.Position = offensivePlayerStartPosition;

            }

            _playerDefensive.SetCollisionDetection(_collisionDetection);
            _playerOffensive.SetCollisionDetection(_collisionDetection);

            //HealtBars for the Boss
            var bossHealthBar = new HealthBar(_assetManager, new Vector2(960, 70), MaxHealthEndBoss, MaxHealthEndBoss);
            _healthBars.AddHealthbar(bossHealthBar);
            CreateEndBoss(bossHealthBar);

            Potion.SetPlayers(_playerDefensive, _playerOffensive);
            Potion.InitializePotionPositions();
            _debugging = new Debugging(_graphicsDevice, _assetManager, [_playerOffensive, _playerDefensive], _collisionDetection, _grid);
        }
    }

    private void SpawnLine()
    {
        float lineWidth = 50;
        float lineHeight = Consts.ScreenHeight;

        for (var i = 0; i < 5; i++)
        {
            var linePosition = new Vector2(_random.Next((int)lineWidth, Consts.ScreenWidth - (int)lineWidth), 0);
            var newLine = new Line(linePosition, lineWidth, lineHeight, 50f, _assetManager);
            Lines.Add(newLine);
        }
    }

    private void UpdateLines(GameTime gameTime)
    {
        _lineSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_lineSpawnTimer >= LineSpawnInterval)
        {
            _lineSpawnTimer = 0f;
            SpawnLine();
        }

        // Update and check collision for each line
        foreach (var line in Lines)
        {
            line.Update(gameTime);

            line.CheckCollision(_playerDefensive);

            if (!line.IsWarning && line.IsActive && (line.CheckCollision(_playerDefensive) || line.CheckCollision(_playerOffensive)))
            {
                _playerOffensive.IsAlive = false;
                _playerDefensive.IsAlive = false;
            }

        }
    }

    private void HandleInput(GameTime gameTime)
    {
        if (_inputManager.IsKeyDown(GlobalSettings.KeyBindings["UpDefensive"]) || _inputManager.IsButtonPressed(Buttons.A, PlayerIndex.One))
        {
            _playerDefensive.Jump();
        }

        if (_inputManager.IsKeyDown(GlobalSettings.KeyBindings["UpOffensive"]) || _inputManager.IsButtonPressed(Buttons.A, PlayerIndex.Two))
        {
            _playerOffensive.Jump();
        }

        if (_inputManager.IsKeyDown(GlobalSettings.KeyBindings["LeftDefensive"]) || _inputManager.ThumbstickDirection(PlayerIndex.One)[0] < 0)
        {

            _playerDefensive.Move(Player.EDirection.Left);
        }

        if (_inputManager.IsKeyDown(GlobalSettings.KeyBindings["RightDefensive"]) || _inputManager.ThumbstickDirection(PlayerIndex.One)[0] > 0)
        {
            _playerDefensive.Move(Player.EDirection.Right);
        }

        if (_inputManager.IsKeyDown(GlobalSettings.KeyBindings["LeftOffensive"]) || _inputManager.ThumbstickDirection(PlayerIndex.Two)[0] < 0)
        {
            _playerOffensive.Move(Player.EDirection.Left);
        }

        if (_inputManager.IsKeyDown(GlobalSettings.KeyBindings["RightOffensive"]) || _inputManager.ThumbstickDirection(PlayerIndex.Two)[0] > 0)
        {
            _playerOffensive.Move(Player.EDirection.Right);
        }

        if (_inputManager.IsLeftMousePressed() || _inputManager.IsRightTriggerPressed(PlayerIndex.Two))
        {
            Vector2 position = Vector2.Zero;

            if (_inputManager.CurrentGamePadStatePlayerTwo.IsConnected && !_inputManager.IsLeftMousePressed())
            {
                const int scaleThumbstick = 150;
                const int offsetX = 10;
                const int offsetY = 0;
                Vector2 thumbstickInput = _inputManager.CurrentGamePadStatePlayerTwo.ThumbSticks.Right;
                
                // calculates the angle between thumbstickInput.Y and thumbstickInput.X
                float angle = (float)Math.Atan2(thumbstickInput.Y, thumbstickInput.X);

                // calculates the dircection using the angle.
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                
                position.X = _playerOffensive.Position.X + offsetX + direction.X * scaleThumbstick;
                position.Y = _playerOffensive.Position.Y + offsetY - direction.Y * scaleThumbstick;
            }
            else
            {
                position = GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition());
            }

            _playerOffensive.HandleRangedAttack(gameTime, position);
        }

        if (_inputManager.IsRightMousePressed() || _inputManager.IsLeftTriggerPressed(PlayerIndex.Two))
        {
            Vector2 position;

            if (_inputManager.CurrentGamePadStatePlayerTwo.IsConnected && !_inputManager.IsLeftMousePressed())
            {
                const int scaleThumbstick = 150;
                const int offsetX = 10;
                const int offsetY = 0;
                Vector2 thumbstickInput = _inputManager.CurrentGamePadStatePlayerTwo.ThumbSticks.Right;
                
                // calculates the angle between thumbstickInput.Y and thumbstickInput.X
                float angle = (float)Math.Atan2(thumbstickInput.Y, thumbstickInput.X);

                // calculates the dircection using the angle.
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                
                position.X = _playerOffensive.Position.X + offsetX + direction.X * scaleThumbstick;
                position.Y = _playerOffensive.Position.Y + offsetY - direction.Y * scaleThumbstick;
            }
            else
            {
                position = GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition());

                _playerOffensive.HandleMeleeAttack(gameTime, position);
            }
        }

        if (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["ActivateShield"]) || _inputManager.IsButtonPressed(Buttons.B, PlayerIndex.One))
        {
            _playerDefensive.ActivateShield();
        }

        if (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["Invisibility"]) || _inputManager.IsButtonPressed(Buttons.X, PlayerIndex.One))
        {
            if (_playerDefensive.CanActivateInvisibility() && _playerOffensive.CanActivateInvisibility())
            {
                _playerDefensive.ActivateInvisibility();
                _playerOffensive.ActivateInvisibility();
            }
            else
            {
                _playerDefensive.SayCooldownComment();
            }
        }

        if (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["DebugMode"]))
        {
            _debugModeActive = !_debugModeActive;
        }
    }

    /// <summary>
    /// Update the main game logic.
    /// </summary>
    public int Update(GameTime gameTime)
    {
        _debugging.Update();
        _mapManager.Update(gameTime);

        if (MenuControl.IsPauseMenuActive)
        {
            IsShaking = false;
            _shakeDuration = 0f;
            _shakeIntensity = 0f;
            _shakeTimer = 0f;
        }

        const float intendedShakeDuration = 3f;
        const float intendedShakeIntensity = 7f;

        if (_durationCounter <= 0)
        {
            _durationCounter = _random.Next(10, 20);
            StartShake(intendedShakeDuration, intendedShakeIntensity);
        }

        _durationCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        _playerDefensive.Update(gameTime);
        _playerDefensive.Direction = Vector2.Zero;
        _playerOffensive.Update(gameTime);
        _playerOffensive.Direction = Vector2.Zero;

        UpdateLines(gameTime);

        HandleInput(gameTime);

        // game over if one player is dead.
        if (!_playerDefensive.IsAlive || !_playerOffensive.IsAlive)
        {
            Potion.ResetPotion();
            _contentLoaded = false;
            return 0;
        }

        _collisionDetection.UpdateAllGameObjectsToCollisionFields();

        if (IsShaking)
        {
            _shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_shakeTimer >= _shakeDuration)
            {
                IsShaking = false;
                _shakeTimer = 0f;
            }
            MathHelper.Lerp(0.5f, 3f, (float)Math.Sin(_shakeTimer * 10));
        }

        // EndBoss begin
        EndBoss.Update(gameTime);
        // EndBoss is dead. You won.
        if (EndBoss.EbIsAlive) return -1;
        foreach (var achievement in _achievements)
        {
            if (achievement.Name == "Fast climber" && _timer.Minutes < 10)
            {
                achievement.IncreaseValue(1);
            }
            if (achievement.Name == "Invincible duo")
            {
                achievement.IncreaseValue(1);
            }
        }
        Potion.ResetPotion();
        return 2;
        // EndBoss end
    }

    /// <summary>
    /// Handle draw calls for the game.
    /// </summary>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var shakeOffset = Vector2.Zero;
        if (IsShaking)
        {
            shakeOffset = new Vector2(
                (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity,
                (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity
            );
        }

        // Create a translation matrix for the camera and shake
        Matrix transformation = Matrix.CreateTranslation(shakeOffset.X, shakeOffset.Y, 0);

        // Start drawing
        spriteBatch.Begin(transformMatrix: transformation, samplerState: SamplerState.PointClamp);

        // Draw title
        spriteBatch.DrawString(
            _font,
            "The Progenitor",
            new Vector2(845, 37),
            Color.LightGreen
        );

        // Draw map and potion
        _mapManager.Draw(transformation);
        Potion.Draw(spriteBatch);
        
        foreach (var line in Lines)
        {
            line.Draw(spriteBatch);
        }

        // Draw FPS counter
        spriteBatch.DrawString(
            _font,
            $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 1, MidpointRounding.AwayFromZero)}",
            new Vector2(20, 50),
            Color.Yellow
        );

        spriteBatch.End();

        // Draw players (unaffected by transformation)
        _playerDefensive.Draw(gameTime, spriteBatch, Matrix.Identity);
        _playerOffensive.Draw(gameTime, spriteBatch, Matrix.Identity);
        _playerDefensive.DrawPlayer(gameTime, spriteBatch, Matrix.Identity);
        _playerOffensive.DrawPlayer(gameTime, spriteBatch, Matrix.Identity);

        spriteBatch.Begin(transformMatrix: transformation, samplerState: SamplerState.PointClamp);
        // Draw end boss
        EndBoss.Draw(gameTime, spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(transformMatrix: transformation, samplerState: SamplerState.PointClamp);
        if (_debugModeActive)
        {
            _debugging.Draw(spriteBatch, gameTime);
        }
        spriteBatch.End();
    }

    private void CreateEndBoss(HealthBar healthBar)
    {
        var bossAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        0, 3, 2.5f)
                },
                {
                    "Glowing",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        10, 17, 2.5f)
                },
                {
                    "RangeAttack",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                       20, 28, 2.5f)
                },
                {
                    "Immune",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        30, 37, 2.5f)
                },
                {
                    "MeleeAttack",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        40, 46, 2.5f)
                },
                {
                    "LaserCast",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        50, 56, 2.5f)
                },
                {
                    "Healing",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        60, 69, 2.5f)
                },
                {
                    "Death",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("stoneGolemBoss"), 10, 10, 0.2f, true, false,
                        70, 83, 2.5f)
                }
            };

        var bossStartPosition = new Vector2(1500, 70);

        EndBoss = new EndBoss(_graphicsDevice, bossStartPosition, new AnimationPlayer(), _collisionDetection, bossAnimations,
            ref _statistics, ref _achievements, _playerDefensive, _playerOffensive, _assetManager, _grid, _mapManager, MaxHealthEndBoss, healthBar);
    }

    private void StartShake(float duration, float intensity)
    {
        IsShaking = true;
        _shakeDuration = duration;
        _shakeIntensity = intensity;
        _shakeTimer = 0f;
    }
}
public class Line(Vector2 position, float width, float height, float shrinkRate, AssetManager assetManager)
{
    private Vector2 Position { get; set; } = position;
    private float Width { get; set; } = width;
    private float Height { get; } = height;
    private float ShrinkRate { get; } = shrinkRate;
    private bool IsShrinking { get; } = true;
    public bool IsActive { get; private set; } = true;
    private Color Color { get; set; } = Color.Yellow;
    public bool IsWarning { get; private set; } = true;
    private float WarningTime { get; set; } = 2f;

    private bool _hasPlayedWarningSound;

    public LineSaveData GetSaveData()
    {
        return new LineSaveData
        {
            PositionX = this.Position.X,
            PositionY = this.Position.Y,
            Width = this.Width,
            Height = this.Height,
            ShrinkRate = this.ShrinkRate,
            IsShrinking = this.IsShrinking,
            IsActive = this.IsActive,
            IsWarning = this.IsWarning,
            WarningTime = this.WarningTime,
            ColorR = this.Color.R,
            ColorG = this.Color.G,
            ColorB = this.Color.B,
            ColorA = this.Color.A
        };
    }

    public Line(LineSaveData data, AssetManager assetManager)
        : this(new Vector2(data.PositionX, data.PositionY), data.Width, data.Height, data.ShrinkRate, assetManager)
    {
        IsShrinking = data.IsShrinking;
        IsActive = data.IsActive;
        IsWarning = data.IsWarning;
        WarningTime = data.WarningTime;
        Color = new Color(data.ColorR, data.ColorG, data.ColorB, data.ColorA);
    }

    public void Update(GameTime gameTime)
    {
        if (IsWarning)
        {
            if (!_hasPlayedWarningSound)
            {
                assetManager.PlaySound("alarm");
                _hasPlayedWarningSound = true;
            }
            WarningTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (WarningTime <= 0)
            {
                IsWarning = false;
                Color = Color.Red;
                _hasPlayedWarningSound = false;
            }
        }

        if (!IsWarning && IsShrinking && IsActive)
        {
            var shrinkAmount = ShrinkRate * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Width -= shrinkAmount;
            Position = new Vector2(Position.X + shrinkAmount / 2, Position.Y);

            if (Width > 0) return;

            Width = 0;
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
        {
            spriteBatch.Draw(assetManager.GetTexture("white"), new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height), new Color(Color, 3));
        }
    }

    // Check if the player collides with this line
    public bool CheckCollision(Player player)
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height).Intersects(player.HitBox.GetHitbox());
    }
}
