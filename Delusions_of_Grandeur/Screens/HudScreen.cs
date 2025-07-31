using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Screens;

public class HudScreen : IScreen
{
    private readonly Health _health;
    private readonly Healthbars _healthBar;
    private Timer _timer;
    private readonly Abilities _abilities;
    private readonly Potion _potion;
    private readonly Player _playerOffensive;
    private readonly Player _playerDefensive;
    private readonly Shield _shield;

    private readonly Texture2D _shieldIconHud;
    private readonly Texture2D _invisabilityIconHud;
    private readonly Texture2D _potionIconHudEmpty;
    private readonly Texture2D _potionIconHudFull;

    public bool UpdateLower => true;
    public bool DrawLower => true;

    /// <summary>
    /// Constructor.
    /// </summary>
    public HudScreen(
        Health health,
        Healthbars healthBar,
        Abilities abilities,
        Timer timer,
        Potion potion,
        Player playerOffensive,
        Player playerDefensive,
        Shield shield,
        AssetManager assetManager 
    )
    {
        _health = health;
        _healthBar = healthBar;
        _timer = timer;
        _abilities = abilities;
        _potion = potion;
        _playerOffensive = playerOffensive;
        _playerDefensive = playerDefensive;
        _shield = shield;

        _shieldIconHud = assetManager.GetTexture("shield");
        _invisabilityIconHud = assetManager.GetTexture("ghost");
        _potionIconHudEmpty = assetManager.GetTexture("potion_empty");
        _potionIconHudFull = assetManager.GetTexture("potion_full");
    }

    public void LoadContent()
    {
    }

    public void UnloadContent()
    {
    }

    public int Update(GameTime gameTime)
    {
        _timer.Update(gameTime);
        _health.Update(gameTime);
        _abilities.Update();

            
        float shieldRatio;

        if (_shield.IsActive)
        {
                
            shieldRatio = 1f - _shield.TimeActive / _shield.Lifespan;
        }
        else
        {
                
            if (_shield.TimeSinceLastActivation < _shield.CoolDown)
            {
                shieldRatio = _shield.TimeSinceLastActivation / _shield.CoolDown;
            }
            else
            {
                shieldRatio = 1f;
            }
        }

        if (shieldRatio < 0f) shieldRatio = 0f;
        if (shieldRatio > 1f) shieldRatio = 1f;

        _abilities.SetShieldCooldown(shieldRatio);

        return -1;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        _health.DrawHearts(spriteBatch);
        _healthBar.Draw(spriteBatch);
        _timer.Draw(spriteBatch);
        _abilities.Draw(spriteBatch);

        spriteBatch.Begin();
        // Potion.Draw(spriteBatch);
        if (_potion.NumberOfPotions > 0)
        {
            spriteBatch.Draw(
                _potionIconHudFull, new Vector2(85, 935), new Rectangle(0, 0, _potionIconHudFull.Width, _potionIconHudFull.Height),
                Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.DrawString(_potion.Font, $"{_potion.NumberOfPotions}", new Vector2(105, 970), Color.White * 0.75f, 0f,
                Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
        // If there are no potions in inventory.
        else
        {
            spriteBatch.Draw(_potionIconHudEmpty, new Vector2(90, 940), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f,
                SpriteEffects.None, 0);
            spriteBatch.DrawString(_potion.Font, $"{_potion.NumberOfPotions}", new Vector2(105, 970), Color.White * 0.5f, 0f,
                Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }

            
        var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
            
        float defensiveInvisRatio;
        var defensiveInvisBarPos = new Vector2(250, 1010);
        const float defensiveBarWidth = 125f;
        const float defensiveBarHeight = 16f;

        if (_playerDefensive.IsInvisible)
        {
            var maxDurDef = _playerDefensive.InvisibilityDuration;
            var timeLeftDef = _playerDefensive.InvisibilityTimeLeft;
            defensiveInvisRatio = timeLeftDef / maxDurDef;
        }
        else
        {
            var maxCdDef = _playerDefensive.InvisibilityCooldown;
            var cdLeftDef = _playerDefensive.InvisibilityTimeLeft;
            defensiveInvisRatio = 1f - (cdLeftDef / maxCdDef);
        }

        if (defensiveInvisRatio < 0f) defensiveInvisRatio = 0f;
        if (defensiveInvisRatio > 1f) defensiveInvisRatio = 1f;

        var defensiveInvisFillWidth = defensiveBarWidth * defensiveInvisRatio;

            
        spriteBatch.Draw(
            pixel,
            new Rectangle(
                (int)defensiveInvisBarPos.X,
                (int)defensiveInvisBarPos.Y,
                (int)defensiveBarWidth,
                (int)defensiveBarHeight
            ),
            Color.DarkGray * 0.3f
        );

            
        spriteBatch.Draw(
            pixel,
            new Rectangle(
                (int)defensiveInvisBarPos.X,
                (int)defensiveInvisBarPos.Y,
                (int)defensiveInvisFillWidth,
                (int)defensiveBarHeight
            ),
            Color.Gray
        );
            
        const int iconSize = 32;
        var iconPosY = (int)(defensiveInvisBarPos.Y - (iconSize / 2f - defensiveBarHeight / 2f));
            
        var iconPosX = (int)defensiveInvisBarPos.X - iconSize + 9;
            
        spriteBatch.Draw(
            _invisabilityIconHud,
            new Rectangle(iconPosX, iconPosY, iconSize, iconSize),
            Color.White
        );
            
        float offensiveInvisRatio;
        Vector2 offensiveInvisBarPos = new Vector2(1555, 1010); 
        var offensiveBarWidth  = 125f;
        var offensiveBarHeight = 16f;
            
        if (_playerOffensive.IsInvisible)
        {
            var maxDurOff = _playerOffensive.InvisibilityDuration; 
            var timeLeftOff = _playerOffensive.InvisibilityTimeLeft;
            offensiveInvisRatio = timeLeftOff / maxDurOff;
        }
        else
        {
            var maxCdOff = _playerOffensive.InvisibilityCooldown; 
            var cdLeftOff = _playerOffensive.InvisibilityTimeLeft;
            offensiveInvisRatio = 1f - cdLeftOff / maxCdOff;
        }

        if (offensiveInvisRatio < 0f) offensiveInvisRatio = 0f;
        if (offensiveInvisRatio > 1f) offensiveInvisRatio = 1f;

        var offensiveInvisFillWidth = offensiveBarWidth * offensiveInvisRatio;

            
        spriteBatch.Draw(
            pixel,
            new Rectangle(
                (int)offensiveInvisBarPos.X,
                (int)offensiveInvisBarPos.Y,
                (int)offensiveBarWidth,
                (int)offensiveBarHeight
            ),
            Color.DarkGray * 0.3f
        );

            
        spriteBatch.Draw(
            pixel,
            new Rectangle(
                (int)offensiveInvisBarPos.X,
                (int)offensiveInvisBarPos.Y,
                (int)offensiveInvisFillWidth,
                (int)offensiveBarHeight
            ),
            Color.Gray
        );
            
        int offensiveIconSize = 32;

        int offensiveIconPosY = (int)(offensiveInvisBarPos.Y - (offensiveIconSize / 2f - offensiveBarHeight / 2f));

        var offensiveIconPosX = (int)offensiveInvisBarPos.X - offensiveIconSize + 9;

        spriteBatch.Draw(
            _invisabilityIconHud,
            new Rectangle(offensiveIconPosX, offensiveIconPosY, offensiveIconSize, offensiveIconSize),
            Color.White
        );
        var angle = 0f;
        var offset = new Vector2(0, 0);

        
        _playerOffensive.Weapon.CurrentWeaponType = Weapon.WeaponType.Melee;
        switch (_playerOffensive.Weapon.Name)
        {

            case "Longsword":
                angle = 45f;
                offset = new Vector2(85, -5);
                break;
            case "Saber":
                angle = 90f;
                offset = new Vector2(100, 10);
                break;
            case "Spear":
                angle = 250f;
                offset = new Vector2(40, 75);
                break;
        }
        spriteBatch.Draw(_playerOffensive.Weapon.Texture, new Vector2(1695, 855) + _playerOffensive.Weapon.Offset + offset,
            null, Color.White, angle, Vector2.Zero, _playerOffensive.Weapon.Scale, SpriteEffects.None, 0.5f);
        
        _playerOffensive.Weapon.CurrentWeaponType = Weapon.WeaponType.Ranged;
        switch (_playerOffensive.Weapon.Name)
        {
            case "Greatbow":
                angle = 45f;
                offset = new Vector2(-65, -35);
                break;
            case "Crossbow":
                angle = 180f;
                offset = new Vector2(-15, 50);
                break;
        }

        spriteBatch.Draw(_playerOffensive.Weapon.Texture, new Vector2(1745, 880) + _playerOffensive.Weapon.Offset + offset, null, Color.White, angle, Vector2.Zero, _playerOffensive.Weapon.Scale, SpriteEffects.None, 0.5f);
        
        

        _playerOffensive.Weapon.CurrentWeaponType = _playerOffensive.LastUsedWeaponType;
        spriteBatch.End();
    }
}
