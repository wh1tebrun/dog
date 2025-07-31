#region File Description
// SaveGameMenu.cs
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur.Menu;

public enum ESaveOperationType
{
    LoadGame,
    SaveGame
}
    
public class SaveGameMenu
{
    private readonly ESaveOperationType _saveOperationType;
    private readonly InputManager _inputManager;
    private readonly List<Button> _buttons;
    private readonly Texture2D _background;
    private readonly Texture2D _buttonBackground;
    private readonly MenuControl _menuControl;

    private bool _showConfirmation;
    private int _pendingSlotIndex = -1;
    private readonly Button _yesButton;
    private readonly Button _noButton;
    private int _confirmationSelectedIndex;
        
    private readonly Texture2D _confirmationTexture;

    public SaveGameMenu(AssetManager assetManager, InputManager inputManager, GraphicsDevice graphics,
        ESaveOperationType saveOperationType, MenuControl menuControl)
    {
        _menuControl = menuControl;
        _saveOperationType = saveOperationType;
        _inputManager = inputManager;
        _buttons = [];
        _confirmationTexture = new Texture2D(graphics, 1, 1);
        _confirmationTexture.SetData([Color.Purple]);   

        // Create slot buttons 
        for (var i = 0; i < SaveSystem.MaxSlot; i++)
        {
            _buttons.Add(new Button("Save Slot " + (i + 1), new Vector2(200, 300 + i * 100), assetManager, _inputManager, new Vector2(200, 50), 3));
        }

        // Add Back button
        _buttons.Add(new Button("Back", new Vector2(200, 300 + SaveSystem.MaxSlot * 100), assetManager, _inputManager, new Vector2(80, 50)));
        _buttons[0].IsSelected = true;

        // Load background
        _background = assetManager.GetTexture("background");
        _buttonBackground = new Texture2D(graphics, 1, 1);
        _buttonBackground.SetData([Color.DarkSlateGray]);

        _yesButton = new Button("YES",  new Vector2(320, 300 + (SaveSystem.MaxSlot + 2) * 100), assetManager, _inputManager, new Vector2(90, 50));
        _noButton  = new Button("NO",   new Vector2(520, 300 + (SaveSystem.MaxSlot + 2) * 100), assetManager, _inputManager, new Vector2(90, 50));
    }

    /// <summary>
    /// Updates the menu logic.
    /// </summary>
    public int Update()
    {
        
        if (_inputManager.IsKeyPressed(Keys.Escape) || _inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One))
        {
            return 0;
        }
        
        // If the confirmation screen is not active, handle normal slot/back selection
        if (!_showConfirmation)
        {
            var selectedButtonIndex = _buttons.FindIndex(button => button.IsSelected);
            if (selectedButtonIndex == -1) selectedButtonIndex = 0;

            var originalIndex = selectedButtonIndex;

            // Change selected menu option
            if (_inputManager.IsKeyPressed(Keys.Down) || _inputManager.IsKeyPressed(Keys.Up) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Up || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Down)
            {
                if (_inputManager.IsKeyPressed(Keys.Down) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Down)
                    selectedButtonIndex = (selectedButtonIndex + 1) % _buttons.Count;
                else if (_inputManager.IsKeyPressed(Keys.Up) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Up)
                    selectedButtonIndex = (selectedButtonIndex - 1 + _buttons.Count) % _buttons.Count;
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
                        // Back
                        if (selectedButtonIndex >= SaveSystem.MaxSlot)
                        {
                            return 0;
                        }
                        _pendingSlotIndex = selectedButtonIndex;
                        _showConfirmation = true;
                        _confirmationSelectedIndex = 0;

                    }
                    break;
                }
            }
                
            if (selectedButtonIndex != originalIndex)
            {
                _buttons[originalIndex].IsSelected = false;
                _buttons[selectedButtonIndex].IsSelected = true;
            }

            // Confirm (Enter)
            if (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One))
            {
                _buttons[selectedButtonIndex].IsClicked = true;

                // Back
                if (selectedButtonIndex >= SaveSystem.MaxSlot)
                {
                    return 0;
                }
                _pendingSlotIndex = selectedButtonIndex;
                _showConfirmation = true;
                _confirmationSelectedIndex = 0;
            }
            return -1;
        }

        if (_yesButton.MouseContained())
        {
            _confirmationSelectedIndex = 0;
            if (_inputManager.IsLeftMousePressed())
            {
                ConfirmSaveOperation(_pendingSlotIndex);
                _showConfirmation = false;
                _pendingSlotIndex = -1;
                _yesButton.IsClicked = true;
            }
                
        } else if (_noButton.MouseContained())
        {
            _confirmationSelectedIndex = 1;
            if (_inputManager.IsLeftMousePressed())
            {
                _showConfirmation = false;
                _pendingSlotIndex = -1;
                _noButton.IsClicked = true;
            }
        }

        // If the confirmation screen is active, handle YES/NO selection
        if (_inputManager.IsKeyPressed(Keys.Left) || _inputManager.IsKeyPressed(Keys.Right) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Left || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Right)
        {
            _confirmationSelectedIndex = _confirmationSelectedIndex == 0 ? 1 : 0;
        }

        if (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One))
        {
            if (_confirmationSelectedIndex == 0) // YES
            {
                // If confirmed, save or load
                _yesButton.IsClicked = true;
                ConfirmSaveOperation(_pendingSlotIndex);
            }
            else
            {
                _noButton.IsClicked = true;
            }

            // NO
            _showConfirmation = false;
            _pendingSlotIndex = -1;
                
        }

        return -1;
    }

    /// <summary>
    /// Handles saving/loading the game to the selected slot.
    /// </summary>
    private void ConfirmSaveOperation(int slotIndex)
    {
        if (_saveOperationType == ESaveOperationType.SaveGame)
        {
            _menuControl.GameControl.SaveSystem.SaveGame(slotIndex);
        }
        else
        {
            _menuControl.ScreenManager.Pop();
            _menuControl.IsSubMenuActive = false;
            _menuControl.IsMainMenuActive = true;
            _menuControl.StartNewGame(true);
            _menuControl.GameControl.SaveSystem.LoadGame(slotIndex);
        }
    }

    /// <summary>
    /// Draws the menu background, buttons, and (if needed) the confirmation message.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_background, Vector2.Zero, Color.White);
        spriteBatch.Draw(_buttonBackground, new Rectangle(100, 230, 1400, 650), new Color(Color.Aqua, 0.5f));

        // Draw normal buttons
        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }

        // If the confirmation screen is active, display a simple message and Yes/No
        if (_showConfirmation)
        {
            var font = _menuControl.GameControl.AssetManager.GetFont("MainMenu");
                
            spriteBatch.Draw(_confirmationTexture, new Rectangle(150, 670, 640, 200), new Color(Color.White, 0.5f));
            spriteBatch.DrawString(font, "Are you sure you want to proceed?", new Vector2(200, 300 + (SaveSystem.MaxSlot + 1) * 100), Color.White);

            _yesButton.IsSelected = _confirmationSelectedIndex == 0;
            _yesButton.Draw(spriteBatch);

            _noButton.IsSelected = _confirmationSelectedIndex == 1;
            _noButton.Draw(spriteBatch);
        }

        spriteBatch.End();
    }
}
