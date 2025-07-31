#region File Description
// SettingsMenu.cs
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Delusions_of_Grandeur.Menu;

public class SettingsMenu
{
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;
    private readonly GameControl _gameControl;
    private readonly List<Button> _buttons;
    private readonly Texture2D _backgroundBlue;
    private readonly Texture2D _buttonBackground;
    private readonly Texture2D _foreground;
    private readonly Texture2D _tower;
    private readonly Texture2D _title;
    private readonly Texture2D _wrench;
    private readonly Texture2D _soundOnTexture;
    private readonly Texture2D _soundOffTexture;
    private float _filledWidthMusic;
    private float _filledWidthSound;
    private Rectangle _volumeBoxMusic;
    private Rectangle _volumeBoxSound;
    private readonly GraphicsDevice _graphics;
    private Rectangle _musicBox;
    public static bool PlayMusic = true;

    public SettingsMenu(AssetManager assetManager, InputManager inputManager, GraphicsDevice graphics, GameControl gameControl)
    {
        _assetManager = assetManager;
        _inputManager = inputManager;
        _gameControl = gameControl;
        _graphics = graphics;

        _title = _assetManager.GetTexture("title");
        _backgroundBlue = new Texture2D(graphics, 1, 1);
        _backgroundBlue.SetData([Color.BlueViolet]);
        _buttonBackground = new Texture2D(graphics, 1, 1);
        _buttonBackground.SetData([Color.DarkSlateGray]);

        _foreground = new Texture2D(graphics, 1, 1);
        _foreground.SetData([Color.DarkSlateGray]);

        _wrench = _assetManager.GetTexture("wrench");
        _tower = _assetManager.GetTexture("background");

        _soundOnTexture = _assetManager.GetTexture("SoundOn");
        _soundOffTexture = _assetManager.GetTexture("SoundOff");

        _buttons =
        [
            new Button("Music volume", new Vector2(200, 500), _assetManager, _inputManager, new Vector2(220, 50), 3),
            new Button("Sound volume", new Vector2(200, 580), _assetManager, _inputManager, new Vector2(220, 50), 3),
            new Button("Key-binding", new Vector2(200, 660), _assetManager, _inputManager, new Vector2(220, 50), 3),
            new Button("Back", new Vector2(200, 740), _assetManager, _inputManager, new Vector2(80, 50))
        ];
        _buttons[0].IsSelected = true;

        _volumeBoxMusic = new Rectangle(470, 500, 200, 30);
        _volumeBoxSound = new Rectangle(470, 575, 200, 30);

        _musicBox = new Rectangle(200, 380, 60, 55);

        _filledWidthMusic = MediaPlayer.Volume * _volumeBoxMusic.Width;
        _filledWidthSound = SoundEffect.MasterVolume * _volumeBoxSound.Width;
    }

    /// <summary>
    /// Updates the selected button based on user input.
    /// Updates the clicked button based on user input.
    /// Triggers actions based on clicked button.
    /// </summary>
    public int Update()
    {
        if (_inputManager.IsLeftMousePressed() && _musicBox.Contains(GameControl.ConvertScreenPositionToScene(_inputManager.MouseClickPosition())) || _inputManager.IsRightTriggerPressed(PlayerIndex.One))
        {
            PlayMusic = !PlayMusic;
            AssetManager.IsSoundActive = !AssetManager.IsSoundActive;
            if (PlayMusic)
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
                case 0:
                    _gameControl.ChangeBackgroundMusicVolumeArrows(0.01f);
                    _filledWidthMusic = GlobalSettings.MediaPlayerVolume * _volumeBoxMusic.Width;
                    break;

                case 1:
                    _assetManager.ChangeSoundVolumeArrows(0.01f);
                    _filledWidthSound = _assetManager.SoundVolume * _volumeBoxSound.Width;
                    break;
                case 2:
                    break;
            }
        }
        else if (_inputManager.IsKeyDown(Keys.Left) || _inputManager.DpaDdirection() == InputManager.EDpaDdirection.Left)
        {
            switch (selectedButtonIndex)
            {
                case 0:
                    _gameControl.ChangeBackgroundMusicVolumeArrows(-0.01f);
                    _filledWidthMusic = GlobalSettings.MediaPlayerVolume * _volumeBoxMusic.Width;
                    break;

                case 1:
                    _assetManager.ChangeSoundVolumeArrows(-0.01f);
                    _filledWidthSound = _assetManager.SoundVolume * _volumeBoxSound.Width;
                    break;
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
                    if (selectedButtonIndex == 3)
                    {
                        return 0;
                    }

                    if (selectedButtonIndex == 2)
                    {
                        MenuControl.IsKeybindingActive = true;
                    }
                }
                break;
            }
        }

        if (selectedButtonIndex != originalIndex)
        {
            _buttons[originalIndex].IsSelected = false;
            _buttons[selectedButtonIndex].IsSelected = true;
        }

        if (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One))
        {
            _buttons[selectedButtonIndex].IsClicked = true;
        }

        if (selectedButtonIndex == 2 && (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One)))
        {
            MenuControl.IsKeybindingActive = true;
        }

        if (_inputManager.IsKeyPressed(Keys.Escape) || _inputManager.IsButtonReleased(Buttons.Start, PlayerIndex.One) || (selectedButtonIndex == 3 && (_inputManager.IsKeyPressed(Keys.Enter) || _inputManager.IsButtonReleased(Buttons.A, PlayerIndex.One))))
        {
            return 0;
        }
        return -1;
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
    /// Draws the menu background, the buttons and the volume visualisation if clicked.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(_tower, Vector2.Zero, Color.White);
        spriteBatch.Draw(_buttonBackground, new Rectangle(100, 230, 1400, 650), new Color(Color.Aqua, 0.5f));
        spriteBatch.DrawString(_assetManager.GetFont("Thaleah"), "Settings", new Vector2(200, 250), Color.Yellow, 0, Vector2.Zero, 6.0f, SpriteEffects.None, 0.5f);
        spriteBatch.Draw(_wrench, new Rectangle(110, 240, _wrench.Width / 5, _wrench.Height / 5), Color.White);
        spriteBatch.Draw(PlayMusic ? _soundOnTexture : _soundOffTexture, new Rectangle(_musicBox.X, _musicBox.Y, _soundOnTexture.Width, _soundOnTexture.Height), Color.White);

        foreach (var button in _buttons)
        {
            button.Draw(spriteBatch);
        }

        DrawVolumeVisualization(spriteBatch, _filledWidthMusic / _volumeBoxMusic.Width, _volumeBoxMusic);
        DrawVolumeVisualization(spriteBatch, _filledWidthSound / _volumeBoxSound.Width, _volumeBoxSound);

        spriteBatch.End();
    }
}
