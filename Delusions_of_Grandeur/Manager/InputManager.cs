# region File Description
// AssetManager.cs
// Manage the Assets for the game.
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur.Manager;

public class InputManager
{
    /// <summary>
    /// Holds state of the keyboard.
    /// </summary>
    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;
    
    /// <summary>
    /// Holds state of the mouse.
    /// </summary>
    private MouseState _previousMouseState;
    private MouseState _currentMouseState;

    /// <summary>
    /// Holds state of the gamepad for the first player.
    /// </summary>
    private GamePadState _previousGamePadStatePlayerOne;
    private GamePadState _currentGamePadStatePlayerOne;
    
    /// <summary>
    /// Holds state of the gamepad for the second player.
    /// </summary>
    private GamePadState _previousGamePadStatePlayerTwo;
    private GamePadState _currentGamePadStatePlayerTwo;

    /// <summary>
    /// Constantly update the controls.
    /// </summary>
    public void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
        
        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
        
        _previousGamePadStatePlayerOne = _currentGamePadStatePlayerOne;
        _currentGamePadStatePlayerOne = GamePad.GetState(PlayerIndex.One);
        
         _previousGamePadStatePlayerTwo = _currentGamePadStatePlayerTwo;
        _currentGamePadStatePlayerTwo = GamePad.GetState(PlayerIndex.Two);
        
    }
    // Considers pressing and releasing a key in order to recognize a single key hit.
    public bool IsKeyPressed(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    public bool IsKeyDown(Keys key)
    {
        return _currentKeyboardState.IsKeyDown(key);
    }
    
    public bool IsLeftMousePressed()
    {
        return _currentMouseState.LeftButton == ButtonState.Pressed &&
               _previousMouseState.LeftButton == ButtonState.Released;
    }

    public bool IsRightMousePressed()
    {
        return _currentMouseState.RightButton == ButtonState.Pressed &&
               _previousMouseState.RightButton == ButtonState.Released;
    }

    public Vector2 MouseClickPosition()
    {
        Vector2 mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
        return mousePosition;
    }

    private bool IsLeftThumbstickMoved(PlayerIndex playerIndex)
    {
        switch (playerIndex)
        {
            // checks if stick is moved left, right or down
            case PlayerIndex.One when _currentGamePadStatePlayerOne.ThumbSticks.Left.X < 0 || _currentGamePadStatePlayerOne.ThumbSticks.Left.X > 0
                || _currentGamePadStatePlayerOne.ThumbSticks.Left.Y <
                0:
            // checks if stick is moved left, right or down
            case PlayerIndex.Two when _currentGamePadStatePlayerTwo.ThumbSticks.Left.X < 0 || _currentGamePadStatePlayerTwo.ThumbSticks.Left.X > 0
                || _currentGamePadStatePlayerTwo.ThumbSticks.Left.Y <
                0:
                return true;
            default:
                return false;
        }
    }
    
    public float[] ThumbstickDirection(PlayerIndex playerIndex)
    {   
        if (playerIndex == PlayerIndex.One && IsLeftThumbstickMoved(playerIndex))
        {
            return [_currentGamePadStatePlayerOne.ThumbSticks.Left.X, _currentGamePadStatePlayerOne.ThumbSticks.Left.Y];
        }
        
        if (playerIndex == PlayerIndex.Two && IsLeftThumbstickMoved(playerIndex))
        {
            return [_currentGamePadStatePlayerTwo.ThumbSticks.Left.X, _currentGamePadStatePlayerTwo.ThumbSticks.Left.Y];
        }

        return [0, 0];
    }
    
    
    public bool IsButtonPressed(Buttons button, PlayerIndex playerIndex)
    {
        return playerIndex == PlayerIndex.One
            ? _currentGamePadStatePlayerOne.IsButtonDown(button) &&
              _previousGamePadStatePlayerOne.IsButtonUp(button)
            : _currentGamePadStatePlayerTwo.IsButtonDown(button) &&
              _previousGamePadStatePlayerTwo.IsButtonUp(button);
    }
    
    public bool IsButtonReleased(Buttons button, PlayerIndex playerIndex)
    {
        return playerIndex == PlayerIndex.One
            ? _currentGamePadStatePlayerOne.IsButtonUp(button) &&
              _previousGamePadStatePlayerOne.IsButtonDown(button)
            : _currentGamePadStatePlayerTwo.IsButtonUp(button) &&
              _previousGamePadStatePlayerTwo.IsButtonDown(button);
    }

    public bool IsLeftTriggerPressed(PlayerIndex playerIndex)
    {
        return playerIndex == PlayerIndex.One
            ? _currentGamePadStatePlayerOne.Triggers.Left > 0.0f &&
              _previousGamePadStatePlayerOne.Triggers.Left == 0.0f
            : _currentGamePadStatePlayerTwo.Triggers.Left > 0.0f &&
              _previousGamePadStatePlayerTwo.Triggers.Left == 0.0f;
    }

    public bool IsRightTriggerPressed(PlayerIndex playerIndex)
    {
        return playerIndex == PlayerIndex.One
            ? _currentGamePadStatePlayerOne.Triggers.Right > 0.0f &&
              _previousGamePadStatePlayerOne.Triggers.Right == 0.0f
            : _currentGamePadStatePlayerTwo.Triggers.Right > 0.0f &&
              _previousGamePadStatePlayerTwo.Triggers.Right == 0.0f;
    }
    
    public EDpaDdirection DpaDdirection()
    {
        if (_previousGamePadStatePlayerOne.DPad.Down == ButtonState.Pressed &&
            _currentGamePadStatePlayerOne.DPad.Down == ButtonState.Released)
            return EDpaDdirection.Down;
        if (_previousGamePadStatePlayerOne.DPad.Up == ButtonState.Pressed &&
            _currentGamePadStatePlayerOne.DPad.Up == ButtonState.Released)
            return EDpaDdirection.Up;
        if (_currentGamePadStatePlayerOne.DPad.Left == ButtonState.Pressed)
            return EDpaDdirection.Left;
        if (_currentGamePadStatePlayerOne.DPad.Right == ButtonState.Pressed)
            return EDpaDdirection.Right;

        return EDpaDdirection.Neutral;
    }
    
    public GamePadState CurrentGamePadStatePlayerTwo => _currentGamePadStatePlayerTwo;

    public enum EDpaDdirection
    {
        Left, Right, Up, Down, Neutral
    }
}