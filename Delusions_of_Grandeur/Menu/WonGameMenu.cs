using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur.Menu;

public class WonGameMenu
{
    private readonly InputManager _inputManager;
    private readonly List<Button> _buttons;
    private readonly Texture2D _background;
    private readonly GraphicsDevice _graphics;
    
    public WonGameMenu(AssetManager assetManager, InputManager inputManager, GraphicsDevice graphics, StatisticMenu statisticMenu)
    {
        _buttons = [];
        _inputManager = inputManager;
        _graphics = graphics;
        
        assetManager.GetMusic("wongamemusic");

        _buttons =
        [
            new Button("Exit", new Vector2(200, 750), assetManager, _inputManager, new Vector2(80, 50))
        ];
        _buttons[0].IsSelected = true;
        
        var yOffset = 200;
        
        foreach (var statistic in statisticMenu.Statistics)
        {
            _buttons.Add(new Button(new Vector2(200, yOffset), assetManager, statistic, _inputManager, new Vector2(10, 10)));
            yOffset += 100;
        }
        
        var buttonBackground = new Texture2D(graphics, 1, 1);
        buttonBackground.SetData([Color.DarkSlateGray]);
        
        assetManager.GetFont("MainMenu");
        _background = assetManager.GetTexture("wongame");
    }

    public int Update()
    {
        if (_buttons[0].MouseContained() && _inputManager.IsLeftMousePressed())
        {
            _buttons[0].IsClicked = true;
            return 0;
        }
        if (!_inputManager.IsKeyPressed(Keys.Escape) && !_inputManager.IsKeyPressed(Keys.Enter) && !_inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One) && !_inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One)) return -1;
        _buttons[0].IsClicked = true;
        return 0;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        var screenWidth = _graphics.Viewport.Width;
        var screenHeight = _graphics.Viewport.Height;

        // Scale image to fit to screen.
        var scaleX = (float)screenWidth / _background.Width;
        var scaleY = (float)screenHeight / _background.Height;
        spriteBatch.Draw(_background, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
        
        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }
        spriteBatch.End();
    }
}