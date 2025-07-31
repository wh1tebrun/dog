#region File Description
// GameControl.cs
// Handle the entire game logic and merge components together.
#endregion

using System;
using Delusions_of_Grandeur.Animation;
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
using Delusions_of_Grandeur.View;
using Microsoft.Xna.Framework.Media;
using Delusions_of_Grandeur.Utilities;

namespace Delusions_of_Grandeur;

public class GameControl(MenuControl menuControl)
{
    // Manager
    public MapManager MapManager { get; private set; }
    public AssetManager AssetManager { get; private set; }
    private InputManager InputManager { get; set; }

    public EnemyManager EnemyManager { get; private set; }
    public SaveSystem SaveSystem { get; private set; }

    public MenuControl MenuControl { get; private set; }

    // Pathfinding
    private Pathfinding.Grid _grid;

    // Player
    public Player PlayerDefensive { get; private set; }
    public Player PlayerOffensive { get; private set; }
    public List<Player> Players => [PlayerDefensive, PlayerOffensive];
    private const int MaxHealthPlayerOffensive = 250;
    private const int MaxHealthPlayerDefensive = 250;

    // Health
    private Health Health { get; set; }
    public Healthbars HealthBars { get; private set; }
    public Potion Potion { get; set; }
    public List<Health> Healths => [Health];

    // Utilities
    public static bool DebugModeActive;
    private Debugging _debugging;
    public CollisionDetection CollisionDetection { get; private set; }
    public Camera Camera { get; private set; }
    public ref List<Statistic> Statistics => ref _statistics;
    private List<Statistic> _statistics;
    public ref List<Achievement> Achievements => ref _achievements;
    private List<Achievement> _achievements;
    public GraphicsDevice GraphicsDevice { get; private set; }

    // Normalize
    public static Rectangle RenderDestination { get; set; }

    private bool _contentLoaded;

    /// <summary>
    /// Initialize game objects.
    /// </summary>
    public void Initialize(GraphicsDevice graphicsDevice, ContentManager content,
        AssetManager assetManager, InputManager inputManager, MapManager mapManager, ref List<Statistic> statistics, ref List<Achievement> achievements, Health health, Healthbars healthBars, Rectangle renderDestination, Pathfinding.Grid grid, Potion potion)
    {
        GraphicsDevice = graphicsDevice;
        MenuControl = menuControl;
        AssetManager = assetManager;
        InputManager = inputManager;
        MapManager = mapManager;

        _grid = grid;
        
        CollisionDetection = new CollisionDetection(10, GraphicsDevice);

        Camera = new Camera();
        AssetManager.GetMusic("epic_music");

        _statistics = [];
        _statistics = statistics;

        _achievements = [];
        _achievements = achievements;

        SaveSystem = new SaveSystem(this);

        Health = health;
        HealthBars = healthBars;
        Potion = potion;

        RenderDestination = renderDestination;
    }

    /// <summary>
    /// Load content for the game.
    /// </summary>
    public void LoadContent()
    {
        if (!_contentLoaded)
        {
            _contentLoaded = true;
            var playerDefensiveAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 13, 15, 2)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 117, 125, 2)
                },
                {
                    "Jump",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 17, 19, 2)
                },
                {
                    "Fall",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 18, 19, 2)
                },
                {
                    "MeleeAttack",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 117, 125, 2)
                },
                {
                    "CastSpell",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerDefensive"), 13, 46, 0.2f, true,
                        false, 26, 32, 2)
                }
            };

            var playerOffensiveAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 13, 15, 2)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 117, 125, 2)
                },
                {
                    "Jump",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 17, 19, 2)
                },
                {
                    "Fall",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveSlingshot"), 13, 46, 0.2f,
                        true, false, 18, 19, 2)
                },
                {
                    "MeleeAttackLongsword",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveLongsword"), 13, 20, 0.2f,
                        true, false, 117, 121, 2)
                },
                {
                    "MeleeAttackSaber",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveSaber"), 13, 20, 0.2f,
                        true, false, 117, 121, 2)
                },
                {
                    "MeleeAttackSpear",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveSpear"), 13, 20, 0.2f,
                        true, false, 117, 121, 2)
                },
                {
                    "RangedAttackGreatbow",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveGreatbow"), 13, 20, 0.2f,
                        true, false, 221, 227, 2)
                },
                {
                    "RangedAttackCrossbow",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("playerOffensiveCrossbow"), 13, 20, 0.2f,
                        true, false, 66, 72, 2)
                }

            };

            var rangedEnemyAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("demonIdle"), 4, 1, 0.2f, true, false, 0,
                        3, 1)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("demonFlying"), 4, 1, 0.2f, true, false,
                        0, 3, 1)
                },
                {
                    "Jump",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("dino_rex_ability_yellow"), 25, 1, 0.2f,
                        true, false, 0, 24, 1)
                },
                {
                    "Fall",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("demonAttack"), 8, 1, 0.2f, true, false,
                        0, 7, 1)
                }
            };

            var meleeEnemyAnimations = new Dictionary<string, Animation.Animation>
            {
                {
                    "Idle",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("suicideEnemy"), 22, 5, 0.2f, true, false,
                        0, 5, 1)
                },
                {
                    "Run",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("suicideEnemy"), 22, 5, 0.2f, true, false,
                        22, 33, 1)
                },
                {
                    "Attack",
                    AnimationFactory.CreateAnimation(AssetManager.GetTexture("suicideEnemy"), 22, 5, 0.2f, true, false,
                        88, 110, 1)
                }
            };

            var offensivePlayerStartPosition = new Vector2(400, 750);
            var defensivePlayerStartPosition = new Vector2(300, 750);

            // HealthBars for the Player.
            var defensivePlayerHealthBar = new HealthBar(AssetManager, new Vector2(150, 1000), MaxHealthPlayerDefensive, MaxHealthPlayerDefensive);
            var offensivePlayerHealthBar = new HealthBar(AssetManager, new Vector2(1770, 1000), MaxHealthPlayerOffensive, MaxHealthPlayerOffensive);
            HealthBars.AddHealthbar(defensivePlayerHealthBar);
            HealthBars.AddHealthbar(offensivePlayerHealthBar);

            MapManager.LoadMaps();
            
            PlayerOffensive = new Player("Player1", offensivePlayerStartPosition, InputManager, AssetManager,
                new AnimationPlayer(), true, CollisionDetection, playerOffensiveAnimations,
                "PlayerOffensive", new Vector2(47, 37), new Vector2(35, 80), ref _statistics, ref _achievements, Health,
                offensivePlayerHealthBar, Potion, MapManager, MaxHealthPlayerOffensive);
            PlayerDefensive = new Player("Player2", defensivePlayerStartPosition, InputManager, AssetManager,
                new AnimationPlayer(), false, CollisionDetection, playerDefensiveAnimations,
                "PlayerDefensive", new Vector2(47, 33), new Vector2(35, 87), ref _statistics, ref _achievements, Health,
                defensivePlayerHealthBar, Potion, MapManager, MaxHealthPlayerDefensive);
            
            
            EnemyManager = new EnemyManager(
                GraphicsDevice,
                AssetManager,
                CollisionDetection,
                ref _statistics,
                ref Achievements,
                meleeEnemyAnimations,
                rangedEnemyAnimations,
                _grid,
                PlayerDefensive,
                PlayerOffensive,
                MapManager
            );
          
            _debugging = new Debugging(GraphicsDevice, AssetManager, [PlayerOffensive, PlayerDefensive], CollisionDetection, _grid);
            Potion.SetPlayers(PlayerDefensive, PlayerOffensive);
            Potion.InitializePotionPositions();
        }
    }

    private void HandleInput(GameTime gameTime)
    {
        if (InputManager.IsKeyDown(GlobalSettings.KeyBindings["UpDefensive"]) || InputManager.IsButtonPressed(Buttons.A, PlayerIndex.One))
        {
            PlayerDefensive.Jump();
        }

        if (InputManager.IsKeyDown(GlobalSettings.KeyBindings["UpOffensive"]) || InputManager.IsButtonPressed(Buttons.A, PlayerIndex.Two))
        {
            PlayerOffensive.Jump();
        }

        if (InputManager.IsKeyDown(GlobalSettings.KeyBindings["LeftDefensive"]) || InputManager.ThumbstickDirection(PlayerIndex.One)[0] < 0)
        {

            PlayerDefensive.Move(Player.EDirection.Left);
        }

        if (InputManager.IsKeyDown(GlobalSettings.KeyBindings["RightDefensive"]) || InputManager.ThumbstickDirection(PlayerIndex.One)[0] > 0)
        {
            PlayerDefensive.Move(Player.EDirection.Right);
        }

        if (InputManager.IsKeyDown(GlobalSettings.KeyBindings["LeftOffensive"]) || InputManager.ThumbstickDirection(PlayerIndex.Two)[0] < 0)
        {
            PlayerOffensive.Move(Player.EDirection.Left);
        }

        if (InputManager.IsKeyDown(GlobalSettings.KeyBindings["RightOffensive"]) || InputManager.ThumbstickDirection(PlayerIndex.Two)[0] > 0)
        {
            PlayerOffensive.Move(Player.EDirection.Right);
        }

        if (InputManager.IsLeftMousePressed() || InputManager.IsRightTriggerPressed(PlayerIndex.Two))
        {
            var position = Vector2.Zero;

            if (InputManager.CurrentGamePadStatePlayerTwo.IsConnected && !InputManager.IsLeftMousePressed())
            {
                const int scaleThumbstick = 150;
                const int offsetX = 10;
                const int offsetY = 0;
                Vector2 thumbstickInput = InputManager.CurrentGamePadStatePlayerTwo.ThumbSticks.Right;
                
                // calculates the angle between thumbstickInput.Y and thumbstickInput.X
                float angle = (float)Math.Atan2(thumbstickInput.Y, thumbstickInput.X);

                // calculates the dircection using the angle.
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                
                position.X = PlayerOffensive.Position.X + offsetX + direction.X * scaleThumbstick;
                position.Y = PlayerOffensive.Position.Y + offsetY - direction.Y * scaleThumbstick;
            }
            else
            {
                position = Vector2.Transform(ConvertScreenPositionToScene(InputManager.MouseClickPosition()),
                    Matrix.Invert(Camera.Transform));

            }

            PlayerOffensive.HandleRangedAttack(gameTime, position);
        }

        if (InputManager.IsRightMousePressed() || InputManager.IsLeftTriggerPressed(PlayerIndex.Two))
        {
            var position = Vector2.Zero;

            if (InputManager.CurrentGamePadStatePlayerTwo.IsConnected && !InputManager.IsLeftMousePressed())
            {
                const int scaleThumbstick = 150;
                const int offsetX = 10;
                const int offsetY = 0;
                Vector2 thumbstickInput = InputManager.CurrentGamePadStatePlayerTwo.ThumbSticks.Right;
                
                // calculates the angle between thumbstickInput.Y and thumbstickInput.X
                float angle = (float)Math.Atan2(thumbstickInput.Y, thumbstickInput.X);

                // calculates the dircection using the angle.
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                
                position.X = PlayerOffensive.Position.X + offsetX + direction.X * scaleThumbstick;
                position.Y = PlayerOffensive.Position.Y + offsetY - direction.Y * scaleThumbstick;
            }
            else
            {
                position = Vector2.Transform(ConvertScreenPositionToScene(InputManager.MouseClickPosition()),
                    Matrix.Invert(Camera.Transform));
            }
            PlayerOffensive.HandleMeleeAttack(gameTime, position);
        }

        if (InputManager.IsKeyPressed(GlobalSettings.KeyBindings["ActivateShield"]) || InputManager.IsButtonPressed(Buttons.B, PlayerIndex.One))
        {
            PlayerDefensive.ActivateShield();
        }
        if (InputManager.IsKeyPressed(GlobalSettings.KeyBindings["Invisibility"]) || InputManager.IsButtonPressed(Buttons.X, PlayerIndex.One))
        {
            if (PlayerDefensive.CanActivateInvisibility() && PlayerOffensive.CanActivateInvisibility())
            {
                PlayerDefensive.ActivateInvisibility();
                PlayerOffensive.ActivateInvisibility();
            }
            else
            {
                PlayerDefensive.SayCooldownComment();
            }
        }

        if (InputManager.IsKeyPressed(GlobalSettings.KeyBindings["DebugMode"]))
        {
            DebugModeActive = !DebugModeActive;
        }

        if (InputManager.IsKeyPressed(GlobalSettings.KeyBindings["GodMode"]))
        {
            PlayerDefensive.GodMode = !PlayerDefensive.GodMode;
            PlayerOffensive.GodMode = !PlayerOffensive.GodMode;
        }

        if (InputManager.IsKeyPressed(GlobalSettings.KeyBindings["ChangeWeapon"]) || InputManager.IsButtonPressed(Buttons.Y, PlayerIndex.Two))
        {
            PlayerOffensive.HandleGroundWeapon(true);
        }
        
        if (InputManager.IsKeyPressed(GlobalSettings.KeyBindings["ChangeShield"]) || InputManager.IsButtonPressed(Buttons.Y, PlayerIndex.One))
        {
            PlayerDefensive.HandleGroundShield(true);
        }
    }

    /// <summary>
    /// Update the main game logic.
    /// </summary>
    public int Update(GameTime gameTime)
    {
        PlayerDefensive.Update(gameTime);
        PlayerDefensive.Direction = Vector2.Zero;
        PlayerOffensive.Update(gameTime);
        PlayerOffensive.Direction = Vector2.Zero;
        EnemyManager.Update(gameTime);

        HandleInput(gameTime);
        List<MeleeEnemy> deathMeleeEnemies = new();
        foreach (var meleeEnemy in EnemyManager.MeleeEnemies)
        {
            meleeEnemy.Update(gameTime);

            if (meleeEnemy.HealthPoints <= 0)
            {
                deathMeleeEnemies.Add(meleeEnemy);
            }
        }
        foreach (var deathEnemy in deathMeleeEnemies)
        {
            EnemyManager.MeleeEnemies.Remove(deathEnemy);
        }

        List<RangedEnemy> deathRangedEnemies = new();
        foreach (var rangedEnemy in EnemyManager.RangedEnemies)
        {
            rangedEnemy.Update(gameTime);
            if (rangedEnemy.HealthPoints <= 0)
            {
                deathRangedEnemies.Add(rangedEnemy);
            }
        }
        foreach (var deathEnemy in deathRangedEnemies)
        {
            EnemyManager.RangedEnemies.Remove(deathEnemy);

            Camera.Shake(5f, 20f);
        }

        var playersMiddleY = (PlayerDefensive.Position.Y + PlayerOffensive.Position.Y) / 2;
        Vector2 playersMiddlePosition = new Vector2(0, playersMiddleY);
        Camera.Update(playersMiddlePosition);

        MapManager.Update(gameTime);

        _debugging.Update();

        PlayerOffensive.HasFallenIntoTheAbyss(Camera.GetLowerBoundary());
        PlayerDefensive.HasFallenIntoTheAbyss(Camera.GetLowerBoundary());

        // game over if one player is dead.
        if (!PlayerDefensive.IsAlive || !PlayerOffensive.IsAlive)
        {
            EnemyManager.MeleeEnemies.Clear();
            EnemyManager.RangedEnemies.Clear();
            Potion.ResetPotion();
            return 0;
        }

        CollisionDetection.TransformCollisionFields(Camera.CurrentCenter);
        CollisionDetection.UpdateAllGameObjectsToCollisionFields();

        return -1;
    }

    /// <summary>
    /// Handle draw calls for the game.
    /// </summary>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var cameraTransform = Camera.Transform;

        spriteBatch.Begin(transformMatrix: cameraTransform, samplerState: SamplerState.PointClamp);
        MapManager.Draw(cameraTransform);

        Potion.Draw(spriteBatch);
        EnemyManager.Draw(gameTime, spriteBatch);
        foreach (var meleeEnemy in EnemyManager.MeleeEnemies)
        {
            meleeEnemy.Draw(gameTime, spriteBatch);
        }
        foreach (var rangedEnemy in EnemyManager.RangedEnemies)
        {
            rangedEnemy.Draw(gameTime, spriteBatch);
        }
        
        if (DebugModeActive)
        {
            _debugging.Draw(spriteBatch, gameTime);
        }

        spriteBatch.End();

        PlayerDefensive.Draw(gameTime, spriteBatch, cameraTransform);
        PlayerOffensive.Draw(gameTime, spriteBatch, cameraTransform);
        PlayerDefensive.DrawPlayer(gameTime, spriteBatch, cameraTransform);
        PlayerOffensive.DrawPlayer(gameTime, spriteBatch, cameraTransform);
    }
    
    
    /// <summary>
    /// This Method is called in SettingsMenu and changes the volume.
    /// The allowed volume range is [0,1].
    /// </summary>
    /// <param name="volumeChangeValue"></param>
    public void ChangeBackgroundMusicVolumeArrows(float volumeChangeValue)
    {
        GlobalSettings.MediaPlayerVolume += volumeChangeValue;
        GlobalSettings.MediaPlayerVolume = MathHelper.Clamp(GlobalSettings.MediaPlayerVolume, 0f, 1f);

        MediaPlayer.Volume = GlobalSettings.MediaPlayerVolume;
    }

    public void ChangeBackgroundMusicVolumeMouse(float volume)
    {
        GlobalSettings.MediaPlayerVolume = MathHelper.Clamp(volume, 0f, 1f);
        MediaPlayer.Volume = GlobalSettings.MediaPlayerVolume;
    }

    public static Vector2 ConvertScreenPositionToScene(Vector2 screenPosition)
    {
        Vector2 resNormalize;
        resNormalize.X = RenderDestination.Width / 1920f;
        resNormalize.Y = RenderDestination.Height / 1080f;

        Vector2 posScene = new Vector2(RenderDestination.X, RenderDestination.Y);
        posScene.X /= resNormalize.X;
        posScene.Y /= resNormalize.Y;
        screenPosition.X /= resNormalize.X;
        screenPosition.Y /= resNormalize.Y;
        screenPosition.X -= posScene.X;
        screenPosition.Y -= posScene.Y;

        return screenPosition;
    }
}
