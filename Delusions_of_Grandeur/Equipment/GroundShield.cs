using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Equipment;

/// <summary>
/// the shield drawn on the plattform. cann be collected. in this case, it drops current shield and gets new one.
/// </summary>
public class GroundShield
{
    public Weapon.WeaponType Type { get; set; }
    public string Name { get; set; }
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Offset { get; set; }
    public float Scale { get; set; }

    private readonly AssetManager _assetManager;
    private readonly Shield _shield;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="assetManager"></param>
    /// <param name="shield"></param>
    /// /// <param name="techdemo"></param>
    public GroundShield(AssetManager assetManager, Shield shield, bool techdemo)
    {
        _assetManager = assetManager;
        _shield = shield;
        Name = "ranged shield";
        Type = Weapon.WeaponType.Ranged;
        Texture = _assetManager.GetTexture("rangedShield");
        var shieldRectangle = MapManager.WeaponRectangles[1];
        Position = new Vector2(shieldRectangle.X, shieldRectangle.Y);
        Offset = new Vector2(30, 55);
        Scale = 0.2f;

        // in tech demo you begin with ranged shield, because there are no melee enemies.
        if (techdemo)
        {
            SwitchShield();
        }
    }
    
    /// <summary>
    /// switches the shield to the one lying on the floor.
    /// </summary>
    public void SwitchShield()
    {
        if (_shield.Type == Weapon.WeaponType.Melee)
        {
            _shield.ChangeType(Weapon.WeaponType.Ranged);
            Type = Weapon.WeaponType.Melee;
            Texture = _assetManager.GetTexture("meleeShield");
            Name = "melee shield";
        }
        else
        {
            _shield.ChangeType(Weapon.WeaponType.Melee);
            Type = Weapon.WeaponType.Ranged;
            Texture = _assetManager.GetTexture("rangedShield");
            Name = "ranged shield";
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position + Offset, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0.5f);
    }
}