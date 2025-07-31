using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hausaufgabe;

public class Game1 : Game
{
    private Texture2D _uniLogo;
    private Texture2D _hintergrundbild;
    private SoundEffect _logoGetroffen;
    private SoundEffect _logoNichtGetroffen;
    private Vector2 _uniLogoPosition;
    private Vector2 _hintergrundbildPosition;
    private Vector2 _uniLogoSkalierung;
    private Vector2 _kreisMittelpunkt;
    private float _kreisRadius;
    private float _uniLogoWinkel;
    private float _deltaZuLogoMittelpunkt;
    private float _geschwindigkeitLogo;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _hintergrundbildPosition = new Vector2(0, 0);
        _uniLogoSkalierung = new Vector2(.15f, .15f);
        _geschwindigkeitLogo = 0.0008f;
        _kreisMittelpunkt = new Vector2(640f, 512f);
        _kreisRadius = 200f;
        _uniLogoWinkel = 0f;
        _deltaZuLogoMittelpunkt = 67f;
        _uniLogoPosition = KoordinatenLogoBerechnen(_uniLogoWinkel);
    }

    protected override void Initialize()
    {
        // Fenstergroesse festlegen
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 1024;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _uniLogo = Content.Load<Texture2D>("Unilogo");
        _hintergrundbild = Content.Load<Texture2D>("Background");
        _logoGetroffen = Content.Load<SoundEffect>("Logo_hit");
        _logoNichtGetroffen = Content.Load<SoundEffect>("Logo_miss");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            //Pruefe, ob die Koordinaten der Maus auf dem Logo sind
            if (MouseIsInside(new Vector2(mouseState.X, mouseState.Y), _uniLogoPosition))
            {
                //Spiele Sound, Objekt getroffen
                _logoGetroffen.Play();
            }
            else
            {
                //Spiele Sound, Objekt nicht getroffen
                _logoNichtGetroffen.Play();
            }
        }
        _uniLogoWinkel += (gameTime.ElapsedGameTime.Milliseconds) * _geschwindigkeitLogo;
        _uniLogoPosition = KoordinatenLogoBerechnen(_uniLogoWinkel);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Zeichne die Bilder auf das Fenster
        _spriteBatch.Begin();
        _spriteBatch.Draw(_hintergrundbild, _hintergrundbildPosition, Color.White);
        _spriteBatch.Draw(_uniLogo, _uniLogoPosition, null, Color.White, 0f, Vector2.Zero, _uniLogoSkalierung,
            SpriteEffects.None, 0f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    /// <summary>
    /// Berechnet anhand des Winkels die Position des Logo-Mittelpunktes auf der Kreisbahn um die Fenstermitte.
    /// </summary>
    /// <param name="winkel">Winkel zum Mittelpunkt</param>
    /// <returns>Koordinaten für das Unilogo auf der Kreisbahn um den Mittelpunkt.</returns>
    Vector2 KoordinatenLogoBerechnen(float winkel)
    {
        return new Vector2((_kreisMittelpunkt.X + (float)Math.Cos(_uniLogoWinkel)*_kreisRadius) - _deltaZuLogoMittelpunkt, (_kreisMittelpunkt.Y + (float)Math.Sin(_uniLogoWinkel)*_kreisRadius) - _deltaZuLogoMittelpunkt);
    }
    /// <summary>
    /// Gibt zurück, ob die Position der Maus innerhalb des Logos ist.
    /// </summary>
    /// <param name="mousePosition">Positions-Vektor der Maus</param>
    /// <param name="logoPosition">Aktuelle Position des Logos </param>
    /// <returns>Wenn die Maus innerhalb des Logos ist, wird "true" zurückgegeben, sonst "false".</returns>
    bool MouseIsInside(Vector2 mousePosition, Vector2 logoPosition)
    {
        if (Vector2.Distance(mousePosition, new Vector2(logoPosition.X + _deltaZuLogoMittelpunkt, logoPosition.Y + _deltaZuLogoMittelpunkt)) <= _deltaZuLogoMittelpunkt)
        {
            return true;
        }else
        {
            return false;
        }
        
    }
}