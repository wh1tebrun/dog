#region File Description
// SettingsMenuScreen.cs
#endregion

using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class SettingsMenuScreen : IScreen
{
    private readonly Menu.SettingsMenu _settingsMenu;
    public bool UpdateLower => false;
    public bool DrawLower => false;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SettingsMenuScreen(Menu.SettingsMenu settingsMenu)
    { 
        _settingsMenu = settingsMenu;
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
        return _settingsMenu.Update();
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
    {
        _settingsMenu.Draw(spriteBatch);
    }
}
