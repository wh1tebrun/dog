#region File Description
// Abilities.cs
// Store the abilities of the player.
#endregion

using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.HUD;

public class Abilities
{
    private bool _abilityUsed;
    private int _frameCount;

    private readonly Texture2D _shieldIcon;
    private readonly Texture2D _weaponFrame;

    private readonly Texture2D _powerUp; 
    private int _powerUpHeight;

    private float _shieldCooldownRatio = 1f; 

    public Abilities(AssetManager assetManager, GraphicsDevice graphicsDevice)
    {
        _shieldIcon = assetManager.GetTexture("shield");
        _weaponFrame = assetManager.GetTexture("weapon_frame");

            
        _frameCount = 0;
        _powerUpHeight = 55;

            
        _powerUp = new Texture2D(graphicsDevice, 1, 1);
        _powerUp.SetData(new[] { Color.Black * 0.5f });
        
    }
    
    public void SetShieldCooldown(float ratio)
    {
        _shieldCooldownRatio = ratio;
    }

    public void Update()
    {
            
        if (!_abilityUsed)
        {
            _abilityUsed = true;
        }

        if (_powerUpHeight <= 0)
        {
            _powerUpHeight = 55;
        }

        if (_frameCount % 50 == 0 && _powerUpHeight > 0)
        {
            _powerUpHeight -= 11;
            if (_powerUpHeight <= 0)
            {
                _powerUpHeight = 55;
            }
        }
        _frameCount++;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
            
        spriteBatch.Draw(
            _shieldIcon,
            new Vector2(155, 940),
            new Rectangle(0, 0, 28, 28),
            Color.White,
            0f,
            Vector2.Zero,
            2f,
            SpriteEffects.None,
            0f
        );

          
        const int iconSize = 56; 
        var fillHeight = iconSize * (1f - _shieldCooldownRatio);
          

        var offsetY = 940 + (int)(iconSize - fillHeight);

            
        spriteBatch.Draw(
            _powerUp,
            new Rectangle(
                155,         // X
                offsetY,     // Y
                iconSize,    
                (int)fillHeight
            ),
            Color.Black
        );
            
        spriteBatch.Draw(
            _weaponFrame,
            new Rectangle(1685, 915, 105, 80),
            Color.White
        );
        spriteBatch.Draw(
            _weaponFrame,
            new Rectangle(1755, 915, 105, 80),
            Color.White
        );
            
        spriteBatch.End();
    }
}
