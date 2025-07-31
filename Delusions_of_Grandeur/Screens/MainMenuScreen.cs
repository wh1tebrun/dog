#region File Description
// MainMenuScreen.cs
// Screen for updating and displaying the main-menu screen.
#endregion

using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class MainMenuScreen : IScreen
{
    private readonly Menu.MainMenu _mainMenu;
    public bool UpdateLower => false;
    public bool DrawLower => false;
    
    /// <summary>
    /// constructor.
    /// </summary>
    public MainMenuScreen(Menu.MainMenu mainMenu)
    { 
        _mainMenu = mainMenu;
    }

    /// <summary>
    /// loads the content with the game control.
    /// </summary>
    public void LoadContent()
    {
        _mainMenu.LoadContent();
    }

    /// <summary>
    /// unloads content. Not needed yet.
    /// </summary>
    public void UnloadContent()
    {
        
    }
    
    /// <summary>
    /// updates the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    public int Update(GameTime gameTime)
    {
        return _mainMenu.Update(gameTime);
    }

    /// <summary>
    /// draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
    {
        _mainMenu.Draw(spriteBatch);
    }
}