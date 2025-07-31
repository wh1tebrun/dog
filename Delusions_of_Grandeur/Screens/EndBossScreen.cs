using Microsoft.Xna.Framework;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class EndBossScreen : IScreen
{
    public EndBossHandle EndBossHandle { get; set; }

    public bool UpdateLower => false;
    public bool DrawLower => false;

    public EndBossScreen(EndBossHandle endBossHandle)
    {
        EndBossHandle = endBossHandle;
    }

    public void LoadContent()
    {
        EndBossHandle.LoadContent();
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
        return EndBossHandle.Update(gameTime);
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        EndBossHandle.Draw(gameTime, spriteBatch);
    }
}