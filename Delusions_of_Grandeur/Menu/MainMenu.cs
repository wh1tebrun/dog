using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur.Menu;

public class MainMenu
{
    private readonly InputManager _inputManager;
    private readonly List<Button> _buttons;
    private readonly Texture2D _title;
    private readonly Vector2 _titlePosition;

    private readonly Texture2D _background;

    public MainMenu(AssetManager assetManager, InputManager inputManager, GraphicsDevice graphics)
    {
        _buttons = [];
        _inputManager = inputManager;

        _buttons =
        [
            new Button("Start", new Vector2(170, 330), assetManager, _inputManager, new Vector2(100, 50)),
            new Button("Load game", new Vector2(170, 430), assetManager, _inputManager, new Vector2(200, 50), 3),
            new Button("Tech-Demo", new Vector2(170, 530), assetManager, _inputManager, new Vector2(200, 50), 3),
            new Button("Statistics", new Vector2(170, 630), assetManager, _inputManager, new Vector2(160, 50), 2),
            new Button("Achievements", new Vector2(170, 730), assetManager, _inputManager, new Vector2(230, 50), 3),
            new Button("Settings", new Vector2(170, 830), assetManager, _inputManager, new Vector2(150, 50), 2),
            new Button("Exit", new Vector2(170, 930), assetManager, _inputManager, new Vector2(80, 50))
        ];
        
        _buttons[0].IsSelected = true;

        var buttonBackground = new Texture2D(graphics, 1, 1);
        buttonBackground.SetData([Color.DarkSlateGray]);

        _title = assetManager.GetTexture("title");
        _titlePosition = new Vector2(100, 20);
        assetManager.GetFont("MainMenu");
        _background = assetManager.GetTexture("tower");
    }

    public void LoadContent()
    {
    }
    
    public int Update(GameTime gameTime)
    {
        var selectedButtonIndex = _buttons.FindIndex(button => button.IsSelected);
        if (selectedButtonIndex == -1) selectedButtonIndex = 0;

        var originalIndex = selectedButtonIndex;

        if (_inputManager.IsKeyPressed(Keys.Down) || _inputManager.IsKeyPressed(Keys.Up) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Down || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Up)
        {
            if (_inputManager.IsKeyPressed(Keys.Down) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Down)
            {
                selectedButtonIndex = (selectedButtonIndex + 1) % _buttons.Count;
            }
            else if (_inputManager.IsKeyPressed(Keys.Up) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Up)
            {
                selectedButtonIndex = (selectedButtonIndex - 1 + _buttons.Count) % _buttons.Count;
            }
        }

        for (int i = 0; i < _buttons.Count; i++)
        {
            if (_buttons[i].MouseContained())
            {
                selectedButtonIndex = i;
                if (_inputManager.IsLeftMousePressed())
                {
                    if (selectedButtonIndex != originalIndex)
                    {
                        _buttons[originalIndex].IsSelected = false;
                        _buttons[selectedButtonIndex].IsSelected = true;
                    }
                    _buttons[selectedButtonIndex].IsClicked = true;
                    return selectedButtonIndex;

                }
                break;
            }
        }
        
        if (selectedButtonIndex != originalIndex)
        {
            _buttons[originalIndex].IsSelected = false;
            _buttons[selectedButtonIndex].IsSelected = true;
        }
        
        
        if ((!_inputManager.IsKeyPressed(Keys.Enter) &&
             !_inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One)) ||
            !_buttons[selectedButtonIndex].IsSelected) return -1;

        _buttons[selectedButtonIndex].IsClicked = true;

        
        foreach (var button in _buttons)
        {
            if (button.OnEnter())
            {

            }
        }
        return selectedButtonIndex;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }
        spriteBatch.Draw(_title, new Rectangle((int)_titlePosition.X, (int)_titlePosition.Y, _title.Width * 3, _title.Height * 3), Color.White);
        spriteBatch.End();
    }
}
