using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

/// <summary>
/// A class for the game screen. 
/// </summary>
public class GameScreen : IScreen
{
    public bool UpdateLower => false;
    public bool DrawLower => false;
    private readonly GameControl _gameControl;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="gameControl"></param>
    public GameScreen(GameControl gameControl)
    { 
       _gameControl = gameControl;
    }

    /// <summary>
    /// Loads the content with the game control.
    /// </summary>
    public void LoadContent()
    {
        _gameControl.LoadContent();
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
        return _gameControl.Update(gameTime);
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
    {
        _gameControl.Draw(gameTime, spriteBatch);
    }
}