using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hausaufgabe;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _logo;
    private Texture2D _background;
    private Vector2 _logoPosition;
    private float _logoScale;
    private Vector2 screenCenter;
    private double _rotationAngle;
    private const float rotationRadius = 150f;
    private SoundEffect _logoHitSound;
    private SoundEffect _logoMissSound;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Set window dimension.
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 1024;
        _graphics.ApplyChanges();

        _logoScale = 0.2f;
        _logoPosition = new Vector2(GraphicsDevice.Viewport.Width / 2f - 128, 150);
        screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _logo = Content.Load<Texture2D>("Unilogo");
        _background = Content.Load<Texture2D>("Background");
        _logoHitSound = Content.Load<SoundEffect>("Logo_hit");
        _logoMissSound = Content.Load<SoundEffect>("Logo_miss");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var mouseState = Mouse.GetState();

        float logoRadius = _logo.Width / 2.0f * _logoScale;

        // Calculate the center of the logo.
        Vector2 logoCenter = _logoPosition + new Vector2(_logo.Width / 2f * _logoScale, _logo.Height / 2f * _logoScale);
        float distanceMouseLogo = Vector2.Distance(logoCenter, new Vector2(mouseState.X, mouseState.Y));

        bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;

        if (isMousePressed)
        {
            if (distanceMouseLogo <= logoRadius)
            {
                _logoHitSound.Play();
            }
            else
            {
                _logoMissSound.Play();
            }
        }

        // Rotate the logo around the center.
        _logoPosition.X = screenCenter.X - logoRadius + (float)Math.Cos(_rotationAngle) * rotationRadius;
        _logoPosition.Y = screenCenter.Y - logoRadius + (float)Math.Sin(_rotationAngle) * rotationRadius;

        _rotationAngle += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, new Vector2(0, 0), Color.White);
        _spriteBatch.Draw(_logo, _logoPosition, null,
            Color.White, 0f, Vector2.Zero, _logoScale, SpriteEffects.None, 0f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
