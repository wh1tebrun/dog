#region File Description
// Weapon.cs
// Draw and manage a weapon.
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Equipment;

public class WeaponData
{
	public string Name { get; init; }
    public Texture2D Texture { get; init; }
    public Vector2 Position { get; set; }
    public Vector2 Offset { get; init; }
    public int Damage { get; init; }
    public float FireRate { get; init; }
    public int Speed { get; init; }
    public int Lifespan { get; init; }
    public bool IsActive { get; set; }
    public float ShotCooldown { get; init; }
    public Vector2 HitBoxSize { get; init; }
    public float Scale { get; init; }
}

public class Weapon : GameObject
{
	// Utilities
	private readonly AssetManager _assetManager;
	
	// Integer
	public int CurrentRangedWeaponIndex {get; set;}
	public int CurrentMeleeWeaponIndex{ get; set;} 
	
	// Float
	private readonly float _angle = 0;
	public WeaponType CurrentWeaponType { get; set; } = WeaponType.Ranged;
	
	
	public readonly List<WeaponData> MeleeWeapons;
	public readonly List<WeaponData> RangedWeapons;
	
	private readonly List<Vector2> _weaponPositions;
	
	
	public string Name => GetCurrentWeapon().Name;
	public Texture2D Texture => GetCurrentWeapon().Texture;
	public Vector2 Offset => GetCurrentWeapon().Offset;
	public int Damage => GetCurrentWeapon().Damage;
	public int Speed => GetCurrentWeapon().Speed;
	public int Lifespan => GetCurrentWeapon().Lifespan;
	public float ShotCooldown => GetCurrentWeapon().ShotCooldown;
	public Vector2 HitBoxSize => GetCurrentWeapon().HitBoxSize;
	public float Scale => GetCurrentWeapon().Scale;
	
	/// <summary>
	/// Switches the Weapon Typw between Ranged and Melee.
	/// </summary>
	/// <returns></returns>
	private WeaponData GetCurrentWeapon()
	{
		if (CurrentWeaponType == WeaponType.Ranged)
		{
			return RangedWeapons[CurrentRangedWeaponIndex];
		}
		
		return MeleeWeapons[CurrentMeleeWeaponIndex];
		
	}
	
	public enum WeaponType
	{
		Ranged,
		Melee
	}
	
	public Weapon(AssetManager assetManager)
	{
		_weaponPositions = new List<Vector2>();
		_assetManager = assetManager;
		InitializeWeaponPositions();
		RangedWeapons = InitializeRangedWeapons();
		MeleeWeapons = InitializeMeleeWeapons();
	}
	
	public void InitializeWeaponPositions()
	{
		foreach (var rect in MapManager.WeaponRectangles)
		{
			_weaponPositions.Add(new Vector2(rect.X, rect.Y));
		}
	}
	
	
	/// <summary>
	/// A list of all ranged weapons with their specific attributes.
	/// </summary>
	/// <returns></returns>
	public List<WeaponData> InitializeRangedWeapons()
	{
		return new List<WeaponData>
		{
			new()
			{
				Name = "Greatbow",
				Texture = _assetManager.GetTexture("iconGreatbow"),
				Position = Vector2.Zero,
				Offset = new Vector2(50, 70),
				Damage = 100,
				FireRate = 0.3f,
				Speed = 250,
				Lifespan = 50,
				IsActive = true,
				ShotCooldown = 0.8f,
				HitBoxSize = new Vector2(15, 15),
				Scale = 0.5f
			},
			new()
			{
				Name = "Crossbow",
				Texture = _assetManager.GetTexture("iconCrossbow"),
				Position = _weaponPositions[2],
				Offset = new Vector2(20, 70),
				Damage = 250,
				FireRate = 0.2f,
				Speed = 300,
				Lifespan = 20,
				IsActive = false,
				ShotCooldown = 0.5f,
				HitBoxSize = new Vector2(15, 15),
				Scale = 1f
			}
		};
	}

	/// <summary>
	/// A list of all melee weapons with their specific attributes.
	/// </summary>
	/// <returns></returns>
	public List<WeaponData> InitializeMeleeWeapons()
	{
		return new List<WeaponData>
		{
			new()
			{
				// Weapon 1
				Name = "Saber",
				Texture = _assetManager.GetTexture("iconSaber"),
				Position = Vector2.Zero,
				Offset = new Vector2(40, 70),
				Damage = 90,
				FireRate = 0.2f,
				Speed = 0,
				Lifespan = 1,
				IsActive = true,
				ShotCooldown = 0.5f,
				HitBoxSize = new Vector2(110, 100),
				Scale = 0.6f

			},
			new()
			{
				// Weapon 2
				Name = "Longsword",
				Texture = _assetManager.GetTexture("iconLongsword"),
				Position = _weaponPositions[3],
				Offset = new Vector2(30, 60),
				Damage = 140,
				FireRate = 0.2f,
				Speed = 0,
				Lifespan = 1,
				IsActive = false,
				ShotCooldown = 0.8f,
				HitBoxSize = new Vector2(80, 100),
				Scale = 0.6f

			},
			new()
			{
				// Weapon 3
				Name = "Spear",
				Texture = _assetManager.GetTexture("iconSpear"),
				Position = _weaponPositions[0],
				Offset = new Vector2(30, 55),
				Damage = 220,
				FireRate = 1.0f,
				Speed = 0,
				Lifespan = 1,
				IsActive = false,
				ShotCooldown = 1.3f,
				HitBoxSize = new Vector2(150, 100),
				Scale = 0.6f

			}
		};
	}
	
	/// <summary>
	/// Switches between the ranged weapons.
	/// </summary>
	/// <param name="index"></param>
	public void SwitchRangedWeapon(int index)
	{
		if (index >= 0 && index < RangedWeapons.Count)
		{
			RangedWeapons[CurrentRangedWeaponIndex].IsActive = false;
			RangedWeapons[CurrentRangedWeaponIndex].Position = RangedWeapons[index].Position;
			CurrentRangedWeaponIndex = index;
			RangedWeapons[CurrentRangedWeaponIndex].IsActive = true;
			RangedWeapons[CurrentRangedWeaponIndex].Position = Vector2.Zero;
		}
	}
	
	/// <summary>
	/// Switches between the melee weapons.
	/// </summary>
	/// <param name="index"></param>
	public void SwitchMeleeWeapon(int index)
	{
		if (index >= 0 && index < MeleeWeapons.Count)
		{
			MeleeWeapons[CurrentMeleeWeaponIndex].IsActive = false;
			MeleeWeapons[CurrentMeleeWeaponIndex].Position = MeleeWeapons[index].Position;
			CurrentMeleeWeaponIndex = index;
			MeleeWeapons[CurrentMeleeWeaponIndex].IsActive = true;
			MeleeWeapons[CurrentMeleeWeaponIndex].Position = Vector2.Zero;
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		foreach (var weapon in RangedWeapons)
		{
			if (weapon.IsActive || weapon.Position == Vector2.Zero)
			{
				continue;
			}
			spriteBatch.Draw(weapon.Texture, weapon.Position + weapon.Offset, null, Color.White, _angle, Vector2.Zero, weapon.Scale, SpriteEffects.None, 0.5f);
		}
		foreach (var weapon in MeleeWeapons)
		{
			if (weapon.IsActive) continue;
			spriteBatch.Draw(weapon.Texture, weapon.Position + weapon.Offset, null, Color.White, _angle, Vector2.Zero, weapon.Scale, SpriteEffects.None, 0.5f);
		}
	}
}

