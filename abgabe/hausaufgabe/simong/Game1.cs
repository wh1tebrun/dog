using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Hausaufgabe;


public class Game1 : Game
{
    private Texture2D _backgroundTexture;
    private Texture2D _logoTexture;
    private Vector2 _logoPosition;
    private float _logoRadius;
    private float _logoSpeed;
    private Vector2 _logoOrigin;
    private List<SoundEffect> _soundEffects;
    private MouseState _ms, _oldms;
    private Vector2 _backgroundCenter;
    private double _angle;
    private SoundEffect _logoHit;
    private SoundEffect _logoMiss;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 1024;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _soundEffects = new List<SoundEffect>();
        
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _logoPosition = new Vector2(GraphicsDevice.Viewport.Width / 2f - 128, 150);
        _logoSpeed = 100f;
        _backgroundCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        
        

        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _backgroundTexture = Content.Load<Texture2D>("Background");
        _logoTexture = Content.Load<Texture2D>("Unilogo");
        _soundEffects.Add(Content.Load<SoundEffect>("Logo_hit"));
        _soundEffects.Add(Content.Load<SoundEffect>("Logo_miss"));
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        _logoRadius = (_logoTexture.Width / 2.0f) * 0.25f;
        _logoOrigin = _logoPosition + new Vector2(_logoTexture.Width / 2 * 0.25f, _logoTexture.Height / 2 * 0.25f);
        
        MouseState mouseState = Mouse.GetState();
        float vectorToOrigin = Vector2.Distance(_logoOrigin, new Vector2(mouseState.X, mouseState.Y));

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            if (vectorToOrigin <= _logoRadius)
            {
                _soundEffects[0].Play();
            }
            else
            {
                _soundEffects[1].Play();
            }

        }
        
        
        
        
        _logoPosition.X = _backgroundCenter.X - _logoRadius + (float)Math.Cos(_angle) * 200f;
        _logoPosition.Y = _backgroundCenter.Y - _logoRadius + (float)Math.Sin(_angle) * 200f;

        _angle += (float)gameTime.ElapsedGameTime.TotalSeconds;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.Draw(_backgroundTexture, new Vector2(0, 0), Color.White);
        _spriteBatch.Draw(_logoTexture,
            _logoPosition,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            0.25f,
            SpriteEffects.None,
            0f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}