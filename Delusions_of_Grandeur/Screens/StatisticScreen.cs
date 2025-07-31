using System;
using System.IO;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class StatisticScreen : IScreen
{
    private readonly Menu.StatisticMenu _statisticMenu;
    public bool UpdateLower => false;
    public bool DrawLower => false;
    
    private readonly string _filePath;

    /// <summary>
    /// Constructor.
    /// </summary>
    public StatisticScreen(Menu.StatisticMenu statisticMenu)
    {
        _statisticMenu = statisticMenu;
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _filePath = Path.Combine(folderPath,  "statistics.json");
    }

    /// <summary>
    /// Loads the content.
    /// </summary>
    public void LoadContent()
    {
        if (File.Exists(_filePath))
        {
            _statisticMenu.LoadStatisticsFromFile(_filePath);
        }
        else
        {
            _statisticMenu.SaveStatisticsToFile(_filePath);
        } 
    }

    /// <summary>
    /// Unloads content.
    /// </summary>
    public void UnloadContent()
    {
        _statisticMenu.SaveStatisticsToFile(_filePath);
    }

    /// <summary>
    /// Updates the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    public int Update(GameTime gameTime)
    {
        return _statisticMenu.Update();
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _statisticMenu.Draw(spriteBatch);        
    }
}
