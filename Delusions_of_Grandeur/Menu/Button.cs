#region File Description
// Button.cs
// Represents a single button
#endregion

using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Menu;

public class Button
{
    // Boolean
    private bool _isSelected;
    private readonly bool _isClicked = false;
    private readonly bool _hasVariableLabelStatistic;
    private readonly bool _hasVariableLabelAchievement;
    
    // Utilities
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;

    public string Label;
    
    public readonly int Size;

    // Achievement/ Statistics
    private readonly Statistic _statistic;
    private readonly Achievement _achievement;

    private readonly SpriteFont _font;
    private readonly Texture2D _rightArrow;
    private readonly Texture2D _buttonSmall;
    private readonly Texture2D _buttonMedium;
    private readonly Texture2D _buttonLarge;
    private readonly Texture2D _buttonHuge;
    
    private readonly Vector2 _buttonField;
    
    public Vector2 Position { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (!_isSelected && value)
            {
                PlaySelectSound();
            }
            _isSelected = value;
        }
    }

    public bool IsClicked
    {
        set
        {
            if (!_isClicked && value)
            {
                PlayClickedSound();
            }
        }
    }

    public Button(string label, Vector2 position, AssetManager assetManager, InputManager inputManager, Vector2 buttonField, int size = 1)
    {
        _isSelected = false;
        Label = label;
        _assetManager = assetManager;
        Position = position;
        _font = _assetManager.GetFont("MainMenu");
        _rightArrow = _assetManager.GetTexture("arrowheads");
        _statistic = null;
        Size = size;
        _buttonSmall = _assetManager.GetTexture("sign_small");
        _buttonMedium = _assetManager.GetTexture("sign_medium");
        _buttonLarge = _assetManager.GetTexture("sign_big");
        _buttonHuge = _assetManager.GetTexture("sign_huge");
        _buttonField = buttonField;
        _inputManager = inputManager;
    }

    public Button(Vector2 position, AssetManager assetManager, Statistic statistic, InputManager inputManager, Vector2 buttonField)
    {
        _isSelected = false;
        _assetManager = assetManager;
        Position = position;
        _hasVariableLabelStatistic = true;
        _font = _assetManager.GetFont("MainMenu");
        _statistic = statistic;
        _inputManager = inputManager;
        _buttonField = buttonField;
    }

    public Button(Vector2 position, AssetManager assetManager, Achievement achievement, InputManager inputManager, Vector2 buttonField)
    {
        _isSelected = false;
        _assetManager = assetManager;
        Position = position;
        _hasVariableLabelAchievement = true;
        _font = _assetManager.GetFont("MainMenu");
        _achievement = achievement;
        _inputManager = inputManager;
        _buttonField = buttonField;
    }

    private void PlaySelectSound()
    {
        _assetManager.PlaySound("select_sound");
    }

    private void PlayClickedSound()
    {
        _assetManager.PlaySound("clicked_sound");
    }
    

    public bool OnEnter()
    {
        var position = Position;

        Position = new Vector2(position.X, position.Y);

        return false;
    }

    public bool MouseContained()
    {
        return new Rectangle((int)Position.X, (int)Position.Y - 10, (int)_buttonField.X, (int)_buttonField.Y).Contains(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition()));
    }

    public void Draw(SpriteBatch spriteBatch, int color = 0)
    {
        if (_hasVariableLabelStatistic)
        {
            Label = _statistic.Name + ": " + _statistic.Value;
        }
        if (_hasVariableLabelAchievement)
        {
            Label = _achievement.Name + ": " + _achievement.Description;
        }
        Color buttonColor;
        if (color == 0)
        {
            buttonColor = IsSelected ? Color.Red : Color.White;
        }
        // For completed achievements.
        else
        {
            buttonColor = Color.LightGreen;
        }

        if (IsSelected)
        {
            spriteBatch.Draw(_rightArrow, new Vector2(Position.X - 70, Position.Y - 15), new Rectangle(0, 0, 512, 512), Color.White, 0, new Vector2(0, 0), 0.1f, SpriteEffects.None, 0);
        }

        switch (Size)
        {
            case 1:
                spriteBatch.Draw(_buttonSmall, new Vector2(Position.X - 20, Position.Y - 20), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
                break;
            case 2:
                spriteBatch.Draw(_buttonMedium, new Vector2(Position.X - 20, Position.Y - 20), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
                break;
            case 3:
                spriteBatch.Draw(_buttonLarge, new Vector2(Position.X - 20, Position.Y - 20), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
                break;
            case 4:
                spriteBatch.Draw(_buttonHuge, new Vector2(Position.X - 20, Position.Y - 20), null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
                break;
        }
        spriteBatch.DrawString(_font, Label, Position, buttonColor,
            0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
        
    }
}
