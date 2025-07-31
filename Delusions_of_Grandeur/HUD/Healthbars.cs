using System.Collections.Generic;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.HUD;

/// <summary>
/// a class for both the healthbars of the players.
/// </summary>
public class Healthbars
{
    public List<HealthBar> HealthbarsList { get; set; } = new();

    /// <summary>
    /// adds a healthbar to the list. needed at the beginning in the gamecontrol to initialize the healthbar of a player.
    /// </summary>
    /// <param name="healthBar"></param>
    public void AddHealthbar(HealthBar healthBar)
    {
        HealthbarsList.Add(healthBar);
    }

    /// <summary>
    /// Draws both health bars.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var healthbar in HealthbarsList)
        {
            healthbar.Draw(spriteBatch);
        }
    }
}

/// <summary>
/// one healthbar. has a position and the current healthpoints of the player.
/// </summary>
public class HealthBar
{
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }

    public float CurrentHealth { get; set; }

    public int MaxHealth { get; set; }

    private Vector2 _position;

    private readonly Texture2D _healthBarDecoration;
    private readonly Texture2D _liveTexture;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="assetManager"></param>
    /// <param name="position"></param>
    /// <param name="currentHealth"></param>
    /// <param name="maxHealth"></param>
    public HealthBar(AssetManager assetManager, Vector2 position, float currentHealth, int maxHealth)
    {
        var assetManager1 = assetManager;
        _healthBarDecoration = assetManager1.GetTexture("health_bar_decoration");
        _liveTexture = assetManager1.GetTexture("health_bar");
        _position = position;
        CurrentHealth = currentHealth * 0.01f;
        MaxHealth = maxHealth;
    }

    /// <summary>
    /// Updates the current health bar based on the health points of the player.
    /// </summary>
    /// <param name="healthpoints"></param>
    public void Update(int healthpoints)
    {
        CurrentHealth = healthpoints * 0.01f;
    }

    /// <summary>
    /// Draws the health bar.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        spriteBatch.Draw(_healthBarDecoration, new Vector2(_position.X - _healthBarDecoration.Width, _position.Y), new Rectangle(0, 0, 64, 17), Color.White, 0f, new Vector2(0, 0), 2, SpriteEffects.None, 0);
        
        spriteBatch.Draw(_liveTexture, new Vector2(_position.X - _liveTexture.Width + 13, _position.Y), new Rectangle(0, 0, (int)(49 * (100 * (CurrentHealth / MaxHealth)) ), 17), Color.White, 0f, new Vector2(0, 0), 2, SpriteEffects.None, 0);

        spriteBatch.End();
    }
}
