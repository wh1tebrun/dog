using System;
using System.Collections.Generic;
using System.Linq;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class WonGameScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Menu.WonGameMenu _wonGameMenu;
    private Confetti _confetti;
    private readonly Random _random;

    private TimeSpan _confettiTimer;
    private readonly TimeSpan _confettiInterval;

    public bool UpdateLower => false;
    public bool DrawLower => false;

    /// <summary>
    /// Constructor.
    /// </summary>
    public WonGameScreen(Menu.WonGameMenu wonGameMenu, GraphicsDevice graphicsDevice)
    {
        _wonGameMenu = wonGameMenu;
        _graphicsDevice = graphicsDevice;
        _random = new Random();
        _confettiTimer = TimeSpan.Zero;
        _confettiInterval = TimeSpan.FromSeconds(1); // Change interval as needed
    }

    /// <summary>
    /// Loads the content.
    /// </summary>
    public void LoadContent()
    {
        var textures = new List<Color>
            {
                new(90, 185, 235),
                new(255, 104, 126),
                new(190, 135, 164)
            }.Select(c => Confetti.Rectangle(_graphicsDevice, 10, 18, c))
            .ToList().AsReadOnly();

        _confetti = new Confetti(textures);
    }

    /// <summary>
    /// Unloads content. Not needed yet.
    /// </summary>
    public void UnloadContent()
    {
    }

    /// <summary>
    /// Updates the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    public int Update(GameTime gameTime)
    {
        _confetti.Update(gameTime);
        
        _confetti.Sprinkle(3, new Rectangle(0, -10, Consts.ScreenWidth, 10));
        
        _confettiTimer += gameTime.ElapsedGameTime;
        if (_confettiTimer >= _confettiInterval)
        {
            var randomX = _random.Next(100, Consts.ScreenWidth - 100);
            var randomY = _random.Next(100, Consts.ScreenHeight - 100);

            _confetti.Explode(20, new Vector2(randomX, randomY));

            _confettiTimer = TimeSpan.Zero;
        }

        return _wonGameMenu.Update();
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _wonGameMenu.Draw(spriteBatch);
        _confetti.Draw(spriteBatch);
    }
}