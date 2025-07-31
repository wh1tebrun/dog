#region File Description
// KeybindingsScreen.cs
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur.Screens;

public class KeybindingsScreen : IScreen
{
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;
    private readonly List<Button> _actionButtons;
    private readonly List<Button> _controllerBackButtons;
    private int _selectedIndex;
    private bool _awaitingKeyPress;
    private bool _showController;
    private Texture2D _buttonBackground;

    public bool UpdateLower => false;
    public bool DrawLower => false;
    public void LoadContent() { }
    public void UnloadContent() { }

    public KeybindingsScreen(AssetManager assetManager, InputManager inputManager, GraphicsDevice graphicsDevice)
    {
        _assetManager = assetManager;
        _inputManager = inputManager;
        _actionButtons = new List<Button>();
        _controllerBackButtons = new List<Button>();
        _buttonBackground = new Texture2D(graphicsDevice, 1, 1);
        _buttonBackground.SetData([Color.DarkSlateGray]);
        _showController = false;
        
        int yOffset = 400;
        int xOffset = 150;

        var keyBindingList = GlobalSettings.KeyBindings.ToList();

        for (int i = 0; i < 3 && i < keyBindingList.Count; i++)
        {
            var action = keyBindingList[i];
            _actionButtons.Add(new Button($"{action.Key}: {action.Value}", new Vector2(xOffset, yOffset), assetManager, inputManager, new Vector2(400, 50), 4));
            yOffset += 75;
        }

        yOffset += 75;
        
        for (int i = 3; i < 7 && i < keyBindingList.Count; i++)
        {
            var action = keyBindingList[i];
            _actionButtons.Add(new Button($"{action.Key}: {action.Value}", new Vector2(xOffset, yOffset), assetManager, inputManager, new Vector2(400, 50), 4));
            yOffset += 75;
        }
        
        yOffset = 400;
        xOffset = 750;
        
        for (int i = 7; i < 15 && i < keyBindingList.Count; i++)
        {
            var action = keyBindingList[i];
            _actionButtons.Add(new Button($"{action.Key}: {action.Value}", new Vector2(xOffset, yOffset), assetManager, inputManager, new Vector2(400, 50), 4));
            yOffset += 75;
        }
        
        yOffset = 400;
        xOffset = 1350;
        
        _actionButtons.Add(new Button("Show controller keybinding", new Vector2(xOffset, yOffset), assetManager, inputManager, new Vector2(200, 50), 4));
        _actionButtons.Add(new Button("Set to default", new Vector2(xOffset, yOffset + 75), assetManager, inputManager, new Vector2(200, 50), 3));
        _actionButtons.Add(new Button("Back", new Vector2(xOffset, yOffset + 150), assetManager, inputManager, new Vector2(200, 50)));
        _actionButtons[_selectedIndex].IsSelected = true;
    }

    public int Update(GameTime gameTime)
    {
        if (_showController)
        {
            if (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsKeyPressed(Keys.Escape) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One))
            {
                _showController = false;
            }
            return -1;
        }

        var originalIndex = _selectedIndex;
        for (int i = 0; i < _actionButtons.Count; i++)
        {
            if (_actionButtons[i].MouseContained())
            {
                _selectedIndex = i;
                if (_inputManager.IsLeftMousePressed())
                {
                    if (_selectedIndex != originalIndex)
                    {
                        _actionButtons[originalIndex].IsSelected = false;
                        _actionButtons[_selectedIndex].IsSelected = true;
                    }
                    _actionButtons[_selectedIndex].IsClicked = true;

                    if (_selectedIndex == 15)
                    {
                        _showController = true;
                        return -1;
                    }

                    if (_selectedIndex == 16)
                    {
                        SetToDefault();
                        return -1;
                    }

                    if (_selectedIndex == 17)
                    {
                        MenuControl.IsKeybindingActive = false;
                        return 0;

                    }
                    if (_selectedIndex != -1 && _selectedIndex != -2 && _selectedIndex != -3)
                    {
                        _awaitingKeyPress = true;
                    }
                }
                break;
            }
        }

        if (_inputManager.IsKeyPressed(Keys.Down) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Down)
        {
            _selectedIndex = (_selectedIndex + 1) % _actionButtons.Count;
        }
        else if (_inputManager.IsKeyPressed(Keys.Up) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Up)
        {
            _selectedIndex = (_selectedIndex - 1 + _actionButtons.Count) % _actionButtons.Count;
        }

        if (_selectedIndex != originalIndex)
        {
            _actionButtons[originalIndex].IsSelected = false;
            _actionButtons[_selectedIndex].IsSelected = true;
        }
        
       
        if (_selectedIndex != _actionButtons.Count - 1 && _selectedIndex != _actionButtons.Count - 2 && 
            _selectedIndex != _actionButtons.Count - 3 &&
            (_inputManager.IsKeyPressed(Keys.Enter)))
        {
            _actionButtons[_selectedIndex].IsClicked = true;
            _awaitingKeyPress = true;
        }

        if (_selectedIndex == 15 && (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One)))
        {
            _actionButtons[_selectedIndex].IsClicked = true;
            _showController = !_showController;
            return -1; 
        }

        if (_selectedIndex == 16 && (_inputManager.IsKeyPressed(Keys.Enter) ||
                                     _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One)))
        {
            _actionButtons[_selectedIndex].IsClicked = true;
            SetToDefault();
            return -1;
        }
        if (_selectedIndex == 17 && (_inputManager.IsKeyPressed(Keys.Enter) ||
                                     _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One))
            || _inputManager.IsKeyPressed(Keys.Escape) || _inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One))
        {
            _actionButtons[_selectedIndex].IsClicked = true;
            MenuControl.IsKeybindingActive = false;
            return 0;
        }
        
        // Wait for user to press a key (except for enter, escape, already bind keys) in order to change the binding
        if (_awaitingKeyPress)
        {
            if (_selectedIndex > _actionButtons.Count - 3)
            {
                return -1;
                
            }
            var pressedKeys = Keyboard.GetState().GetPressedKeys();
            if (pressedKeys.Length == 0)
            {
                return -1;
            }
            var newKey = pressedKeys[0];
            if (newKey is Keys.Enter || _selectedIndex >= GlobalSettings.KeyBindings.ToArray().Count())
            {
                return -1;
            }
            var action = GlobalSettings.KeyBindings.Keys.ToArray()[_selectedIndex];
            if (GlobalSettings.KeyBindings.ContainsValue(newKey))
            {
                return -1;
            }
            GlobalSettings.KeyBindings[action] = newKey;
            _actionButtons[_selectedIndex].Label = $"{action}: {newKey}";
            _awaitingKeyPress = false;

        }
        return -1;
    }

    private void SetToDefault()
    {
        // set all keybindings to default explicitly.
        // offensive player
        GlobalSettings.KeyBindings["UpOffensive"] = Keys.Up;
        GlobalSettings.KeyBindings["LeftOffensive"] = Keys.Left;
        GlobalSettings.KeyBindings["RightOffensive"] = Keys.Right;
        GlobalSettings.KeyBindings["ChangeWeapon"] = Keys.RightShift;
        
        // general
        GlobalSettings.KeyBindings["EndBossActivate"] = Keys.M;
        GlobalSettings.KeyBindings["DebugMode"] = Keys.I;
        GlobalSettings.KeyBindings["GodMode"] = Keys.G;
        
        // defensive player
        GlobalSettings.KeyBindings["UpDefensive"] = Keys.W;
        GlobalSettings.KeyBindings["LeftDefensive"] = Keys.A;
        GlobalSettings.KeyBindings["RightDefensive"] = Keys.D;
        GlobalSettings.KeyBindings["Invisibility"] = Keys.R;
        GlobalSettings.KeyBindings["PickUpPotion"] = Keys.E;
        GlobalSettings.KeyBindings["ConsumePotion"] = Keys.F;
        GlobalSettings.KeyBindings["ActivateShield"] = Keys.Q;
        GlobalSettings.KeyBindings["ChangeShield"] = Keys.LeftShift;
        
        // show the lables with the default key.
        for (int i = 0; i < _actionButtons.Count - 3; i++)
        {
            _actionButtons[i].Label = $"{GlobalSettings.KeyBindings.Keys.ToArray()[i]}: {GlobalSettings.KeyBindings.Values.ToArray()[i]}";
        }
    }

    private void DrawController(SpriteBatch spriteBatch)
    {
        if (_showController)
        {
            spriteBatch.Draw(_buttonBackground, new Rectangle(50, 230, 1235, 800), new Color(Color.Aqua, 0.85f));
            spriteBatch.Draw(_assetManager.GetTexture("UI_Controller_Keys_Thumbnail2"), new Vector2(45, 125),
                new Rectangle(0, 0, 4311, 3629), Color.White, 0, Vector2.Zero, new Vector2(0.3f, 0.3f),
                SpriteEffects.None, 0);
        }
    }


    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        spriteBatch.Draw(_assetManager.GetTexture("background"), Vector2.Zero, Color.White);
        spriteBatch.Draw(_buttonBackground, new Rectangle(50, 230, 1850, 800), new Color(Color.Aqua, 0.5f));


        spriteBatch.DrawString(
            _assetManager.GetFont("Thaleah"),
            "Keybindings",
            new Vector2(150, 250),
            Color.Yellow,
            0,
            Vector2.Zero,
            4.0f,
            SpriteEffects.None,
            0.5f
        );

        foreach (var button in _actionButtons)
        {
            button.Draw(spriteBatch);
        }
        

        if (_awaitingKeyPress)
        {
            var messagePosition = new Vector2(150, 300);
            

                spriteBatch.DrawString(
                    _assetManager.GetFont("Thaleah"),
                    "Press any key to rebind...",
                    messagePosition,
                    Color.Yellow,
                    0,
                    Vector2.Zero,
                    3.0f,
                    SpriteEffects.None,
                    0.5f
                );
            
        }

        if (!_awaitingKeyPress)
        {
            var messagePosition = new Vector2(150, 300);

            spriteBatch.DrawString(
                _assetManager.GetFont("Thaleah"),
                "Press Enter or click mouse to edit",
                messagePosition,
                Color.Yellow,
                0,
                Vector2.Zero,
                3.0f,
                SpriteEffects.None,
                0.5f
            );
        }
        DrawController(spriteBatch);

        spriteBatch.End();
    }
}
