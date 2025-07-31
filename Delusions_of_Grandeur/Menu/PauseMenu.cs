using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Delusions_of_Grandeur.Menu;

internal enum EPauseMenuButtonType
{
    Continue,
    MusicVolume,
    SoundVolume
}

public class PauseMenu
{
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;
    private readonly List<Button> _buttons;
    private readonly GameControl _gameControl;
    private readonly Texture2D _backgroundBlue;
    private readonly Texture2D _foreground;
    private Rectangle _volumeBoxMusic;
    private Rectangle _volumeBoxSound;
    private float _filledWidthMusic;
    private float _filledWidthSound;

    private readonly Texture2D _soundOnTexture;
    private readonly Texture2D _soundOffTexture;
    private readonly GraphicsDevice _graphics;
    private Rectangle _musicBox;
    
    public PauseMenu(AssetManager assetManager, InputManager inputManager, GameControl gameControl, GraphicsDevice graphics)
    {
        _assetManager = assetManager;
        _inputManager = inputManager;
        _gameControl = gameControl;
        _graphics = graphics;

        _backgroundBlue = new Texture2D(graphics, 1, 1);
        _backgroundBlue.SetData([Color.DarkBlue]);

        _foreground = new Texture2D(graphics, 1, 1);
        _foreground.SetData([Color.DarkSlateGray]);

        _soundOnTexture = _assetManager.GetTexture("SoundOn");
        _soundOffTexture = _assetManager.GetTexture("SoundOff");

        _musicBox = new Rectangle(1400, 625, 60, 55);

        _assetManager = assetManager;
        _inputManager = inputManager;
        _gameControl = gameControl;

        _backgroundBlue = new Texture2D(graphics, 1, 1);
        _backgroundBlue.SetData([Color.BlueViolet]);

        _foreground = new Texture2D(graphics, 1, 1);
        _foreground.SetData([Color.DarkSlateGray]);

        _volumeBoxMusic = new Rectangle(1100, 605, 200, 20);
        _volumeBoxSound = new Rectangle(1100, 705, 200, 20);
        _filledWidthMusic = MediaPlayer.Volume * _volumeBoxMusic.Width;
        _filledWidthSound = SoundEffect.MasterVolume * _volumeBoxSound.Width;

        _buttons =
        [
            new Button("Continue", new Vector2(800, 400), _assetManager, _inputManager, new Vector2(150, 50), 2),
            new Button("Save Game", new Vector2(800, 500), _assetManager, _inputManager, new Vector2(180, 50), 3),
            new Button("Music Volume", new Vector2(800, 600), _assetManager, _inputManager, new Vector2(220, 50), 3),
            new Button("Sound Volume", new Vector2(800, 700), _assetManager, _inputManager, new Vector2(220, 50), 3),
            new Button("Quit", new Vector2(800, 800), _assetManager, _inputManager, new Vector2(100, 50))
        ];

        _buttons[(int)EPauseMenuButtonType.Continue].IsSelected = true;
    }

    /// <summary>
    /// Updates the selected button based on user input.
    /// Updates the clicked button based on user input.
    /// Triggers actions based on clicked button.
    /// </summary>
    public int Update()
    {
        if (_buttons.Count == 0) return -1;

        if (_inputManager.IsKeyPressed(Keys.Escape) || _inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One))
        {
            return 0;
        }
        
        if (_inputManager.IsLeftMousePressed() && _musicBox.Contains(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition())) || _inputManager.IsRightTriggerPressed(PlayerIndex.One))
        {
            SettingsMenu.PlayMusic = !SettingsMenu.PlayMusic;
            AssetManager.IsSoundActive = !AssetManager.IsSoundActive;
            if (SettingsMenu.PlayMusic)
            {
                MediaPlayer.IsMuted = false;
            }
            else
            {
                MediaPlayer.IsMuted = true;
            }
        }

        if (_inputManager.IsLeftMousePressed())
        {
            if (_volumeBoxMusic.Contains(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition())))
            {
                _filledWidthMusic = MathHelper.Clamp(
                    GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition()).X - _volumeBoxMusic.X,
                    0,
                    _volumeBoxMusic.Width
                );

                var volume = _filledWidthMusic / _volumeBoxMusic.Width;
                _gameControl.ChangeBackgroundMusicVolumeMouse(volume);
            }
            else if (_volumeBoxSound.Contains(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition())))
            {
                _filledWidthSound = MathHelper.Clamp(
                    GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition()).X - _volumeBoxSound.X,
                    0,
                    _volumeBoxSound.Width
                );

                var volume = _filledWidthSound / _volumeBoxSound.Width;
                _assetManager.ChangeSoundVolumeMouse(volume);
            }
        }
        var selectedButtonIndex = _buttons.FindIndex(button => button.IsSelected);

        if (selectedButtonIndex == -1) selectedButtonIndex = 0;

        var originalIndex = selectedButtonIndex;

        if (_inputManager.IsKeyDown(Keys.Right) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Right)
        {
            switch (selectedButtonIndex)
            {
                case (int)EPauseMenuButtonType.MusicVolume:
                    _gameControl.ChangeBackgroundMusicVolumeArrows(0.01f);
                    _filledWidthMusic = GlobalSettings.MediaPlayerVolume * _volumeBoxMusic.Width;
                    break;

                case (int)EPauseMenuButtonType.SoundVolume:
                    _assetManager.ChangeSoundVolumeArrows(0.01f);
                    _filledWidthSound = _assetManager.SoundVolume * _volumeBoxSound.Width;
                    break;
                case 4:
                    return 0;
            }
        }
        else if (_inputManager.IsKeyDown(Keys.Left) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Left)
        {
            switch (selectedButtonIndex)
            {
                case (int)EPauseMenuButtonType.MusicVolume:
                    _gameControl.ChangeBackgroundMusicVolumeArrows(-0.01f);
                    _filledWidthMusic = GlobalSettings.MediaPlayerVolume * _volumeBoxMusic.Width;
                    break;

                case (int)EPauseMenuButtonType.SoundVolume:
                    _assetManager.ChangeSoundVolumeArrows(-0.01f);
                    _filledWidthSound = _assetManager.SoundVolume * _volumeBoxSound.Width;
                    break;
                case 4:
                    return 0;
            }
        }

        if (_inputManager.IsKeyPressed(Keys.Down) || _inputManager.IsKeyPressed(Keys.Up) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Up || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Down)
        {
            _buttons[selectedButtonIndex].IsSelected = false;

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

        if (!_inputManager.IsKeyPressed(Keys.Enter) &&
                !_inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One)) return -1;
        _buttons[selectedButtonIndex].IsClicked = true;

        return selectedButtonIndex;
    }


    /// <summary>
    /// Draws a grey bar and then a violet bar that represents the relative value of volume.
    /// It represents the current volume.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="volume"></param>
    /// <param name="volumeBox"></param>
    private void DrawVolumeVisualization(SpriteBatch spriteBatch, float volume, Rectangle volumeBox)
    {
        spriteBatch.Draw(_backgroundBlue, volumeBox, Color.White);

        int filledWidth = (int)(volumeBox.Width * volume);
        Rectangle filledBar = new Rectangle(volumeBox.X, volumeBox.Y, filledWidth, volumeBox.Height);

        spriteBatch.Draw(_foreground, filledBar, Color.DarkSlateGray);
        spriteBatch.Draw(_assetManager.GetTexture("horizontal_border"), volumeBox, Color.White);
    }

    /// <summary>
    /// Draw the Menu screen with the respective buttons.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        const int centerX = (1920 - 1200) / 2;
        const int centerY = (1080 - 800) / 2;
        var overlayRectangle = new Rectangle(centerX, centerY, 1200, 800);

        var overlayTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        overlayTexture.SetData([Color.DarkSlateGray * 0.5f]);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        spriteBatch.Draw(overlayTexture, overlayRectangle, Color.White);
        spriteBatch.Draw(SettingsMenu.PlayMusic ? _soundOnTexture : _soundOffTexture, new Rectangle(1400, 625, _soundOnTexture.Width, _soundOnTexture.Height), Color.White);


        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }

        DrawVolumeVisualization(spriteBatch, _filledWidthMusic / _volumeBoxMusic.Width, _volumeBoxMusic);
        DrawVolumeVisualization(spriteBatch, _filledWidthSound / _volumeBoxSound.Width, _volumeBoxSound);

        spriteBatch.End();
    }
}
