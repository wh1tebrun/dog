#region File Description
// Health.cs
// HUD-element for the heath of the players.
#endregion

using System;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.HUD;
public class Health
{
    public int Lives { get; set; } = 3;

    private readonly Texture2D _heartTexture;
    private float _scale = 2.0f;
    private float _time = 0.0f;

    /// <summary>
    /// A class for the hearts.
    /// </summary>
    /// <param name="assetManager"></param>
    public Health(AssetManager assetManager)
    {
        _heartTexture = assetManager.GetTexture("heart");
    }

    public void Update(GameTime gameTime)
    {
        _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _scale = 2.0f + 0.1f * MathF.Sin(_time * 2 * MathF.PI);
    }

    /// <summary>
    /// Removes a heart.
    /// </summary>
    /// <returns></returns>
    public int RemoveHeart()
    {
        Lives -= 1;
        if (Lives <= 0)
        {
            Lives = 0; // otherwise Lives becomes infinitely small in god mode.
            return 0;
        }
        
        return 1;
    }

    /// <summary>
    /// Draws all current hearts.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void DrawHearts(SpriteBatch spriteBatch)
    {
        var heartWidth = _heartTexture.Width;
        const int spacing = 60;
        var totalWidth = Lives * heartWidth + (Lives - 1) * spacing;
        var startX = (1920 - totalWidth) / 2;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        for (int i = 0; i < Lives; i++)
        {
            spriteBatch.Draw(_heartTexture, new Vector2(startX + i * spacing, 1000), new Rectangle(0, 0, 17, 17), Color.White, 0f, new Vector2(_heartTexture.Width / 2f, _heartTexture.Height / 2f), _scale, SpriteEffects.None, 0);
        }

        spriteBatch.End();
    }
}
