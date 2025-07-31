using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Equipment;

public class SpeechBubble(string message, AssetManager assetManager, Vector2 position)
{
    public string Message {get; set;} = message;
    private  readonly Texture2D _texture = assetManager.GetTexture("speechbubble"); // gibts noch nicht !!!
    public Vector2 Position {get;set;} = position;
    private readonly SpriteFont _font = assetManager.GetFont("MainMenu");

    public void Update(string message, Vector2 position)
    {
        Message = message;
        Position = position;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        /*spriteBatch.Draw(_texture, Position, Color.White);
        spriteBatch.DrawString(_font, Message, Position, Color.Black);*/
        
        const float scale = 0.8f;

        // Position der Sprechblase
        var bubblePosition = Position;

        // Maß der Sprechblase
        Rectangle bubbleBounds = new Rectangle(
            (int)bubblePosition.X,
            (int)bubblePosition.Y,
            (int)(_texture.Width * scale),
            (int)(_texture.Height * scale)
        );

        // Größe des Textes berechnen
        var textSize = _font.MeasureString(Message);

        // Position des Texts innerhalb der Sprechblase
        Vector2 textPosition = new Vector2(
            bubbleBounds.X + (bubbleBounds.Width - textSize.X * scale) / 2,
            bubbleBounds.Y - 10 + (bubbleBounds.Height - textSize.Y * scale) / 2
        );

        // Sprechblase zeichnen (mit Skalierung)
        spriteBatch.Draw(_texture, bubblePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // Text zeichnen (mit Skalierung)
        spriteBatch.DrawString(_font, Message, textPosition, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

    }
}