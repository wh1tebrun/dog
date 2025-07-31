using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.HUD;

public class Timer(AssetManager assetManager)
{
    public double ElapsedTime;
    private readonly SpriteFont _font = assetManager.GetFont("MainMenu");

    public int Minutes {get; set;}
    public int Seconds {get; set;}

    public void Update(GameTime gameTime)
    {
        ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

        Minutes = (int)ElapsedTime / 60;
        Seconds = (int)ElapsedTime % 60;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(_font, $"{Minutes:00}:{Seconds:00}", new Vector2(1700, 50), Color.White);
        spriteBatch.End();
    }
}
