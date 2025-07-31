using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Hausaufgabe1;

public class Game1 : Game
{
    private Texture2D _background;
    private Texture2D _unilogo;
    private SoundEffect _clickOnLogoSound;
    private SoundEffect _clickOutsideSound;
    
    private float _rotation;
    private float _radius;
    private float _logoScale = 0.15f;
    private Vector2 _logoPosition;
    private Vector2 _logoOrigin;
    private float _logoRadius;
    
    // the origin of the window
    private float _xWindow;
    private float _yWindow;

    private int _windowWidth;
    private int _windowHeight;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        _rotation = 0f;
        _radius = 160f;
        
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _background = Content.Load<Texture2D>("Background");
        _unilogo = Content.Load<Texture2D>("Unilogo");
        
        var viewport = GraphicsDevice.Viewport;
        _xWindow = viewport.Width / 2;
        _yWindow = viewport.Height / 2;

        _windowWidth = viewport.Width;
        _windowHeight = viewport.Height;
        
        _logoPosition = new Vector2(_xWindow + _radius * (float)Math.Cos(_rotation),_yWindow + _radius * (float)Math.Sin(_rotation));
        _logoOrigin = new Vector2(_unilogo.Width / 2, _unilogo.Height / 2);
        _logoRadius = _logoOrigin.X;
        
        _clickOnLogoSound = Content.Load<SoundEffect>("Logo_hit");
        _clickOutsideSound = Content.Load<SoundEffect>("Logo_miss");
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _rotation += (float)(Math.PI / 180);
        _logoPosition = new Vector2(_xWindow + _radius * (float)Math.Cos(_rotation),_yWindow + _radius * (float)Math.Sin(_rotation));

        // Checks if the left mouse button was clicked, if so you can hear a sound depending on where you clicked.
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            if (IsMouseInsideLogo(mousePosition))
            {
                _clickOnLogoSound.Play();
            }
            else
            {
                _clickOutsideSound.Play();
            }
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        
        
        _spriteBatch.Draw(_background, new Rectangle(0, 0, _windowWidth, _windowHeight), Color.White);
        
        _spriteBatch.Draw(_unilogo, _logoPosition, null, Color.White, 0f, _logoOrigin, _logoScale, SpriteEffects.None, 0f);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    // measures the distance between the mouse click and the position of the logo. If it is smaller than
    // the logo scale, the click is inside the logo.
    private bool IsMouseInsideLogo(Vector2 mousePosition)
        {
            float distance = Vector2.Distance(mousePosition, _logoPosition);
            float scaledLogoRadius = _logoRadius * _logoScale;
            return distance <= scaledLogoRadius;
        }
}