# region File Description
// ScreenManager.cs
// Manage the screens for the game.
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Manager;

/// <summary>
/// Manages the different screens.
/// </summary>
public class ScreenManager
{
    public Stack<IScreen> ScreenStack { get; set; } = new();
    private readonly GraphicsDevice _graphicsDevice;

    public ScreenManager(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice; 
    }
    
    /// <summary>
    /// Updates the current screen.
    /// </summary>
    /// <param name="gameTime"></param>
    public int Update(GameTime gameTime)
    {
        if (ScreenStack.Count <= 0) return -1;
        var currentScreen = ScreenStack.Peek();

        var below = -1;
        // If UpdateLower is true, updates also the Screen below current screen.
        if (currentScreen.UpdateLower && ScreenStack.Count > 1)
        { 
            var screensArray = ScreenStack.ToArray();
            IScreen belowScreen = screensArray[ScreenStack.Count - 2];
            if (belowScreen.UpdateLower && ScreenStack.Count > 1)
            { 
                var screensArray2 = ScreenStack.ToArray();
                IScreen belowScreen2 = screensArray2[ScreenStack.Count - 2];
                belowScreen2.Update(gameTime); 
            }
            below = belowScreen.Update(gameTime); 
        }
        // int current = currentScreen.Update(gameTime);
        if (currentScreen is not HudScreen) return currentScreen.Update(gameTime);
        currentScreen.Update(gameTime);
        return below;
    }
    
    /// <summary>
    /// Draws the current screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (ScreenStack.Count <= 0) return;
        IScreen currentScreen = ScreenStack.Peek();

        // Draw screen below current screen if DrawLower is true.
        if (currentScreen.DrawLower && ScreenStack.Count > 1)
        {
            var belowScreen = ScreenStack.ToArray()[1];
            if (belowScreen.DrawLower && ScreenStack.Count > 2)
            {
                ScreenStack.ToArray()[2]?.Draw(gameTime, spriteBatch);
            }

            belowScreen.Draw(gameTime, spriteBatch);
        }

        currentScreen.Draw(gameTime, spriteBatch);
    }

    /// <summary>
    /// Pushes a new screen on the stack.
    /// </summary>
    /// <param name="screen"></param>
    public void Push(IScreen screen)
    {
        ScreenStack.Push(screen);
        screen.LoadContent();
    }

    /// <summary>
    /// Pops the first screen on stack.
    /// </summary>
    /// <returns> screen </returns>
    public IScreen Pop()
    {
        IScreen removedScreen = ScreenStack.Pop();
        removedScreen.UnloadContent();
        return removedScreen;
    }
}

/// <summary>
/// An interface for one screen.
/// </summary>
public interface IScreen
{
    bool UpdateLower { get; }
    bool DrawLower { get; }
    void LoadContent();
    void UnloadContent();
    int Update(GameTime gameTime);
    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}