using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur.Menu;

public class StatisticMenu
{
    public List<Statistic> Statistics;
    private readonly InputManager _inputManager;
    private readonly AssetManager _assetManager;
    private readonly List<Button> _buttons;
    private readonly Texture2D _background;
    private readonly Texture2D _buttonBackground;

    public StatisticMenu(AssetManager assetManager, InputManager inputManager, List<Statistic> statistics, GraphicsDevice graphics)
    {
        Statistics = statistics;
        _inputManager = inputManager;
        _assetManager = assetManager;
        _buttons = [];
        
        var yOffset = 250;
        foreach (var statistic in Statistics)
        {
            _buttons.Add(new Button(new Vector2(200, yOffset), assetManager, statistic, _inputManager, new Vector2(10, 10)));
            yOffset += 100;
        }
        _buttons.Add(new Button("Back", new Vector2(200, yOffset + 50), assetManager, _inputManager, new Vector2 (80, 50)));
        
        _background = _assetManager.GetTexture("background");
        _buttonBackground = new Texture2D(graphics, 1, 1);
        _buttonBackground.SetData([Color.DarkSlateGray]);
    }
    
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

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        spriteBatch.Draw(_buttonBackground, new Rectangle(100, 230, 1400, 650), new Color(Color.Aqua, 0.5f));
        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }
        spriteBatch.End();
    }
    
    
    /// <summary>
    /// sets the current value for all statistic.
    /// </summary>
    /// <param name="statistics"></param>
    public void SetStatistics(List<Statistic> statistics)
    {
        for (int i = 0; i < Statistics.Count; i++)
        {
            Statistics[i].Value += statistics[i].Value;
        }
    }
    
    /// <summary>
    /// serializes statistics into a json file.
    /// </summary>
    /// <param name="filePath"></param>
    public void SaveStatisticsToFile(string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(Statistics, options);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// loads and deserializes the data for the statistics from json file.
    /// </summary>
    /// <param name="filePath"></param>
    public void LoadStatisticsFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return;
        
        var json = File.ReadAllText(filePath);
        Statistics = JsonSerializer.Deserialize<List<Statistic>>(json);
        
        UpdateButtons();
    }
    
    /// <summary>
    /// updates the button, so that the value shown in the statistics menu is correct.
    /// </summary>
    private void UpdateButtons()
    {
        _buttons.Clear();
        var yOffset = 250;
        foreach (var statistic in Statistics)
        {
            _buttons.Add(new Button(new Vector2(200, yOffset), _assetManager, statistic, _inputManager, new Vector2(10, 10)));
            yOffset += 100;
        }
        _buttons.Add(new Button("Back", new Vector2(200, yOffset + 50), _assetManager, _inputManager, new Vector2 (100, 50)));
    }
}