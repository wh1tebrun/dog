#region File Description
// AchievementMenu.cs
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Text.Json;

namespace Delusions_of_Grandeur.Menu;


/// <summary>
/// a class for all the achievements. Stores them and updates the achievements.
/// </summary>
public class AchievementMenu
{
    private readonly List<Button> _buttons;
    private readonly InputManager _inputManager;
    private readonly Texture2D _background;
    private readonly Texture2D _buttonBackground;
    private List<Achievement> _achievements;

    /// <summary>
    /// initializes all achievements, the buttons for the achievements and the Background.
    /// </summary>
    public AchievementMenu(AssetManager assetManager, InputManager inputManager, GraphicsDevice graphics, List<Achievement> achievements)
    {
        _inputManager = inputManager;
        _achievements = [];
        _achievements = achievements;

        _buttons = [];

        var yOffset = 250;
        foreach (var achievement in achievements)
        {
            _buttons.Add(new Button(new Vector2(200, yOffset), assetManager, achievement, _inputManager, new Vector2(10, 10)));
            yOffset += 100;
        }
        _buttons.Add(new Button("Back", new Vector2(200, yOffset + 50), assetManager, _inputManager, new Vector2 (80, 50)));

        _background = assetManager.GetTexture("background");
        _buttonBackground = new Texture2D(graphics, 1, 1);
        _buttonBackground.SetData([Color.DarkSlateGray]);
    }

    /// <summary>
    /// Updates if the back button was clicked, returns 0 if yes, else -1.
    /// </summary>
    /// <returns></returns>
    public int Update()
    {
        _buttons[5].IsSelected = true;

        if (_buttons[5].MouseContained() && _inputManager.IsLeftMousePressed())
        {
            _buttons[5].IsClicked = true;
            return 0;
        }
        
        if (!_inputManager.IsKeyPressed(Keys.Escape) && !_inputManager.IsKeyPressed(Keys.Enter) && !_inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One) && !_inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One)) return -1;
        _buttons[5].IsClicked = true;
        return 0;
    }

    /// <summary>
    /// draws the achievement buttons.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        spriteBatch.Draw(_buttonBackground, new Rectangle(100, 230, 1400, 650), new Color(Color.Aqua, 0.5f));
        for (var i = 0; i < _buttons.Count; i++)
        {
            var button = _buttons[i];
            if (i != 5)
            {
                var achievement = _achievements[i];
                if (achievement.Completed)
                {
                    button.Draw(spriteBatch, 1);
                }
                else
                {
                    button.Draw(spriteBatch);
                }
            }
            else
            {
                button.Draw(spriteBatch);
            }
        }

        spriteBatch.End();
    }
    

    /// <summary>
    /// sets the current value for all achievements and checks if an achievement is completed.
    /// </summary>
    /// <param name="achievements"></param>
    public void SetAchievements(List<Achievement> achievements)
    {
        for (var i = 0; i < _achievements.Count; i++)
        {
            _achievements[i].CurrentValue += achievements[i].CurrentValue;
        }
        // checks if achievement is completed.
        foreach (var achievement in _achievements)
        {
            achievement.IncreaseValue(0);
        }
    }

    /// <summary>
    /// serializes achievements into a json file.
    /// </summary>
    /// <param name="filePath"></param>
    public void SaveAchievementsToFile(string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_achievements, options);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// loads and deserializes the data for the achievements from json file.
    /// </summary>
    /// <param name="filePath"></param>
    public void LoadAchievementsFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return;
        var json = File.ReadAllText(filePath);
        _achievements = JsonSerializer.Deserialize<List<Achievement>>(json);
    }
}
