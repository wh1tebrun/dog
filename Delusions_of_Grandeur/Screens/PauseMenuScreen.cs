#region File Description
// PauseMenuScreen.cs
#endregion

using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class PauseMenuScreen : IScreen
{
    private readonly Menu.PauseMenu _pauseMenu;
    public bool UpdateLower => false;
    public bool DrawLower => true;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PauseMenuScreen(Menu.PauseMenu pauseMenu)
    { 
        _pauseMenu = pauseMenu;
    }

    /// <summary>
    /// Loads the content.
    /// </summary>
    public void LoadContent()
    {
        
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
         return _pauseMenu.Update();
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
    {
        _pauseMenu.Draw(spriteBatch);
    }
}
