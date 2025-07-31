using System;
using System.Collections.Generic;
using System.IO;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Delusions_of_Grandeur.Menu;

public class MenuControl
{
    public bool IsMainMenuActive { get; set; } = true;

    public ScreenManager ScreenManager { get; }
    private readonly InputManager _inputManager;
    private readonly AssetManager _assetManager;
    private readonly MapManager _mapManager;
    private readonly GraphicsDevice _graphicsDevice;
    private ContentManager _content;
    public Timer Timer { get; private set; }

    public GameControl GameControl { get; private set; }
    private TechDemo _techDemo;
    public static bool IsPauseMenuActive { get; set; }
    public static bool IsEndBossActive { get; set; }
    public static bool IsKeybindingActive { get; set; }
    private static bool _keyBindingStarted;
    private bool IsEndBossMapLoaded { get; set; }
    public bool IsSubMenuActive { get; set; }
    public bool IsSaveGameMenuActive { get; set; }
    private bool _isGameScreenActive;
    private bool _isTechdemoActive;
    private bool _isLossMenuActive;
    private bool _isWonMenuActive;
    private bool _exitGame;

    private readonly Song _menuSong;
    private readonly Song _lostGameSong;
    private readonly Song _wonGameSong;
    private readonly Song _endBossSong;
    public static bool HasStoppedMenuMusic;
    private readonly Song _backgroundMusic;
    private Song _currentSong;
    private List<Statistic> _statistics;
    private List<Achievement> _achievements;
    private Health _health;
    private Healthbars _healthBars;
    private HudScreen _hudScreen;
    private bool _isGameOn;
    private readonly AchievementMenu _achievementMenu;
    private readonly string _achievementsFilePath;
    private readonly StatisticMenu _statisticMenu;
    private readonly string _statisticsFilePath;
    private bool _shakeToggled;
    public Potion Potion { get; set; }

    private readonly Pathfinding.Grid _grid;

    public Rectangle RenderDestination { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="screenManager"></param>
    /// <param name="assetManager"></param>
    /// <param name="inputManager"></param>
    /// <param name="mapManager"></param>
    /// <param name="graphicsDevice"></param>
    /// <param name="renderDestination"></param>
    /// <param name="grid"></param>
    public MenuControl(ScreenManager screenManager, AssetManager assetManager, InputManager inputManager, MapManager mapManager, GraphicsDevice graphicsDevice, Rectangle renderDestination, Pathfinding.Grid grid)
    {
        GlobalSettings.MediaPlayerVolume = 0.5f;
        MediaPlayer.Volume = GlobalSettings.MediaPlayerVolume;
        SoundEffect.MasterVolume = 0.5f;
        _graphicsDevice = graphicsDevice;
        _assetManager = assetManager;
        _inputManager = inputManager;
        _mapManager = mapManager;
        ScreenManager = screenManager;
        Timer = new Timer(_assetManager);
        GameControl = new GameControl(this);
        _menuSong = _assetManager.GetMusic("Route209");
        _lostGameSong = _assetManager.GetMusic("lostgamemusic");
        _wonGameSong = _assetManager.GetMusic("wongamemusic");
        _backgroundMusic = _assetManager.GetMusic("epic_music");
        _endBossSong = _assetManager.GetMusic("bossmusic");
        IsPauseMenuActive = false;
        IsSubMenuActive = false;
        IsSaveGameMenuActive = false;
        IsEndBossActive = false;
        IsEndBossMapLoaded = false;
        _isGameScreenActive = false;
        _isTechdemoActive = false;
        _isLossMenuActive = false;
        _isWonMenuActive = false;

        _grid = grid;

        _statistics =
        [
            new Statistic("Total defeated enemies"),
            new Statistic("Damage endured"),
            new Statistic("Spells used"),
            new Statistic("Damaged dealt"),
            new Statistic("Blocked damage")

        ];

        _achievements =
        [
            new Achievement("Invincible duo",
                "Complete the game without having used a resurrection", 1),

            new Achievement("Endurance",
                "Endure 1000 damage points in the game.", 1000),

            new Achievement("Master of defense",
                "Block a total of 500 points of damage with the shield.", 500),

            new Achievement("Attack expert",
                "Defeat 100 enemies with the offensive player.", 100),

            new Achievement("Fast climber",
                "Finish the game in under 10 minutes.", 1)
        ];

        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _achievementsFilePath = Path.Combine(folderPath, "achievements.json");
        _achievementMenu = new AchievementMenu(_assetManager, _inputManager, _graphicsDevice, _achievements);
        if (File.Exists(_achievementsFilePath))
        {
            _achievementMenu.LoadAchievementsFromFile(_achievementsFilePath);
        }

        _statisticMenu = new StatisticMenu(_assetManager, _inputManager, _statistics, _graphicsDevice);
        _statisticsFilePath = Path.Combine(folderPath, "statistics.json");
        if (File.Exists(_statisticsFilePath))
        {
            _statisticMenu.LoadStatisticsFromFile(_statisticsFilePath);
        }
            
        _health = new Health(_assetManager);
        _healthBars = new Healthbars();
        Potion = new Potion(_assetManager);
            
            
        RenderDestination = renderDestination;
        _techDemo = new TechDemo(_graphicsDevice, _content, _assetManager, _inputManager,
            new Health(_assetManager),
            _healthBars, Potion, ref _statistics, ref _achievements);

    }

    /// <summary>
    /// starts the program. it always begins in the main menu.
    /// </summary>
    public void StartProgram() => ScreenManager.Push(new MainMenuScreen(new MainMenu(_assetManager, _inputManager, _graphicsDevice)));

    /// <summary>
    /// Initializes the game control.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="graphics"></param>
    public void Initialize(ContentManager content, GraphicsDeviceManager graphics)
    {
        _content = content;
        CreateNewGame(true);
    }

    /// <summary>
    /// updates the menu navigation based on the user input and which screen is displayed at this moment.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <returns></returns>
    public bool Update(GameTime gameTime)
    {
        HandleBackgroundMusic();

        var selectedButton = ScreenManager.Update(gameTime);

        GameControl.RenderDestination = RenderDestination;

        if (IsMainMenuActive && MediaPlayer.State != MediaState.Playing)
        {
            MediaPlayer.Play(_menuSong);
        }

        if (_isGameScreenActive)
        {
            HandlePauseMenuToggle();
            _isGameOn = GameIsOn(selectedButton);
        }

        if (IsEndBossActive && !IsEndBossMapLoaded)
        {
            StartEndBossMap(Player.PressedButtonM);
        }

        if (IsKeybindingActive && !_keyBindingStarted)
        {
            StartKeybindingsScreen();
        }
            
        if (IsMainMenuActive)
        {
            HandleMainMenuSelection(selectedButton);
        }
            
        if (IsPauseMenuActive)
        {
            HandlePauseMenuSelection(selectedButton);
            if (IsSaveGameMenuActive)
            {
                return _exitGame;
            }
        }

        if (IsSubMenuActive)
        {
            HandleSubMenuReturn(selectedButton);
        }

            
        // Stop music once when entering the game screen
        if (_isGameScreenActive && !HasStoppedMenuMusic)
        {
            MediaPlayer.Stop();
            HasStoppedMenuMusic = true;
        }

        if (_isGameOn || !_isGameScreenActive) return _exitGame;
        _achievementMenu.SaveAchievementsToFile(_achievementsFilePath);
        _statisticMenu.SaveStatisticsToFile(_statisticsFilePath);
        HandleLossOrWonMenu(selectedButton);
        return _exitGame;
    }

    /// <summary>
    /// draws current screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => ScreenManager.Draw(gameTime, spriteBatch);

    /// <summary>
    /// says if you lost the game (returns false in that case)
    /// </summary>
    /// <param name="selectedButton"></param>
    /// <returns></returns>
    private bool GameIsOn(int selectedButton)
    {
        return selectedButton == -1;
    }

    private void HandlePauseMenuToggle()
    {
        if (!_inputManager.IsKeyPressed(Keys.Escape) &&
            !_inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One) &&
            !_inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.Two)) return;

        if (!IsPauseMenuActive)
        {
            if (EndBossHandle.IsShaking)
            {
                _shakeToggled = true;
                EndBossHandle.IsShaking = false;
            }
            else
            {
                _shakeToggled = false;
            }
            ScreenManager.Push(new PauseMenuScreen(new PauseMenu(_assetManager, _inputManager, GameControl, _graphicsDevice)));
            IsPauseMenuActive = true;
            _isGameScreenActive = false;
        }
    }

    private void HandlePauseMenuSelection(int selectedButton)
    {
        GameControl.PlayerOffensive.AnimationPlayer.IsPaused = true;
        GameControl.PlayerDefensive.AnimationPlayer.IsPaused = true;

        foreach (var meleeEnemy in GameControl.EnemyManager.MeleeEnemies)
        {
            meleeEnemy.AnimationPlayer.IsPaused = true;
        }

        foreach (var rangedEnemy in GameControl.EnemyManager.RangedEnemies)
        {
            rangedEnemy.AnimationPlayer.IsPaused = true;
        }
        switch (selectedButton)
        {
            case 0:
                // Continue button logic
                ScreenManager.Pop();

                if (_shakeToggled)
                {
                    EndBossHandle.IsShaking = true;
                }
                _isGameScreenActive = true;
                IsPauseMenuActive = false;
                PlaySong(_backgroundMusic, true);
                GameControl.PlayerOffensive.AnimationPlayer.IsPaused = false;
                GameControl.PlayerDefensive.AnimationPlayer.IsPaused = false;
                foreach (var meleeEnemy in GameControl.EnemyManager.MeleeEnemies)
                {
                    meleeEnemy.AnimationPlayer.IsPaused = false;
                }

                foreach (var rangedEnemy in GameControl.EnemyManager.RangedEnemies)
                {
                    rangedEnemy.AnimationPlayer.IsPaused = false;
                }
                break;
            case 1:
                // Save Game button logic
                IsMainMenuActive = false;
                IsPauseMenuActive = false;
                IsSubMenuActive = true;
                IsSaveGameMenuActive = true;
                ScreenManager.Push(new SaveGameScreen(new
                    SaveGameMenu(_assetManager, _inputManager,
                        _graphicsDevice, ESaveOperationType.SaveGame, this)));
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                // Quit button logic
                Potion.ResetPotion(); // has to be changed in order to save the game.
                IsPauseMenuActive = false;
                ScreenManager.Pop();
                _isGameScreenActive = false;
                ScreenManager.Pop();
                MediaPlayer.Stop();
                HasStoppedMenuMusic = true;
                ScreenManager.Pop();
                IsMainMenuActive = true;
                _achievementMenu.SetAchievements(_achievements);
                _achievementMenu.SaveAchievementsToFile(_achievementsFilePath);
                _statisticMenu.SetStatistics(_statistics);
                _statisticMenu.SaveStatisticsToFile(_statisticsFilePath);
                break;
        }
    }

    private void HandleLossOrWonMenu(int lost)
    {
        ScreenManager.Pop();
        ScreenManager.Pop();
        _isGameScreenActive = false;
        IsEndBossActive = false;
        _achievementMenu.SetAchievements(_achievements);
        _achievementMenu.SaveAchievementsToFile(_achievementsFilePath);
        _statisticMenu.SetStatistics(_statistics);
        _statisticMenu.SaveStatisticsToFile(_statisticsFilePath);
        if (lost == 0)
        {
            // push lost game screen.
            ScreenManager.Push(new LostGameScreen(new LostGameMenu(_assetManager, _inputManager, _graphicsDevice, _statisticMenu)));
            _isLossMenuActive = true;
        }
        else
        {
            // push won screen.
            ScreenManager.Push(new WonGameScreen(new WonGameMenu(_assetManager, _inputManager, _graphicsDevice, _statisticMenu), _graphicsDevice));
            _isWonMenuActive = true;
        }
        IsMainMenuActive = false;
        IsSubMenuActive = true;

        if (!_inputManager.IsKeyPressed(Keys.Enter) &&
            !_inputManager.IsButtonPressed(Buttons.Start, PlayerIndex.One) &&
            !_inputManager.IsButtonPressed(Buttons.Start, PlayerIndex.Two)) return;

        _isLossMenuActive = false;
        HasStoppedMenuMusic = false;
        _isWonMenuActive = false;
        ScreenManager.Pop();
        IsMainMenuActive = true;
        IsSubMenuActive = false;
    }
        

    private void HandleMainMenuSelection(int selectedButton)
    {
        switch (selectedButton)
        {
            case 0:
                StartNewGame(true);
                break;
            case 1:
                ScreenManager.Push(new SaveGameScreen(new SaveGameMenu(_assetManager, _inputManager,
                    _graphicsDevice, ESaveOperationType.LoadGame, this)));
                IsMainMenuActive = false;
                IsSubMenuActive = true;
                break;
            case 2:
                StartTechDemo();
                break;
            case 3:
                _statisticMenu.SetStatistics(_statistics);
                ScreenManager.Push(new StatisticScreen(_statisticMenu));
                IsMainMenuActive = false;
                IsSubMenuActive = true;
                break;
            case 4:
                _achievementMenu.SetAchievements(_achievements);
                ScreenManager.Push(new AchievementMenuScreen(_achievementMenu));
                IsMainMenuActive = false;
                IsSubMenuActive = true;
                break;
            case 5:
                ScreenManager.Push(new SettingsMenuScreen(new SettingsMenu(_assetManager, _inputManager, _graphicsDevice, GameControl)));
                IsMainMenuActive = false;
                IsSubMenuActive = true;
                break;
            case 6:
                _exitGame = true;
                break;
        }
    }

    private void HandleSubMenuReturn(int selectedButton)
    {
        if (selectedButton != 0) return;
        ScreenManager.Pop();
        if (_isTechdemoActive)
        {
            ScreenManager.Pop();
            _isTechdemoActive = false;
        }
        
        if (IsSaveGameMenuActive)
        {
            IsPauseMenuActive = true;
            IsSaveGameMenuActive = false;
            IsMainMenuActive = false;
        }
        else
        {
            IsMainMenuActive = true;
        }
        IsSubMenuActive = false;
        _isLossMenuActive = false;
        _isWonMenuActive = false;
        
        if (_keyBindingStarted)
        {
            IsSubMenuActive = true;
            IsMainMenuActive = false;
            _keyBindingStarted = false;
        }
    }

    public void StartNewGame(bool isFirstGame)
    {
        
        CreateNewGame(isFirstGame);
        ScreenManager.Push(new GameScreen(GameControl));
        _hudScreen = new HudScreen(_health, _healthBars, new Abilities(_assetManager,
            _graphicsDevice), Timer, Potion ,GameControl.PlayerOffensive, GameControl.PlayerDefensive, GameControl.PlayerDefensive.Shield, _assetManager);
        ScreenManager.Push(_hudScreen);
        IsMainMenuActive = false;
        _isGameScreenActive = true;
        _isGameOn = true;
    }

    private void CreateNewGame(bool isFirstGame)
    {
        IsEndBossActive = false;
        IsEndBossMapLoaded = false;
        if (isFirstGame)
        {
            Timer = new Timer(_assetManager);

        }
        GameControl = new GameControl(this);
        _health = new Health(_assetManager);
        _healthBars = new Healthbars();
        GameControl.Initialize(_graphicsDevice, _content, _assetManager, _inputManager, _mapManager, ref _statistics, ref _achievements,
            _health, _healthBars, RenderDestination, _grid, Potion);
    }

    private void StartTechDemo()
    {
        _health = new Health(_assetManager);
        _healthBars = new Healthbars();
        _techDemo = new TechDemo(_graphicsDevice, _content, _assetManager, _inputManager,
            new Health(_assetManager),
            _healthBars, Potion, ref _statistics, ref _achievements);
        ScreenManager.Push(new TechDemoScreen(_techDemo));
        ScreenManager.Push(new HudScreen(_health, _healthBars, new Abilities(_assetManager,
            _graphicsDevice), Timer, Potion, _techDemo.Players[1], _techDemo.Players[0], _techDemo.Players[0].Shield, _assetManager));
        IsMainMenuActive = false;
        IsSubMenuActive = true;
        _isTechdemoActive = true;
        IsEndBossActive = false;
    }

    public void StartEndBossMap(bool pressedButtonM)
    {
        var hud = ScreenManager.Pop();
        ScreenManager.Pop();
        ScreenManager.Push(new EndBossScreen(new EndBossHandle(_graphicsDevice, _assetManager, _inputManager, _mapManager, _healthBars, Potion, ref _statistics, ref _achievements, GameControl.PlayerOffensive, GameControl.PlayerDefensive, Timer, pressedButtonM)));
        IsMainMenuActive = false;
        ScreenManager.Push(hud);
        IsEndBossMapLoaded = true;

    }
    private void StartKeybindingsScreen()
    {
        ScreenManager.Push(new KeybindingsScreen(_assetManager, _inputManager, _graphicsDevice));
        IsMainMenuActive = false;
        _isGameOn = false;
        _keyBindingStarted = true;

    }
    
    private void HandleBackgroundMusic()
    {
        if ((_isGameScreenActive || _isTechdemoActive) && !IsEndBossActive)
        {
            PlaySong(_backgroundMusic);
        }
        else if (_isLossMenuActive)
        {
            PlaySong(_lostGameSong);
        }
        else if (_isWonMenuActive)
        {
            PlaySong(_wonGameSong);
        }else if (IsEndBossActive)
        {
            PlaySong(_endBossSong);
        }
        else if (IsMainMenuActive || IsSubMenuActive)
        {
            PlaySong(_menuSong);
        }
        if (IsPauseMenuActive)
        {
            MediaPlayer.Pause();
        }

        // music is looping.
        if (MediaPlayer.State == MediaState.Stopped)
        {
            PlaySong(_currentSong, true);
        }
    }

    private void PlaySong(Song song, bool wasPaused = false)
    {
        if (_currentSong != song || wasPaused)
        {
            _currentSong = song;
            MediaPlayer.Play(song);
        }
    }

    /// <summary>
    /// Pauses the media player
    /// </summary>
    public void PauseSong()
    {
        MediaPlayer.Pause();
    }

    /// <summary>
    /// Resume the media player
    /// </summary>
    public void ResumeSong()
    {
        MediaPlayer.Resume();
    }
}