using System;
using System.IO;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class AchievementMenuScreen : IScreen
{
    private readonly Menu.AchievementMenu _achievementMenu;
    public bool UpdateLower => false;
    public bool DrawLower => false;
    
    private readonly string _filePath;

    /// <summary>
    /// Constructor.
    /// </summary>
    public AchievementMenuScreen(Menu.AchievementMenu achievementMenu)
    { 
        _achievementMenu = achievementMenu;
        var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _filePath = Path.Combine(folderPath,  "achievements.json");
    }

    /// <summary>
    /// Loads the content.
    /// </summary>
    public void LoadContent()
    {
        if (File.Exists(_filePath))
        {
            _achievementMenu.LoadAchievementsFromFile(_filePath);
        }
        else
        {
            _achievementMenu.SaveAchievementsToFile(_filePath);
        }
    }

    /// <summary>
    /// Unloads content. Achievements are serialized in json file.
    /// </summary>
    public void UnloadContent()
    {
        _achievementMenu.SaveAchievementsToFile(_filePath);
    }

    /// <summary>
    /// Updates the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    public int Update(GameTime gameTime)
    {
        return _achievementMenu.Update();
    }

    /// <summary>
    /// draws the screen.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="spriteBatch"></param>
    public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
    {
        _achievementMenu.Draw(spriteBatch);
    }
}
