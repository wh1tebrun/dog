using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hausaufgabe;

public class Game1 : Game
{
    private GraphicsDeviceManager _deviceManager;
    private SpriteBatch _canvas;
    private Texture2D _logoTexture;
    private Texture2D _backgroundTexture;
    private Vector2 _logoPosition;
    private float _logoScale;
    private Vector2 _screenCenter;
    private double _rotationAngle;
    private const float RotationRadius = 150f;
    private SoundEffect _correctClickSound;
    private SoundEffect _incorrectClickSound;

    public Game1()
    {
        _deviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _deviceManager.IsFullScreen = false;
        _deviceManager.PreferredBackBufferWidth = 1280;
        _deviceManager.PreferredBackBufferHeight = 1024;
        _deviceManager.ApplyChanges();

        _logoScale = 0.2f;
        _logoPosition = new Vector2(GraphicsDevice.Viewport.Width / 2f - 128, 150);
        _screenCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _canvas = new SpriteBatch(GraphicsDevice);
        _logoTexture = Content.Load<Texture2D>("Unilogo");
        _backgroundTexture = Content.Load<Texture2D>("Background");
        _correctClickSound = Content.Load<SoundEffect>("Logo_hit");
        _incorrectClickSound = Content.Load<SoundEffect>("Logo_miss");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var mouseState = Mouse.GetState();
        float logoCenterDistance = _logoTexture.Width / 2.0f * _logoScale;

        Vector2 logoCenter = _logoPosition + new Vector2(_logoTexture.Width / 2f * _logoScale, _logoTexture.Height / 2f * _logoScale);
        float distanceToLogo = Vector2.Distance(logoCenter, new Vector2(mouseState.X, mouseState.Y));

        bool isLeftClick = mouseState.LeftButton == ButtonState.Pressed;

        if (isLeftClick)
        {
            if (distanceToLogo <= logoCenterDistance)
            {
                _correctClickSound.Play();
            }
            else
            {
                _incorrectClickSound.Play();
            }
        }

        Vector2 logoOffset = new Vector2((float)Math.Cos(_rotationAngle), (float)Math.Sin(_rotationAngle)) * RotationRadius;
        _logoPosition = _screenCenter - new Vector2(logoCenterDistance, logoCenterDistance) + logoOffset;

        _rotationAngle += (float)gameTime.ElapsedGameTime.TotalSeconds;

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _canvas.Begin();
        _canvas.Draw(_backgroundTexture, Vector2.Zero, Color.White);
        _canvas.Draw(_logoTexture, _logoPosition, null, Color.White, 0f, Vector2.Zero, _logoScale, SpriteEffects.None, 0f);
        _canvas.End();

        base.Draw(gameTime);
    }
}
