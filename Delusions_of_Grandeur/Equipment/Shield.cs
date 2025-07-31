#region File Description
// Shield.cs
// Handle the shield for the player.
#endregion

using System;
using Delusions_of_Grandeur.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Equipment;

public class Shield : GameObject
{
    // Boolean
    public bool IsActive { get; set; }

    // Float
    public float TimeSinceLastActivation { get; set; }

    public float TimeActive { get; set; }
    public float Lifespan { get; set; }
    public float CoolDown { get; set; }
    public readonly float Radius;

    // Vector
    private readonly Vector2 _shieldPositionOffset;
    private readonly Vector2 _positionOffset;
    public Vector2 Centre { get; set; }

    // Entities
    private readonly Player _defensivePlayer;

    // Shield Type
    public Weapon.WeaponType Type { get; set; }

    // Utilities
    private readonly Manager.AssetManager _assetManager;
    private readonly Random _random;
    private Texture2D _texture;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="assetManager"></param>
    /// <param name="player"></param>
    /// <param name="lifetime"></param>
    /// <param name="coolDown"></param>
    /// <param name="positionOffset"></param>
    /// <param name="type"></param>
    public Shield(Manager.AssetManager assetManager, Player player,
        float lifetime, float coolDown, Vector2 positionOffset, Weapon.WeaponType type = Weapon.WeaponType.Melee)
    {
        _assetManager = assetManager;
        _defensivePlayer = player;
        TimeActive = 0f;
        Lifespan = lifetime;
        CoolDown = coolDown;
        _shieldPositionOffset = new Vector2(-80, -70);
        Position = _defensivePlayer.Position + _shieldPositionOffset;
        _positionOffset = positionOffset;
        Centre = _defensivePlayer.Position + _positionOffset;

        ChangeType(type);

        Radius = _texture.Width * 0.5f * 0.5f;

        _random = new Random();
        TimeSinceLastActivation = CoolDown;
    }

    /// <summary>
    /// changes the shield type. needed for ground shield.
    /// </summary>
    /// <param name="type"></param>
    public void ChangeType(Weapon.WeaponType type)
    {
        Type = type;
        if (type == Weapon.WeaponType.Melee)
        {
            _texture = _assetManager.GetTexture("playerShield");
        }
        else
        {
            _texture = _assetManager.GetTexture("playerShield_blue");
        }
    }

    /// <summary>
    /// Sets the shield active.
    /// </summary>
    public void SetActive()
    {
        if (TimeSinceLastActivation < CoolDown) return;

        IsActive = true;
        TimeActive = 0f;
        TimeSinceLastActivation = 0f;
        _assetManager.GetSound("shield");
        _assetManager.PlaySound("shield");
    }

    /// <summary>
    /// Update logic for the shield.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        TimeSinceLastActivation += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (IsActive)
        {
            TimeActive += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeActive >= Lifespan)
            {
                IsActive = false;
                TimeSinceLastActivation = 0f; // Reset cooldown timer when shield deactivates
            }
        }

        Position = _defensivePlayer.Position + _shieldPositionOffset;
        Centre = _defensivePlayer.Position + _positionOffset;
    }

    /// <summary>
    /// Checks if a projectile collides with the shield. It will collide if a projectile is within the radius of the shield.
    /// Returns true if collides, else false.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool CollidesWithProjectile(Vector2 position, Weapon.WeaponType type)
    {
        if (IsActive && type == Type)
        {
            var distance = Vector2.Distance(position, Centre);
            if (distance <= Radius)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// A function to calculate with a random factor whether the projectile should be completely blocked, not blocked at all, or just reduced in damage.
    /// Returns a float between 0 and 1. The higher the number, the less damage is blocked.
    /// </summary>
    /// <returns></returns>
    public float SetDamageToProjectile()
    {
        var randomNumber = _random.Next(100);
        switch (randomNumber)
        {
            case < 40:
                // Damage should be completely blocked.
                _defensivePlayer.WriteBlockedText("Full Blocked");
                return 0;
            case < 60:
                // Shield does not block any damage.
                return 1;
            default:
                // Shield should block some damage to the projectile, but not all.
                _defensivePlayer.WriteBlockedText("Blocked");
                return (float)_random.NextDouble();
        }
    }

    /// <summary>
    /// Draws the shield if active.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(0.5f, 0.5f),
                SpriteEffects.None,
                0f
            );
        }
    }
}
