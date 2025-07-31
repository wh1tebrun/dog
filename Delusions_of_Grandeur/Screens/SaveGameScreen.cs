#region File Description
// SaveGameScreen.cs
#endregion

using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class SaveGameScreen : IScreen
{
    private readonly Menu.SaveGameMenu _saveGameMenu;
    public bool UpdateLower => false;
    public bool DrawLower => true;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SaveGameScreen(Menu.SaveGameMenu saveGameMenu)
    {
        _saveGameMenu = saveGameMenu;
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
        return _saveGameMenu.Update();
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _saveGameMenu.Draw(spriteBatch);
    }
}
