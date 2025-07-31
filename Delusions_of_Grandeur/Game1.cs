# region File Description
// AssetManager.cs
// Manage the Assets for the game.
#endregion

using System;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly ScreenManager _screenManager;
    private readonly InputManager _inputManager;
    private readonly AssetManager _assetManager;
    private Pathfinding.Grid _grid;
    private MapManager _mapManager;

    private const int NativeResolutionWidth = 1920;
    private const int NativeResolutionHeight = 1080;

    private RenderTarget2D _renderTarget;
    private Rectangle _renderDestination;
    private bool _isResizing;

    private bool _previousIsActive;

    public MenuControl MenuControl { get; set; }
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _assetManager = new AssetManager();
        _inputManager = new InputManager();
        _screenManager = new ScreenManager(GraphicsDevice);
        _previousIsActive = true;
        // This Method is called every time the windows size changes.
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;

        _graphics.PreferredBackBufferWidth = NativeResolutionWidth;
        _graphics.PreferredBackBufferHeight = NativeResolutionHeight;
        _graphics.ApplyChanges();

        Window.AllowUserResizing = true;

        _mapManager = new MapManager(GraphicsDevice, Content);
        _grid = new Pathfinding.Grid(GraphicsDevice, _mapManager);
        _assetManager.LoadFonts(Content);
        _assetManager.LoadTextures(Content, GraphicsDevice);
        _assetManager.LoadMusic(Content);
        _assetManager.LoadSounds(Content);
        // _assetManager.LoadShaders(Content);
        MenuControl = new MenuControl(_screenManager, _assetManager, _inputManager, _mapManager, GraphicsDevice, _renderDestination, _grid);
        MenuControl.Initialize(Content, _graphics);

        base.Initialize();
        CalculateRenderDestination();
    }

    /// <summary>
    /// Load content for the game.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderTarget = new RenderTarget2D(GraphicsDevice, NativeResolutionWidth, NativeResolutionHeight);

        MenuControl.Initialize(Content, _graphics);
        MenuControl.StartProgram();
    }

    /// <summary>
    /// Update the main game logic.
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        if (IsActive && !_previousIsActive)
        {
            MenuControl.ResumeSong();
        }
        else if (!IsActive && _previousIsActive)
        {
            MenuControl.PauseSong();
        }
        _previousIsActive = IsActive;

        if (!IsActive) return;
        _inputManager.Update();

        var exitGame = MenuControl.Update(gameTime);
        if (exitGame)
        {
            _assetManager.UnloadTextures();
            _assetManager.UnloadMusic();
            _assetManager.UnloadSounds();
            Exit();
        }

        MenuControl.RenderDestination = _renderDestination;

        base.Update(gameTime);
    }

    /// <summary>
    /// Handle draw calls for the game.
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Transparent);

        GraphicsDevice.SetRenderTarget(_renderTarget); //Draw the following to the render target.

        MenuControl.Draw(gameTime, _spriteBatch);
        base.Draw(gameTime);

        GraphicsDevice.SetRenderTarget(null); //Finished with drawing to the render target.

        //Draw the render target to the screen
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, _renderDestination, Color.White);
        _spriteBatch.End();
    }

    private void CalculateRenderDestination()
    {
        var size = GraphicsDevice.Viewport.Bounds.Size;
        //find out the scale to the borders
        var scaleX = (float)size.X / _renderTarget.Width;
        var scaleY = (float)size.Y / _renderTarget.Height;
        var scale = Math.Min(scaleX, scaleY);
        //scales the RRenderDestination rectangle 
        _renderDestination.Width = (int)(_renderTarget.Width * scale);
        _renderDestination.Height = (int)(_renderTarget.Height * scale);

        //Center the game screen in the window
        _renderDestination.X = (size.X - _renderDestination.Width) / 2;
        _renderDestination.Y = (size.Y - _renderDestination.Height) / 2;
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        // Windows size is not constantly calculating, only on the last time window size is changed
        if (_isResizing || Window.ClientBounds.Width <= 0 || Window.ClientBounds.Height <= 0) return;
        _isResizing = true;
        CalculateRenderDestination();
        _isResizing = false;
    }
}
