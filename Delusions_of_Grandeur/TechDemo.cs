using Delusions_of_Grandeur.Animation;
using System;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Menu;
using Delusions_of_Grandeur.Pathfinding;
using Delusions_of_Grandeur.Utilities;
using Delusions_of_Grandeur.View;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Delusions_of_Grandeur;

public class TechDemo
{
    private readonly GraphicsDevice _graphicsDevice;
    private bool _contentLoaded;

    private readonly InputManager _inputManager;
    private readonly MapManager _mapManager;
    private readonly AssetManager _assetManager;

    private CollisionDetection _collisionDetection;
    private bool _debugModeActive;
    private readonly SpriteFont _font;
    
    private Camera _camera;

    private Player _playerDefensive;
    private Player _playerOffensive;
    private const int MaxHealthPlayers = 200;
    public List<Player> Players => [_playerDefensive, _playerOffensive];
    
    private readonly List<RangedEnemy> _rangedEnemies = [];

    private readonly Grid _grid;

    private readonly Potion _potion;
    private readonly Health _health;
    private Healthbars _healthBars;

    private List<Statistic> _statistics;
    private List<Achievement> _achievements;

    public TechDemo(GraphicsDevice graphicsDevice, ContentManager content, AssetManager assetManager, InputManager inputManager, 
        Health health, Healthbars healthBars, Potion potion, ref List<Statistic> statistics, ref List<Achievement> achievements)
    {
        _graphicsDevice = graphicsDevice;
        _contentLoaded = false;

        _assetManager = assetManager;
        _inputManager = inputManager;
        _mapManager = new MapManager(_graphicsDevice, content);
        
        _collisionDetection = new CollisionDetection(10, _graphicsDevice);

        _camera = new Camera();
        
        _health = health;
        _healthBars = healthBars;
        _potion = potion;
        
        _grid = new Grid(graphicsDevice, _mapManager);
        
        _statistics = new List<Statistic>();
        _statistics = statistics;
        
        _achievements = new List<Achievement>();
        _achievements = achievements;

        _font = _assetManager.GetFont("MainMenu");
    }
    
    /// <summary>
    /// Load content for the game.
    /// </summary>
    public void LoadContent()
    {
        if (!_contentLoaded)
        {
            _contentLoaded = true;
            _mapManager.LoadTechDemoMap();
            _grid.Update(_mapManager.Maps[0], 0);
            var playerDefensiveAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 13, 15, 2)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 117, 125, 2)
                },
                {
                    "Jump",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 17, 19, 2)
                },
                {
                    "Fall",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 18, 19, 2)
                },
                {
                    "MeleeAttack",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 117, 125, 2)
                },
                {
                    "CastSpell",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 26, 32, 2)
                }
            };

            var playerOffensiveAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 13, 15, 2)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 117, 125, 2)
                },
                {
                    "Jump",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 17, 19, 2)
                },
                {
                    "Fall",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 18, 19, 2)
                },
                {
                    "MeleeAttackLongsword",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveLongsword"), 13, 20, 0.2f,
                        true, false, 117, 121, 2)
                },
                {
                    "MeleeAttackSaber",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveSaber"), 13, 20, 0.2f,
                        true, false, 117, 121, 2)
                },
                {
                    "MeleeAttackSpear",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveSpear"), 13, 20, 0.2f,
                        true, false, 117, 121, 2)
                },
                {
                    "RangedAttackGreatbow",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveGreatbow"), 13, 20, 0.2f,
                        true, false, 221, 227, 2)
                },
                {
                    "RangedAttackCrossbow",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("playerOffensiveCrossbow"), 13, 20, 0.2f,
                        true, false, 66, 72, 2)
                }

            };

            var rangedEnemyAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("demonIdle"), 4, 1, 0.2f, true, false, 0,
                        3, 0.75f)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("demonFlying"), 4, 1, 0.2f, true, false,
                        0, 3, 0.75f)
                },
                {
                    "Jump",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("dino_rex_ability_yellow"), 25, 1, 0.2f,
                        true, false, 0, 24, 0.75f)
                },
                {
                    "Fall",
                    AnimationFactory.CreateAnimation(_assetManager.GetTexture("demonAttack"), 8, 1, 0.2f, true, false,
                        0, 7, 0.75f)
                }
            };
            
            
            var offensivePlayerStartPosition = new Vector2(400, 805);
            var defensivePlayerStartPosition = new Vector2(300, 805);

            // HealthBars for the Player.
            var defensivePlayerHealthBar = new HealthBar(_assetManager, new Vector2(150, 1000), MaxHealthPlayers, MaxHealthPlayers);
            var offensivePlayerHealthBar = new HealthBar(_assetManager, new Vector2(1770, 1000), MaxHealthPlayers, MaxHealthPlayers);
            _healthBars.AddHealthbar(defensivePlayerHealthBar);
            _healthBars.AddHealthbar(offensivePlayerHealthBar);
            
            _playerOffensive = new Player("Player1", offensivePlayerStartPosition, _inputManager, _assetManager,
                new AnimationPlayer(), true, _collisionDetection, playerOffensiveAnimations,
                "PlayerOffensive", new Vector2(47, 37), new Vector2(35, 80), ref _statistics, ref _achievements, _health,
                offensivePlayerHealthBar, _potion, _mapManager, MaxHealthPlayers, true);
            _playerDefensive = new Player("Player2", defensivePlayerStartPosition, _inputManager, _assetManager,
                new AnimationPlayer(), false, _collisionDetection, playerDefensiveAnimations,
                "PlayerDefensive", new Vector2(47, 33), new Vector2(35, 87), ref _statistics, ref _achievements, _health,
                defensivePlayerHealthBar, _potion,  _mapManager, MaxHealthPlayers, true);

            SpawnEnemies(rangedEnemyAnimations);
            
            _potion.SetPlayers(_playerDefensive, _playerOffensive);
            _potion.InitializePotionPositions();

            foreach (var rangedEnemy in _rangedEnemies)
            {
                if (true)
                {
                    rangedEnemy.SearchPath();
                }
            }
        }
    }

    public void SpawnEnemies(Dictionary<string, Animation.Animation> rangedEnemyAnimations)
    {
        var leftMargin = 400; // offset for left border
        var enemyWidth = 40;
        var enemyHeight = 40;
        var horizontalSpacing = 20;
        var verticalSpacing = 30;

        var enemiesPerRow = (Consts.ScreenWidth - leftMargin - enemyWidth) / (enemyWidth + horizontalSpacing);
        var totalEnemies = 998;
        var currentRow = 0;
        
        var startY = -2160;
        
        for (var i = 0; i < totalEnemies; i++)
        {
            // Position of the current enemy.
            var x = leftMargin + i % enemiesPerRow * (enemyWidth + horizontalSpacing);
            var y = startY + currentRow * (enemyHeight + verticalSpacing);

            // creates the enemy at the position.
            Vector2 position = new Vector2(x, y);
            
            RangedEnemy rangedEnemy = new RangedEnemy(_graphicsDevice, position, 50f, _assetManager, new AnimationPlayer(), true,
                _collisionDetection, rangedEnemyAnimations, ref _statistics, ref _achievements, _playerOffensive, _playerDefensive, _grid, _mapManager, new Vector2(0, 15), true);
            _rangedEnemies.Add(rangedEnemy);

            // Move to the next row when the current row is full.
            if ((i + 1) % enemiesPerRow == 0)
            {
                currentRow++;
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
                position = Vector2.Transform(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition()), Matrix.Invert(_camera.Transform));
            }
            
            _playerOffensive.HandleRangedAttack(gameTime, position);
        }
        
        if (_inputManager.IsRightMousePressed() || _inputManager.IsLeftTriggerPressed(PlayerIndex.Two))
        {
            var position = Vector2.Zero;

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
                position = Vector2.Transform(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition()), Matrix.Invert(_camera.Transform));
            }
            
            _playerOffensive.HandleMeleeAttack(gameTime, position);
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
        if (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["ChangeWeapon"]) || _inputManager.IsButtonPressed(Buttons.Y, PlayerIndex.Two))
        {
            _playerOffensive.HandleGroundWeapon(true);
        }
        
        if (_inputManager.IsKeyPressed(GlobalSettings.KeyBindings["ChangeShield"]) || _inputManager.IsButtonPressed(Buttons.Y, PlayerIndex.One))
        {
            _playerDefensive.HandleGroundShield(true);
        }
    }
    
    /// <summary>
    /// Update the main game logic.
    /// </summary>
    public int Update(GameTime gameTime)
    {
        _playerDefensive.Update(gameTime);
        _playerDefensive.Direction = Vector2.Zero;
        _playerOffensive.Update(gameTime);
        _playerOffensive.Direction = Vector2.Zero;

        HandleInput(gameTime);
        
        // For parallelize the collision detection do first the global update method for each enemy.
        // After that it needed the parallelized loop for collisions detection to change the position to the previous position if needed.
        // The hit box update will be executed after parallelize collision detection   

        List<RangedEnemy> deathRangedEnemies = new ();
        foreach (var rangedEnemy in _rangedEnemies)
        {
            rangedEnemy.Update(gameTime);
            if (rangedEnemy.HealthPoints <= 0)
            {
                deathRangedEnemies.Add(rangedEnemy);
            }
        }
        
        foreach (var deathEnemy in deathRangedEnemies)
        {
            _rangedEnemies.Remove(deathEnemy);
        }

        var playersMiddleY = (_playerDefensive.Position.Y + _playerOffensive.Position.Y) / 2;
        Vector2 playersMiddlePosition = new Vector2(0, playersMiddleY);
        _camera.Update(playersMiddlePosition);

        _mapManager.Update(gameTime);
        
        _playerOffensive.HasFallenIntoTheAbyss(_camera.GetLowerBoundary());
        _playerDefensive.HasFallenIntoTheAbyss(_camera.GetLowerBoundary());

        // game over if one player is dead.
        if (!_playerDefensive.IsAlive || !_playerOffensive.IsAlive)
        {
            _rangedEnemies.Clear();
            _potion.ResetPotion();
            _contentLoaded = false;
            return 0;
        }

        if (_inputManager.IsKeyPressed(Keys.Escape) || _inputManager.IsButtonPressed(Buttons.Start, PlayerIndex.One) 
                                                    || _inputManager.IsButtonPressed(Buttons.Start, PlayerIndex.Two))
        {
            _potion.ResetPotion();
            return 0;
        }
        
        _collisionDetection.TransformCollisionFields(_camera.CurrentCenter);
        _collisionDetection.UpdateAllGameObjectsToCollisionFields();

        return -1;
    }
    
    /// <summary>
    /// Handle draw calls for the game.
    /// </summary>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var cameraTransform = _camera.Transform;

        spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
        _mapManager.Draw(cameraTransform);

        foreach (var rangedEnemy in _rangedEnemies)
        {
            rangedEnemy.Draw(gameTime, spriteBatch);
        }
        
        spriteBatch.DrawString(_font, "FPS: " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 1, MidpointRounding.AwayFromZero), Vector2.Transform(new Vector2(20,50),
            Matrix.Invert(_camera.Transform)), Color.Yellow);
        spriteBatch.DrawString(_font, "Number of active game objects: " + (_rangedEnemies.Count + Players.Count), Vector2.Transform(new Vector2(20,80),
            Matrix.Invert(_camera.Transform)), Color.Yellow);
        _potion.Draw(spriteBatch);
        spriteBatch.End();

        _playerDefensive.Draw(gameTime, spriteBatch, cameraTransform);
        _playerOffensive.Draw(gameTime, spriteBatch, cameraTransform);
        _playerDefensive.DrawPlayer(gameTime, spriteBatch, cameraTransform);
        _playerOffensive.DrawPlayer(gameTime, spriteBatch, cameraTransform);
    }
}
