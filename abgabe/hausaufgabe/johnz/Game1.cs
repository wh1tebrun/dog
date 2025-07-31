using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hausaufgabe;

public class Game1 : Game
{

    private Texture2D _backgroundTexture;
    private Texture2D _uniLogoTexture;
    private Vector2 _uniLogoPosition;
    private Vector2 _screenCenter;
    private float _angle;
    private float _rotationRadius;
    private float _uniLogoRadius;
    private float _uniLogoScale;
    private MouseState _mouse;
    private Vector2 _mousePosition;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SoundEffect _soundEffectHit;
    private SoundEffect _soundEffectMiss;
    private Vector2 _window;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // _rotationRadius of the circle the rotating element describes.
        _rotationRadius = Math.Min(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 5f;
        // Starting point of the rotating element.
        _angle = 0f;
        // Get the dimensions of the content window.
        _window = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // Load the background image and the Uni Logo image
        _backgroundTexture = Content.Load<Texture2D>("background");
        _uniLogoTexture = Content.Load<Texture2D>("unilogo");
        // The center of the screen will be the rotation point.
        _screenCenter = new Vector2((float)GraphicsDevice.Viewport.Width / 2, (float)GraphicsDevice.Viewport.Height / 2);
        
        // Determine the radius of the unilogo.
        _uniLogoScale = 0.1f;
        _uniLogoRadius = (float)_uniLogoTexture.Width / 2 * _uniLogoScale;
        
        _soundEffectHit = Content.Load<SoundEffect>("logo_hit");
        _soundEffectMiss = Content.Load<SoundEffect>("logo_miss");
        
    }

    protected override void Update(GameTime gameTime)
    {

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        // Update the _angle of the rotating element to the screen center (rotation point).
        _angle += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update the position of the rotating element using the circle equation
        _uniLogoPosition = new Vector2(
            _screenCenter.X + _rotationRadius * (float)Math.Cos(_angle),
            _screenCenter.Y + _rotationRadius * (float)Math.Sin(_angle)
        );

        // Read the mouse coordinates.
        _mouse = Mouse.GetState();
        _mousePosition = new Vector2(_mouse.X, _mouse.Y);
        // Console.WriteLine("X: " + mouse.X + ", Y: " + mouse.Y);
        
        // The mouse is inside the circle (uniLogo) if the distance of the unilogo center and the mouse position
        // is smaller than the unilogo radius.
        var isInsideCircle = Vector2.Distance(_uniLogoPosition, _mousePosition) <= _uniLogoRadius;
        
        // Check if mouse is clicked.
        var isMouseInWindow = 0 <= _mousePosition.X
            && _mousePosition.X <= _window.X
            && 0 <= _mousePosition.Y
            && _mousePosition.Y <= _window.Y;
        var isMouseClicked = Mouse.GetState().LeftButton == ButtonState.Pressed;
        
        // Play Hit sound if logo is clicked and Miss sound if background is clicked.
        if (isMouseInWindow &&isMouseClicked)
        {
            if (isInsideCircle)
            {
                _soundEffectHit.Play();
            }
            else
            {
                _soundEffectMiss.Play();
            }
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        // Draw the background texture, then draw the unilogo texture
        _spriteBatch.Begin();
        _spriteBatch.Draw(
            _backgroundTexture,
            new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            Color.White);
        _spriteBatch.Draw(
            _uniLogoTexture,
            _uniLogoPosition,
            null,
            Color.White,
            0f,
            new Vector2((float)_uniLogoTexture.Width / 2, (float)_uniLogoTexture.Height / 2),
            _uniLogoScale,
            SpriteEffects.None,
            0f
            );
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}