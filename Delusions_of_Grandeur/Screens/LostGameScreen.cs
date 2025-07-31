using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class LostGameScreen : IScreen
{
    private readonly Menu.LostGameMenu _lostGameMenu;
    public bool UpdateLower => false;
    public bool DrawLower => false;
    /// <summary>
    /// Constructor.
    /// </summary>
    public LostGameScreen(Menu.LostGameMenu lostGameMenu)
    {
        _lostGameMenu = lostGameMenu;
    }

    /// <summary>
    /// Lads the content.
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
        return _lostGameMenu.Update();
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _lostGameMenu.Draw(spriteBatch);
    }
}