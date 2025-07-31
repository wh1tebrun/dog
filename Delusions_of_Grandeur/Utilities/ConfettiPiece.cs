using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur;
public class ConfettiPiece
{
    private readonly Texture2D _texture;
    public Vector2 Position;
    public float Scale = 1;
    public Vector2 Velocity;
    public int TicksToLive = 200;

    public ConfettiPiece(Texture2D texture)
    {
        _texture = texture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (TicksToLive <= 0)
        {
            return;
        }
        spriteBatch.Begin();
        spriteBatch.Draw(_texture, Position, null, Color.White, (float) Math.Atan2(Velocity.X, -Velocity.Y),
            new Vector2(_texture.Width / 2f, _texture.Height / 2f), new Vector2(Scale, Scale), SpriteEffects.None, 0);
        spriteBatch.End();
    }

    public void Update()
    {
        if (TicksToLive <= 0)
        {
            return;
        }
        Position += Velocity;
        TicksToLive--;
        Velocity.Y += 0.05f;
    }
}