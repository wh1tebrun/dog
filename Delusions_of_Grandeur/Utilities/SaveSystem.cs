#region File Description
// SaveSystem.cs
// System for saving game objects.
#endregion

using System.Text.Json;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Equipment;
using Delusions_of_Grandeur.HUD;
using Delusions_of_Grandeur.Manager;
using Delusions_of_Grandeur.Screens;

namespace Delusions_of_Grandeur.Utilities;

/// <summary>
/// An interface to hold common data properties.
/// All entities that need to save basic properties can implement this.
/// </summary>

/// <summary>
/// Special properties for Health.
/// </summary>
public class HealthData
{
    public int Lives { get; init; }
}

public class LineSaveData
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float ShrinkRate { get; set; }
    public bool IsShrinking { get; set; }
    public bool IsActive { get; set; }
    public bool IsWarning { get; set; }
    public float WarningTime { get; set; }
    public int ColorR { get; set; }
    public int ColorG { get; set; }
    public int ColorB { get; set; }
    public int ColorA { get; set; }
}

public class EndBossData
{
    public float PositionX { get; init; }
    public float PositionY { get; init; }
    public float TimeSinceLastAttack { get; set; }
    public float TimeSinceLastImmune { get; set; }
    public float TimeSinceLastHealing { get; set; }
    public float TimeSinceLastHit { get; set; }
    public float EbHealingDuration { get; set; }
    public float EbImmuneDuration { get; set; }
    public float TimeSinceLastGlobalSkill { get; set; }
    public float TimeSinceDamageBuffActivation { get; set; }
    public float TimeSinceDeath { get; set; }
    public int HealingValue { get; set; }
    public bool IsHealing { get; set; }
    public bool IsImmune { get; set; }
    public bool GlobalSkillIsActive { get; set; }
    public bool DamageBuffIsActive { get; set; }
    public bool DamageBuffAnimationRunning { get; set; }
    public bool RangeAttackActive { get; set; }
    public bool MeleeAttackActive { get; set; }
    public bool FlipAnimation { get; set; }
    public bool EbIsAlive { get; set; }
    public bool DeathAnimationRunning { get; set; }
    public int GameObjectHealthPoints { get; set; }

    public double TimeSinceLastUpdate {get; set;}
}

public class CameraData
{
    public float CentreX { get; init; }
    public float CentreY { get; init; }
    public float CurrentPositionX { get; init; }
    public float CurrentPositionY { get; init; }
    public float ShakeDuration { get; init; }
    public float ShakeIntensity { get; init; }
    public float LowerBoundary { get; init; }
    public int CounterInBeginning { get; init; }

    public float M11 { get; init; }
    public float M12 { get; init; }
    public float M13 { get; init; }
    public float M14 { get; init; }
    public float M21 { get; init; }
    public float M22 { get; init; }
    public float M23 { get; init; }
    public float M24 { get; init; }
    public float M31 { get; init; }
    public float M32 { get; init; }
    public float M33 { get; init; }
    public float M34 { get; init; }
    public float M41 { get; init; }
    public float M42 { get; init; }
    public float M43 { get; init; }
    public float M44 { get; init; }
}


public class MusicData
{
    public string MusicKey { get; init; }
    public float Volume { get; init; }
    public bool IsPlaying { get; init; }
}

public class TimerData
{ 
    public double ElapsedTime { get; init; }
}

public class ShieldData
{
    public bool IsActive { get; init; }
    public float TimeSinceLastActivation { get; init; }
    public float TimeActive { get; init; }
    public float Lifespan { get; init; }
    public float CoolDown { get; init; }
    
    public float CentreX { get; init; }
    public float CentreY { get; init; }
}
public class HealthBarData
{
    public float HealthBarPositionX { get; init; }
    public float HealthBarPositionY { get; init; }
    public int HealthPoints { get; init; }
}

/// <summary>
/// Special properties for Player.
/// </summary>
public class PlayerData
{
    public int GameObjectHealthpoints { get; init; }
    public float PositionX { get; init; }
    public float PositionY { get; init; }
    public bool IsAlive { get; init; }
    public float JumpVelocity { get; init; }
    public bool IsOnGround { get; init; }
    public float TimeSinceLastShot { get; init; }
    public float TimeSinceLastHit { get; init; }
    public float DirectionX { get; init; }
    public float DirectionY { get; init; }
    public string Name { get; init; }
    public bool Flipped { get; init; }
    public bool HasAttackSystem { get; init; }
    public bool IsInvisible { get; init; }
    public float InvisibilityTimeLeft { get; init; }
    public bool IsDebugMode { get; init; }
    public bool GodMode { get; init; }
    public bool OnEndBossMap { get; init; }
    public bool TechDemo { get; init; }
    public float InvisibilityDuration { get; init; }
    public float InvisibilityCooldown { get; init; }
    public float TimeSinceLastComment { get; init; }
    public bool IsInCheckPoint { get; init; }
}


/// <summary>
/// Special properties for MeleeEnemy.
/// </summary>
public class MeleeEnemyData
{
    public float PositionX { get; init; }
    public float PositionY { get; init; }
    public bool IsAlive { get; init; }
    public float Speed { get; init; }
    public float MoveTimeX { get; init; }
    public float MoveTimeY { get; init; }
    public float MaxMoveTime { get; init; }
    public int MoveDirectionX { get; init; }
    public int MoveDirectionY { get; init; }
    public bool IsMovingInXDirection { get; init; }
    public bool IsOnGround { get; init; }
    public bool Flipped { get; init; }
    public bool IsExploding { get; init; }
    public float ExplosionAnimationTimer { get; init; }
    public bool AnimationWasFlippedBecauseOfPlayer { get; init; }
    public bool ProjectileSpawned { get; init; }
    public float ExplosionDelay { get; init; }
    public float ExplosionShortFactor { get; init; }
    public int MapIndex { get; init; }
    public int Damage { get; init; }
}


public class PlayerProjectileData
{
    public float MobPositionX { get; init; }
    public float MobPositionY { get; init; }
    public float TargetPositionX { get; init; }
    public float TargetPositionY { get; init; }
    public int Speed { get; init; }
    public int Damage { get; init; }
    public float LifeSpan { get; init; }

    public string OwnerId { get; init; }
    
    public float DirectionX { get; init; }
    public float DirectionY { get; init; }
}

/// <summary>
/// Special properties for RangedEnemy.
/// </summary>
public class RangedEnemyData
{
    public float PositionX { get; init; }
    public float PositionY { get; init; }
    public bool IsAlive { get; init; }
    public float Speed { get; init; }
    public float MoveTimeX { get; init; }
    public float MoveTimeY { get; init; }
    public float MaxMoveTime { get; init; }
    public float TimeSinceLastShot { get; init; }
    public float ShotCooldown { get; init; }
    public int MoveDirectionX { get; init; }
    public int MoveDirectionY { get; init; }
    public bool IsMovingInXDirection { get; init; }
    public int Health { get; init; }
    public bool HasAttackSystem { get; init; }
    public bool TechDemo { get; init; }
    public float PrevPositionX { get; init; }
    public float PrevPositionY { get; init; }
    public float StartPositionX { get; init; }
    public float StartPositionY { get; init; }
    public double TimeSinceLastUpdate { get; init; }
    public int MapIndex { get; init; }
}

public class PotionEndBossMapSaveData
{
    public int NumberOfPotions { get; init; }
    public List<float> PositionX { get; init; }
    public List<float> PositionY { get; init; }

    public List<float> PotionRectanglesX { get; init; }
    public List<float> PotionRectanglesY { get; init; }

}

public class PotionSaveData
{
    public int NumberOfPotions { get; init; }
    
    public List<float> PositionX { get; init; }
    public List<float> PositionY { get; init; }

    public List<float> PotionRectanglesX { get; init; }
    public List<float> PotionRectanglesY { get; init; }
}
public class WeaponSaveData
{
    public string Name { get; set; }
    public string TextureKey { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public int Damage { get; set; }
    public float FireRate { get; set; }
    public int Speed { get; set; }
    public int Lifespan { get; set; }
    public bool IsActive { get; set; }
    public float ShotCooldown { get; set; }
    public float HitBoxSizeX { get; set; }
    public float HitBoxSizeY { get; set; }
    public float Scale { get; set; }
    public bool IsRanged { get; set; }
}

public class AllWeaponsSave
{
    public int CurrentRangedWeaponIndex { get; set; }
    public int CurrentMeleeWeaponIndex { get; set; }
    public List<WeaponSaveData> Weapons { get; set; }
}

public class ShieldSaveData
{
    public string Name { get; set; }
    public string TextureKey { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public float Scale { get; set; }
    public bool IsActive { get; set; }
    public float TimeSinceLastActivation { get; set; }
    public float TimeActive { get; set; }
    public float Lifespan { get; set; }
    public float CoolDown { get; set; }
    public Weapon.WeaponType Type { get; set; }
}

public class AllShieldsSave
{
    public List<ShieldSaveData> Shields { get; set; }
}

static class SaveSlotManager
{
    private static string FilePath = Path.Combine("Content", "slots.json");
    public static HashSet<int> Slots { get; private set; } = new HashSet<int>();

    public static void LoadSlots()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            Slots = JsonConvert.DeserializeObject<HashSet<int>>(json) ?? new HashSet<int>();
        }
    }

    public static void SaveSlots()
    {
        string json = JsonConvert.SerializeObject(Slots, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }
}

public class SaveSystem(GameControl gameControl)
{
    public const int MaxSlot = 3;
    private readonly string _savePath = Path.Combine(Directory.GetCurrentDirectory(), "Saves");

    public void SaveGame(int slotIndex)
    {
        SaveSlotManager.Slots.Add(slotIndex + 1);
        SaveSlotManager.SaveSlots();

        SaveRangedEnemies("RangedEnemiesSave" + slotIndex + ".json");
        SavePlayers("PlayersSave" + slotIndex + ".json");
        SaveMeleeEnemies("MeleeEnemiesSave" + slotIndex + ".json");
        SaveHealth("HealthSave" + slotIndex + ".json");
        SavePlayerProjectile("PlayerProjectileSave" + slotIndex + ".json");
        SaveRangedEnemyProjectile("RangedEnemyProjectileSave" + slotIndex + ".json");
        SaveHealthBar("HealthbarSave" + slotIndex + ".json");
        SaveTimer("TimerSave" + slotIndex + ".json");
        SaveShield("ShieldSave" + slotIndex + ".json");
        SaveMusic("MusicSave" + slotIndex + ".json");
        SaveCamera("CameraSave" + slotIndex + ".json");
        SavePotions("PotionSave" + slotIndex + ".json");
        SavePotionsEndBossMap("PotionEndBossMapSave" + slotIndex + ".json");
        SaveWeapons("WeaponsSave" + slotIndex + ".json");
        SaveShields("ShieldsSave" + slotIndex + ".json");
        SaveEndBoss("EndBossSave" + slotIndex + ".json");
        SaveLines("LinesSave" + slotIndex + ".json");
        SaveEndBossProjectiles("EndBossProjectiles" + slotIndex + ".json");
        SavePlayerProjectilesOnEndBossMap("EndBossPlayerProjectiles" + slotIndex + ".json");
    }

    public void LoadGame(int slotIndex)
    {
        SaveSlotManager.LoadSlots();

        if (!SaveSlotManager.Slots.Contains(slotIndex + 1))
        {
            return;
        }
        LoadPlayers("PlayersSave" + slotIndex + ".json");
        LoadHealth("HealthSave" + slotIndex + ".json");
        LoadProjectile("PlayerProjectileSave" + slotIndex + ".json");
        LoadProjectile("EndBossProjectileSave" + slotIndex + ".json");
        LoadHealthBar("HealthbarSave" + slotIndex + ".json");
        LoadTimer("TimerSave" + slotIndex + ".json");
        LoadShield("ShieldSave" + slotIndex + ".json");
        LoadMusic("MusicSave" + slotIndex + ".json");
        LoadCamera("CameraSave" + slotIndex + ".json");
        LoadWeapons("WeaponsSave" + slotIndex + ".json");
        LoadShields("ShieldsSave" + slotIndex + ".json");
        LoadPotions("PotionSave" + slotIndex + ".json");
        LoadRangedEnemies("RangedEnemiesSave" + slotIndex + ".json");
        LoadMeleeEnemies("MeleeEnemiesSave" + slotIndex + ".json");
        LoadProjectile("RangedEnemyProjectileSave" + slotIndex + ".json");
        LoadEndBoss("EndBossSave" + slotIndex + ".json");
        LoadLines("LinesSave" + slotIndex + ".json");
        LoadEndBossProjectiles("EndBossProjectiles" + slotIndex + ".json");
        LoadPlayerProjectilesOnEndBossMap("EndBossPlayerProjectiles" + slotIndex + ".json");
        LoadPotionsEndBossMap("PotionEndBossMapSave" + slotIndex + ".json");
    }

        /// <summary>
        /// A generic save function.
        /// Takes a data list and writes it to JSON.
        /// </summary>
        private void SaveToFile<T>(List<T> dataList, string fileName)
        {
            if (!Directory.Exists(_savePath))
                Directory.CreateDirectory(_savePath);

            var filePath = Path.Combine(_savePath, fileName);
            var json = System.Text.Json.JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        private void ClearFile(string fileName)
        {
            var filePath = Path.Combine(_savePath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
    /// <summary>
    /// A generic load function.
    /// Reads from a file and returns a list.
    /// </summary>
    private List<T> LoadFromFile<T>(string fileName)
    {
        var filePath = Path.Combine(_savePath, fileName);
        if (!File.Exists(filePath))
            return null;

        var json = File.ReadAllText(filePath);
        return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json);
    }
    public void SaveShields(string fileName)
    {
        var shieldDataList = new List<ShieldSaveData>();

        // Save the player's active shield
        var playerShield = gameControl.PlayerDefensive.Shield;
        shieldDataList.Add(new ShieldSaveData
        {
            Name = playerShield.Type.ToString(),
            TextureKey = playerShield.Type == Weapon.WeaponType.Melee ? "playerShield" : "playerShield_blue",
            PositionX = playerShield.Centre.X,
            PositionY = playerShield.Centre.Y,
            OffsetX = 0,
            OffsetY = 0,
            Scale = 1.0f,
            IsActive = playerShield.IsActive,
            TimeSinceLastActivation = playerShield.TimeSinceLastActivation,
            TimeActive = playerShield.TimeActive,
            Lifespan = playerShield.Lifespan,
            CoolDown = playerShield.CoolDown,
            Type = playerShield.Type
        });

        // Save the ground shield
        var groundShield = gameControl.PlayerDefensive.GroundShield;
        shieldDataList.Add(new ShieldSaveData
        {
            Name = groundShield.Name,
            TextureKey = groundShield.Type == Weapon.WeaponType.Melee ? "meleeShield" : "rangedShield",
            PositionX = groundShield.Position.X,
            PositionY = groundShield.Position.Y,
            OffsetX = groundShield.Offset.X,
            OffsetY = groundShield.Offset.Y,
            Scale = groundShield.Scale,
            IsActive = true,
            Type = groundShield.Type
        });

        SaveToFile(new List<AllShieldsSave> { new AllShieldsSave { Shields = shieldDataList } }, fileName);
    }

    public void LoadShields(string fileName)
    {
        var shieldSaveList = LoadFromFile<AllShieldsSave>(fileName);
        if (shieldSaveList == null || shieldSaveList.Count == 0)
            return;

        var loadedShields = shieldSaveList[0].Shields;
        if (loadedShields.Count < 2)
            return;

        // Load player's shield
        var playerShield = gameControl.PlayerDefensive.Shield;
        var shieldData = loadedShields[0];
        playerShield.Centre = new Vector2(shieldData.PositionX, shieldData.PositionY);
        playerShield.IsActive = shieldData.IsActive;
        playerShield.TimeSinceLastActivation = shieldData.TimeSinceLastActivation;
        playerShield.TimeActive = shieldData.TimeActive;
        playerShield.Lifespan = shieldData.Lifespan;
        playerShield.CoolDown = shieldData.CoolDown;
        playerShield.ChangeType(shieldData.Type);

        // Load ground shield
        var groundShield = gameControl.PlayerDefensive.GroundShield;
        var groundShieldData = loadedShields[1];
        groundShield.Position = new Vector2(groundShieldData.PositionX, groundShieldData.PositionY);
        groundShield.Offset = new Vector2(groundShieldData.OffsetX, groundShieldData.OffsetY);
        groundShield.Scale = groundShieldData.Scale;
        groundShield.Name = groundShieldData.Name;
        groundShield.Type = groundShieldData.Type;
        groundShield.Texture = gameControl.AssetManager.GetTexture(groundShieldData.TextureKey);
    }

    private void SaveMusic(string fileName)
    {
        var musicDataList = new List<MusicData>();

        Song currentSong = gameControl.GetType()
                                      .GetField("_backgroundMusicPlaying",
                                                System.Reflection.BindingFlags.NonPublic |
                                                System.Reflection.BindingFlags.Instance)
                                      ?.GetValue(gameControl) as Song;

        if (currentSong != null)
        {
            var bgMusicDict = gameControl.AssetManager
                .GetType()
                .GetField("_backgroundMusic", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(gameControl.AssetManager) as Dictionary<string, Song>;

            string foundKey = null;
            if (bgMusicDict != null)
            {
                foreach (var kv in bgMusicDict)
                {
                    if (kv.Value != currentSong) continue;
                    foundKey = kv.Key;
                    break;
                }
            }

            bool isPlaying = MediaPlayer.State == MediaState.Playing;
            var volume = MediaPlayer.Volume;

            musicDataList.Add(new MusicData
            {
                MusicKey = foundKey,
                Volume = volume,
                IsPlaying = isPlaying
            });
        }
        SaveToFile(musicDataList, fileName);
    }
    
    private void LoadMusic(string fileName)
    {
        var musicDataList = LoadFromFile<MusicData>(fileName);
        if (musicDataList == null || musicDataList.Count == 0)
            return;

        var data = musicDataList[0];
        if (string.IsNullOrEmpty(data.MusicKey))
            return;

        Song loadedSong = gameControl.AssetManager.GetMusic(data.MusicKey);
        if (loadedSong == null)
            return;

        var field = gameControl.GetType().GetField("_backgroundMusicPlaying",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(gameControl, loadedSong);
        }
        MediaPlayer.Volume = data.Volume;

        if (data.IsPlaying)
        {
            MediaPlayer.Play(loadedSong);
        }
        else
        {
            MediaPlayer.Stop();
        }
    }

    private void SaveCamera(string fileName)
    {
        var cam = gameControl.Camera;
        var cameraDataList = new List<CameraData>();

        var data = new CameraData
        {
            CentreX = cam.Centre.X,
            CentreY = cam.Centre.Y,
            CurrentPositionX = cam.CurrentPosition.X,
            CurrentPositionY = cam.CurrentPosition.Y,
            ShakeDuration = cam.ShakeDuration,
            ShakeIntensity = cam.ShakeIntensity,
            LowerBoundary = cam.LowerBoundary,
            CounterInBeginning = cam.CounterInBeginning,
            M11 = cam.Transform.M11,
            M12 = cam.Transform.M12,
            M13 = cam.Transform.M13,
            M14 = cam.Transform.M14,
            M21 = cam.Transform.M21,
            M22 = cam.Transform.M22,
            M23 = cam.Transform.M23,
            M24 = cam.Transform.M24,
            M31 = cam.Transform.M31,
            M32 = cam.Transform.M32,
            M33 = cam.Transform.M33,
            M34 = cam.Transform.M34,
            M41 = cam.Transform.M41,
            M42 = cam.Transform.M42,
            M43 = cam.Transform.M43,
            M44 = cam.Transform.M44
        };

        cameraDataList.Add(data);
        SaveToFile(cameraDataList, fileName);
    }

    private void LoadCamera(string fileName)
    {
        var dataList = LoadFromFile<CameraData>(fileName);
        if (dataList == null || dataList.Count == 0)
            return;

        var camData = dataList[0];
        var cam = gameControl.Camera;

        cam.Centre = new Vector2(camData.CentreX, camData.CentreY);
        cam.CurrentPosition = new Vector2(camData.CurrentPositionX, camData.CurrentPositionY);
        cam.ShakeDuration = camData.ShakeDuration;
        cam.ShakeIntensity = camData.ShakeIntensity;
        cam.LowerBoundary = camData.LowerBoundary;
        cam.CounterInBeginning = camData.CounterInBeginning;

        cam.Transform = new Matrix(
            camData.M11, camData.M12, camData.M13, camData.M14,
            camData.M21, camData.M22, camData.M23, camData.M24,
            camData.M31, camData.M32, camData.M33, camData.M34,
            camData.M41, camData.M42, camData.M43, camData.M44
        );
    }

    private void SaveHealth(string fileName)
    {
        var healths = gameControl.Healths;
        var dataList = new List<HealthData>();
        foreach (var health in healths)
        {
            dataList.Add(new HealthData
            {
                Lives = health.Lives
            });
        }
        SaveToFile(dataList, fileName);
    }
    
    private void SaveWeapons(string fileName)
{
    var weaponObj = gameControl.PlayerOffensive.Weapon; 
    

    var allWeaponsSave = new AllWeaponsSave
    {
        CurrentRangedWeaponIndex = weaponObj.CurrentRangedWeaponIndex,
        CurrentMeleeWeaponIndex  = weaponObj.CurrentMeleeWeaponIndex,
        Weapons                  = new List<WeaponSaveData>()
    };

    // 1) RangedWeapons
    foreach (var w in weaponObj.RangedWeapons)
    {

        string textureKey = GetTextureKeyFromName(w.Name);

        var data = new WeaponSaveData
        {
            Name         = w.Name,
            TextureKey   = textureKey,
            PositionX    = w.Position.X,
            PositionY    = w.Position.Y,
            OffsetX      = w.Offset.X,
            OffsetY      = w.Offset.Y,
            Damage       = w.Damage,
            FireRate     = w.FireRate,
            Speed        = w.Speed,
            Lifespan     = w.Lifespan,
            IsActive     = w.IsActive,
            ShotCooldown = w.ShotCooldown,
            HitBoxSizeX  = w.HitBoxSize.X,
            HitBoxSizeY  = w.HitBoxSize.Y,
            Scale        = w.Scale,
            IsRanged     = true
        };
        allWeaponsSave.Weapons.Add(data);
    }

    // 2) MeleeWeapons
    foreach (var w in weaponObj.MeleeWeapons)
    {
        string textureKey = GetTextureKeyFromName(w.Name);

        var data = new WeaponSaveData
        {
            Name         = w.Name,
            TextureKey   = textureKey,
            PositionX    = w.Position.X,
            PositionY    = w.Position.Y,
            OffsetX      = w.Offset.X,
            OffsetY      = w.Offset.Y,
            Damage       = w.Damage,
            FireRate     = w.FireRate,
            Speed        = w.Speed,
            Lifespan     = w.Lifespan,
            IsActive     = w.IsActive,
            ShotCooldown = w.ShotCooldown,
            HitBoxSizeX  = w.HitBoxSize.X,
            HitBoxSizeY  = w.HitBoxSize.Y,
            Scale        = w.Scale,
            IsRanged     = false
        };
        allWeaponsSave.Weapons.Add(data);
    }
    
    SaveToFile(new List<AllWeaponsSave> { allWeaponsSave }, fileName);
}

    private string GetTextureKeyFromName(string weaponName)
    {
        return weaponName switch
        {
            "Greatbow"   => "iconGreatbow",
            "Crossbow"   => "iconCrossbow",
            "Saber"      => "iconSaber",
            "Longsword"  => "iconLongsword",
            "Spear"      => "iconSpear",
            _            => "iconSaber" // default
        };
    }

    private void LoadWeapons(string fileName)
{
    var list = LoadFromFile<AllWeaponsSave>(fileName);
    if (list == null || list.Count == 0)
        return;

    
    var loaded = list[0];

    var weaponObj = gameControl.PlayerOffensive.Weapon;
   

    
    weaponObj.RangedWeapons.Clear();
    weaponObj.MeleeWeapons.Clear();

    
    weaponObj.CurrentRangedWeaponIndex = loaded.CurrentRangedWeaponIndex;
    weaponObj.CurrentMeleeWeaponIndex  = loaded.CurrentMeleeWeaponIndex;

    
    foreach (var wsd in loaded.Weapons)
    {
       
        var texture = gameControl.AssetManager.GetTexture(wsd.TextureKey);

        var newWeaponData = new WeaponData
        {
            Name      = wsd.Name,
            Texture   = texture,
            Position  = new Vector2(wsd.PositionX, wsd.PositionY),
            Offset    = new Vector2(wsd.OffsetX, wsd.OffsetY),
            Damage    = wsd.Damage,
            FireRate  = wsd.FireRate,
            Speed     = wsd.Speed,
            Lifespan  = wsd.Lifespan,
            IsActive  = wsd.IsActive,
            ShotCooldown = wsd.ShotCooldown,
            HitBoxSize   = new Vector2(wsd.HitBoxSizeX, wsd.HitBoxSizeY),
            Scale        = wsd.Scale
        };

        if (wsd.IsRanged)
            weaponObj.RangedWeapons.Add(newWeaponData);
        else
            weaponObj.MeleeWeapons.Add(newWeaponData);
    }
    
    if (weaponObj.CurrentRangedWeaponIndex < 0 
        || weaponObj.CurrentRangedWeaponIndex >= weaponObj.RangedWeapons.Count)
    {
        weaponObj.CurrentRangedWeaponIndex = 0;
    }
    if (weaponObj.CurrentMeleeWeaponIndex < 0 
        || weaponObj.CurrentMeleeWeaponIndex >= weaponObj.MeleeWeapons.Count)
    {
        weaponObj.CurrentMeleeWeaponIndex = 0;
    }
}
    
    private void SaveShield(string fileName)
    {
        var shields = new List<Shield> { gameControl.PlayerDefensive.Shield };
        var dataList = new List<ShieldData>();
        foreach (var shield in shields)
        {
            dataList.Add(new ShieldData
            {
                IsActive = shield.IsActive,
                TimeSinceLastActivation = shield.TimeSinceLastActivation,
                TimeActive = shield.TimeActive,
                Lifespan = shield.Lifespan,
                CoolDown = shield.CoolDown,
                CentreX = shield.Centre.X,
                CentreY = shield.Centre.Y
            });
        }
        SaveToFile(dataList, fileName);
    }

    private void SaveHealthBar(string fileName)
    {
        var healthBars = gameControl.HealthBars.HealthbarsList;
        var dataList = new List<HealthBarData>();

        foreach (var healthBar in healthBars)
        {
            int rawHealthValue = (int)(healthBar.CurrentHealth * 100);

            var item = new HealthBarData
            {
                HealthBarPositionX = healthBar.Position.X,
                HealthBarPositionY = healthBar.Position.Y,
                HealthPoints = rawHealthValue
            };
            dataList.Add(item);
        }
        SaveToFile(dataList, fileName);
    }

    private void SavePotionsEndBossMap(string fileName)
    {
        var menuCtrl = gameControl.MenuControl;
        if (menuCtrl == null) return;
        var endBossScreen =
            gameControl.MenuControl.ScreenManager.ScreenStack.ElementAt(gameControl.MenuControl.ScreenManager
                .ScreenStack.Count - 2) as EndBossScreen;
        if (endBossScreen == null) return;
        var endBossHandle = endBossScreen.EndBossHandle;
        if (endBossHandle == null) return;
        var potionsEndBossMap = endBossHandle.Potion;
        if (potionsEndBossMap == null) return;
        
        var data = new PotionEndBossMapSaveData()
        {
            NumberOfPotions = potionsEndBossMap.NumberOfPotions,
            PositionX = new List<float>(),
            PositionY = new List<float>(),
            
            PotionRectanglesX = new List<float>(),
            PotionRectanglesY = new List<float>()
        };
        
        foreach (var pos in potionsEndBossMap.PotionPositions)
        {
            data.PositionX.Add(pos.X);
            data.PositionY.Add(pos.Y);
        }
        
        foreach (var rect in MapManager.PotionRectangles)
        {
            data.PotionRectanglesX.Add(rect.X);
            data.PotionRectanglesY.Add(rect.Y);
        }
        
        var dataList = new List<PotionEndBossMapSaveData> { data };
        SaveToFile(dataList, fileName);
    }
    
    private void LoadPotionsEndBossMap(string fileName)
    {
        var menuCtrl = gameControl.MenuControl;
        if (menuCtrl == null)
        {
            return;
        }
        var endBossScreen =
            gameControl.MenuControl.ScreenManager.ScreenStack.ElementAt(gameControl.MenuControl.ScreenManager
                .ScreenStack.Count - 2) as EndBossScreen;
        if (endBossScreen == null)
        {
            return;
        }
        var endBossHandle = endBossScreen.EndBossHandle;
        if (endBossHandle == null)
        {
            return;
        }
        var potionsEndBossMap = endBossHandle.Potion;
        if (potionsEndBossMap == null)
        {
            return;
        }
        var dataList = LoadFromFile<PotionEndBossMapSaveData>(fileName);

        if (dataList == null || dataList.Count == 0)
        {
            return;
        }
            
        
        var data = dataList[0];
       
        potionsEndBossMap.NumberOfPotions = data.NumberOfPotions;
        
        potionsEndBossMap.PotionPositions.Clear();

        for (int i = 0; i < data.PositionX.Count; i++)
        {
            var posX = data.PositionX[i];
            var posY = data.PositionY[i];
            potionsEndBossMap.PotionPositions.Add(new Vector2(posX, posY));
        }
        
        MapManager.PotionRectangles.Clear();
        
        for (int i = 0; i < data.PotionRectanglesX.Count; i++)
        {
            var rectX = (int)data.PotionRectanglesX[i];
            var rectY = (int)data.PotionRectanglesY[i];
            var rect = new Rectangle(rectX, rectY, 16, 16);
            MapManager.PotionRectangles.Add(rect);
        }
    }

    private void SavePotions(string fileName)
    {
        var potions = gameControl.Potion;
        if (potions == null)
            return;

        var data = new PotionSaveData
        {
            NumberOfPotions = potions.NumberOfPotions,
            PositionX = new List<float>(),
            PositionY = new List<float>(),
            
            PotionRectanglesX = new List<float>(),
            PotionRectanglesY = new List<float>()
        };
        
        foreach (var pos in potions.PotionPositions)
        {
            data.PositionX.Add(pos.X);
            data.PositionY.Add(pos.Y);
        }
        
        foreach (var rect in MapManager.PotionRectangles)
        {
            data.PotionRectanglesX.Add(rect.X);
            data.PotionRectanglesY.Add(rect.Y);
        }
        
        var dataList = new List<PotionSaveData> { data };
        SaveToFile(dataList, fileName);
    }
    private void LoadPotions(string fileName)
    {
        var dataList = LoadFromFile<PotionSaveData>(fileName);
        
        if (dataList == null || dataList.Count == 0)
            return;

       
        var data = dataList[0];

        var potions = gameControl.Potion;
        if (potions == null)
            return;

       
        potions.NumberOfPotions = data.NumberOfPotions;
        
        potions.PotionPositions.Clear();

        for (int i = 0; i < data.PositionX.Count; i++)
        {
            var posX = data.PositionX[i];
            var posY = data.PositionY[i];
            potions.PotionPositions.Add(new Vector2(posX, posY));
        }
        
        MapManager.PotionRectangles.Clear();
        
        for (int i = 0; i < data.PotionRectanglesX.Count; i++)
        {
            var rectX = (int)data.PotionRectanglesX[i];
            var rectY = (int)data.PotionRectanglesY[i];
            var rect = new Rectangle(rectX, rectY, 16, 16);
            MapManager.PotionRectangles.Add(rect);
        }
    }

    /// <summary>
    /// Save Players to a file.
    /// </summary>
    private void SavePlayers(string fileName)
    {
        var players = gameControl.Players;
        var dataList = new List<PlayerData>();
    
        foreach (var p in players)
        {
            dataList.Add(new PlayerData
            {
                GameObjectHealthpoints = p.GameObjectHealthPoints,
                PositionX = p.Position.X,
                PositionY = p.Position.Y,
                IsAlive = p.IsAlive,
                JumpVelocity = p.JumpVelocity,
                IsOnGround = p.IsOnGround,
                TimeSinceLastShot = p.TimeSinceLastShot,
                TimeSinceLastHit = p.TimeSinceLastHit,
                DirectionX = p.Direction.X,
                DirectionY = p.Direction.Y,
                Name = p.Name,
                Flipped = p.Flipped,
                HasAttackSystem = p.HasAttackSystem,
                IsInvisible = p.IsInvisible,
                InvisibilityTimeLeft = p.InvisibilityTimeLeft,
                IsDebugMode = p.IsDebugMode,
                GodMode = p.GodMode,
                TechDemo = p.TechDemo,
                InvisibilityDuration = p.InvisibilityDuration,
                InvisibilityCooldown = p.InvisibilityCooldown,
                TimeSinceLastComment = p.TimeSinceLastComment,
                IsInCheckPoint = p.IsInCheckPoint
            });
        }
    
        SaveToFile(dataList, fileName);
    }
    
    public void SaveLines(string fileName)
    {
        
        var menuCtrl = gameControl.MenuControl;
        if (menuCtrl == null)
            return;

        
        var screenStack = menuCtrl.ScreenManager.ScreenStack;
        if (screenStack.Count < 2)
            return;

        var endBossScreen = screenStack.ElementAt(screenStack.Count - 2) as EndBossScreen;
        if (endBossScreen == null)
            return;

        var endBossHandle = endBossScreen.EndBossHandle;
        if (endBossHandle == null)
            return;

        
        var lineDataList = new List<LineSaveData>();
        foreach (var line in endBossHandle.Lines)
        {
            lineDataList.Add(line.GetSaveData());
        }

        SaveToFile(lineDataList, fileName);
    }

    public void LoadLines(string fileName)
    {
        var menuCtrl = gameControl.MenuControl;
        if (menuCtrl == null)
            return;

        var screenStack = menuCtrl.ScreenManager.ScreenStack;
        if (screenStack.Count < 2)
            return;

        var endBossScreen = screenStack.ElementAt(screenStack.Count - 2) as EndBossScreen;
        if (endBossScreen == null)
            return;

        var endBossHandle = endBossScreen.EndBossHandle;
        if (endBossHandle == null)
            return;

        var lineDataList = LoadFromFile<LineSaveData>(fileName);
        if (lineDataList == null)
            return;
        
        endBossHandle.Lines.Clear();
        
        foreach (var data in lineDataList)
        {
            var line = new Line(data, gameControl.AssetManager);
            endBossHandle.Lines.Add(line);
        }
    }


    private void SaveRangedEnemyProjectile(string fileName)
    {
        var dataList = new List<PlayerProjectileData>();
        foreach (var enemy in gameControl.EnemyManager.RangedEnemies)
        {
            var projectiles = enemy.AttackSystem.Projectiles;
            foreach (var p in projectiles)
            {
                dataList.Add(new PlayerProjectileData
                {
                    MobPositionX = p.Position.X,
                    MobPositionY = p.Position.Y,
                    TargetPositionX = p.TargetPosition.X,
                    TargetPositionY = p.TargetPosition.Y,
                    Speed = p.Speed,
                    Damage = p.Damage,
                    LifeSpan = p.Lifespan,
                    OwnerId = p.OwnerId,
                    DirectionX = p.Direction.X,
                    DirectionY = p.Direction.Y,
                });
            }
        }
        SaveToFile(dataList, fileName);
    }

    private void SavePlayerProjectile(string fileName)
    {
        var projectiles = gameControl.PlayerOffensive.AttackSystem.Projectiles;
        var dataList = new List<PlayerProjectileData>();
        foreach (var p in projectiles)
        {
            dataList.Add(new PlayerProjectileData
            {
                MobPositionX = p.Position.X,
                MobPositionY = p.Position.Y,
                TargetPositionX = p.TargetPosition.X,
                TargetPositionY = p.TargetPosition.Y,
                Speed = p.Speed,
                Damage = p.Damage,
                LifeSpan = p.Lifespan,
                OwnerId = p.OwnerId,
                DirectionX = p.Direction.X,
                DirectionY = p.Direction.Y,
                
            });
        }
        SaveToFile(dataList, fileName);
    }

    private void LoadProjectile(string fileName)
    {
        var dataList = LoadFromFile<PlayerProjectileData>(fileName);
        if (dataList == null) return;

        const int sizeX = 20;
        const int sizeY = 20;

        foreach (var data in dataList)
        {

            var mobPosition = new Vector2(data.MobPositionX, data.MobPositionY);
            var targetPosition = new Vector2(data.TargetPositionX, data.TargetPositionY);
            var size = new Vector2(sizeX, sizeY);

            var newProjectile = new Projectile(
                mobPosition,
                targetPosition,
                data.Speed,
                data.Damage,
                (int)data.LifeSpan,
                gameControl.CollisionDetection,
                gameControl.AssetManager,
                size,
                ref gameControl.Statistics,
                ref gameControl.Achievements,
                data.OwnerId,
                gameControl.PlayerDefensive,
                gameControl.MapManager
            );
            gameControl.PlayerOffensive.AttackSystem.AddProjectile(newProjectile);
            newProjectile.Direction = new Vector2(data.DirectionX, data.DirectionY);
        }
    }

    private void LoadHealth(string fileName)
    {
        List<Health> healths = gameControl.Healths;
        var dataList = LoadFromFile<HealthData>(fileName);
        if (dataList == null || dataList.Count != healths.Count) return;

        for (int i = 0; i < healths.Count; i++)
        {
            healths[i].Lives = dataList[i].Lives;
        }
    }

    private void LoadHealthBar(string fileName)
    {
        var healthbars = gameControl.HealthBars.HealthbarsList;
        var dataList = LoadFromFile<HealthBarData>(fileName);
        if (dataList == null || dataList.Count != healthbars.Count)
            return;

        for (var i = 0; i < healthbars.Count; i++)
        {
            healthbars[i].Position = new Vector2(
                dataList[i].HealthBarPositionX,
                dataList[i].HealthBarPositionY
            );

            healthbars[i].Update(dataList[i].HealthPoints);
        }
    }
    public void SaveEndBossProjectiles(string fileName)
    {
        var menuCtrl = gameControl.MenuControl;
        if (menuCtrl == null)
            return;
        
        var screenStack = menuCtrl.ScreenManager.ScreenStack;
        if (screenStack.Count < 2)
            return;
    
        var endBossScreen = screenStack.ElementAt(screenStack.Count - 2) as EndBossScreen;
        if (endBossScreen == null)
            return;
    
        var endBossHandle = endBossScreen.EndBossHandle;
        if (endBossHandle == null)
            return;
    
        var endBoss = endBossHandle.EndBoss;
        if (endBoss == null)
            return;
    
        var projectileDataList = new List<PlayerProjectileData>();
    
        foreach (var p in endBoss.AttackSystem.Projectiles)
        {
            projectileDataList.Add(new PlayerProjectileData
            {
                MobPositionX = p.Position.X,
                MobPositionY = p.Position.Y,
                TargetPositionX = p.TargetPosition.X,
                TargetPositionY = p.TargetPosition.Y,
                Speed = p.Speed,
                Damage = p.Damage,
                LifeSpan = p.Lifespan,
                OwnerId = p.OwnerId,
                DirectionX = p.Direction.X,
                DirectionY = p.Direction.Y,
            });
        }
    
        SaveToFile(projectileDataList, fileName);
    }
    public void LoadEndBossProjectiles(string fileName)
    {
        var projectileDataList = LoadFromFile<PlayerProjectileData>(fileName);
        if (projectileDataList == null)
            return;
    
        var menuCtrl = gameControl.MenuControl;
        if (menuCtrl == null)
            return;
    
        var screenStack = menuCtrl.ScreenManager.ScreenStack;
        if (screenStack.Count < 2)
            return;
    
        var endBossScreen = screenStack.ElementAt(screenStack.Count - 2) as EndBossScreen;
        if (endBossScreen == null)
            return;
    
        var endBossHandle = endBossScreen.EndBossHandle;
        if (endBossHandle == null)
            return;
    
        var endBoss = endBossHandle.EndBoss;
        if (endBoss == null)
            return;
    
        
        endBoss.AttackSystem.Projectiles.Clear();
    
        
        foreach (var data in projectileDataList)
        {
            
            Vector2 size = new Vector2(20, 20);
            var newProjectile = new Projectile(
                new Vector2(data.MobPositionX, data.MobPositionY),
                new Vector2(data.TargetPositionX, data.TargetPositionY),
                data.Speed,
                data.Damage,
                (int)data.LifeSpan,
                gameControl.CollisionDetection,
                gameControl.AssetManager,
                size,
                ref gameControl.Statistics,
                ref gameControl.Achievements,
                data.OwnerId,
                gameControl.PlayerDefensive,
                gameControl.MapManager
            );
            endBoss.AttackSystem.AddProjectile(newProjectile);
        }
    }
    public void SavePlayerProjectilesOnEndBossMap(string fileName)
    {
        var dataList = new List<PlayerProjectileData>();
        
        foreach (var player in gameControl.Players)
        {
            
            foreach (var proj in player.AttackSystem.Projectiles)
            {
                dataList.Add(new PlayerProjectileData
                {
                    MobPositionX = proj.Position.X,
                    MobPositionY = proj.Position.Y,
                    TargetPositionX = proj.TargetPosition.X,
                    TargetPositionY = proj.TargetPosition.Y,
                    Speed = proj.Speed,
                    Damage = proj.Damage,
                    LifeSpan = proj.Lifespan,
                    OwnerId = proj.OwnerId,
                    DirectionX = proj.Direction.X,
                    DirectionY = proj.Direction.Y,
                });
            }
        }
        
        SaveToFile(dataList, fileName);
    }
    public void LoadPlayerProjectilesOnEndBossMap(string fileName)
    {
        var dataList = LoadFromFile<PlayerProjectileData>(fileName);
        if (dataList == null)
            return;
        
        foreach (var player in gameControl.Players)
        {
            player.AttackSystem.Projectiles.Clear();
        }
        
        const int sizeX = 20;
        const int sizeY = 20;
        var projectileSize = new Vector2(sizeX, sizeY);
        
        foreach (var data in dataList)
        {
            
            var newProj = new Projectile(
                new Vector2(data.MobPositionX, data.MobPositionY),
                new Vector2(data.TargetPositionX, data.TargetPositionY),
                data.Speed,
                data.Damage,
                (int)data.LifeSpan,
                gameControl.CollisionDetection,
                gameControl.AssetManager,
                projectileSize,
                ref gameControl.Statistics,
                ref gameControl.Achievements,
                data.OwnerId,
                gameControl.PlayerDefensive,
                gameControl.MapManager
            );
            
            bool added = false;
            foreach (var player in gameControl.Players)
            {
                if (player.Name == data.OwnerId)
                {
                    player.AttackSystem.AddProjectile(newProj);
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                gameControl.PlayerOffensive.AttackSystem.AddProjectile(newProj);
            }
        }
    }



    /// <summary>
    /// Timer saving process
    /// </summary>
    private void SaveTimer(string fileName)
    {
        var timer = gameControl.MenuControl.Timer;
        var dataList = new List<TimerData>
        {
            new()
            {
                ElapsedTime = timer.ElapsedTime
            }
        };
        SaveToFile(dataList, fileName);
    }

    /// <summary>
    /// Timer loading process
    /// </summary>
    private void LoadTimer(string fileName)
    {
        var dataList = LoadFromFile<TimerData>(fileName);
        if (dataList == null || dataList.Count == 0)
            return;

        gameControl.MenuControl.Timer.ElapsedTime = dataList[0].ElapsedTime;
    }

    /// <summary>
    /// Load Players from a file.
    /// </summary>
    private void LoadPlayers(string fileName)
    {
        var players = gameControl.Players;
        var dataList = LoadFromFile<PlayerData>(fileName);
        if (dataList == null || dataList.Count != players.Count) return;

        for (var i = 0; i < players.Count; i++)
        {
            players[i].GameObjectHealthPoints = dataList[i].GameObjectHealthpoints;
            players[i].Position = new Vector2(dataList[i].PositionX, dataList[i].PositionY);
            players[i].IsAlive = dataList[i].IsAlive;
            players[i].JumpVelocity = dataList[i].JumpVelocity;
            players[i].IsOnGround = dataList[i].IsOnGround;
            players[i].TimeSinceLastShot = dataList[i].TimeSinceLastShot;
            players[i].TimeSinceLastHit = dataList[i].TimeSinceLastHit;
            players[i].Direction = new Vector2(dataList[i].DirectionX, dataList[i].DirectionY);
            players[i].Name = dataList[i].Name;
            players[i].Flipped = dataList[i].Flipped;
            players[i].HasAttackSystem = dataList[i].HasAttackSystem;
            players[i].InvisibilityTimeLeft = dataList[i].InvisibilityTimeLeft;
            players[i].IsInvisible = dataList[i].IsInvisible;
            players[i].IsDebugMode = dataList[i].IsDebugMode;
            players[i].GodMode = dataList[i].GodMode;
            players[i].OnEndBossMap = dataList[i].OnEndBossMap;
            players[i].TechDemo = dataList[i].TechDemo;
            players[i].InvisibilityDuration = dataList[i].InvisibilityDuration;
            players[i].InvisibilityCooldown = dataList[i].InvisibilityCooldown;
            players[i].TimeSinceLastComment = dataList[i].TimeSinceLastComment;
            players[i].IsInCheckPoint = dataList[i].IsInCheckPoint;
        }
    }


    private void LoadShield(string fileName)
    {
        var shields = new List<Shield> { gameControl.PlayerDefensive.Shield };
        var dataList = LoadFromFile<ShieldData>(fileName);
        if (dataList == null || dataList.Count != shields.Count) return;

        for (var i = 0; i < shields.Count; i++)
        {
            shields[i].Centre = new Vector2(dataList[i].CentreX, dataList[i].CentreY);
            shields[i].Lifespan = dataList[i].Lifespan;
            shields[i].TimeActive = dataList[i].TimeActive;
            shields[i].TimeSinceLastActivation = dataList[i].TimeSinceLastActivation;
            shields[i].IsActive = dataList[i].IsActive;
            shields[i].CoolDown = dataList[i].CoolDown;
        }
    }
    
    private void SaveMeleeEnemies(string fileName)
    {
        var meleeEnemies = gameControl.EnemyManager.MeleeEnemies;
        var dataList = new List<MeleeEnemyData>();

        foreach (var m in meleeEnemies)
        {
            
            dataList.Add(new MeleeEnemyData
            {
                PositionX = m.Position.X,
                PositionY = m.Position.Y,
                IsAlive = m.IsAlive,
                Speed = m.Speed,
                MoveTimeX = m.MoveTimeX,
                MoveTimeY = m.MoveTimeY,
                MaxMoveTime = m.MaxMoveTime,
                MoveDirectionX = m.MoveDirectionX,
                MoveDirectionY = m.MoveDirectionY,
                IsMovingInXDirection = m.IsMovingInXDirection,
                IsOnGround = false,
                Flipped = m.Flipped,
                IsExploding = m.IsExploding,
                ExplosionAnimationTimer = m.ExplosionAnimationTimer,
                AnimationWasFlippedBecauseOfPlayer = m.AnimationWasFlippedBecauseOfPlayer,
                ProjectileSpawned = m.ProjectileSpawned,
                ExplosionDelay = m.ExplosionDelay,
                ExplosionShortFactor = m.ExplosionShortFactor,
                MapIndex = m.MapIndex,
                Damage = m.Damage
            });
        }

        SaveToFile(dataList, fileName);
    }


    private void LoadMeleeEnemies(string fileName)
    {
        var dataList = LoadFromFile<MeleeEnemyData>(fileName);
        if (dataList == null) return;
        
        
        gameControl.EnemyManager.ClearMeleeEnemies();

        foreach (var data in dataList)
        {
            if (!data.IsAlive)
            {
                continue;
            }
            
            var newMeleeEnemy = new MeleeEnemy(
                new Vector2(data.PositionX, data.PositionY),
                data.Speed,
                new Animation.AnimationPlayer(),
                gameControl.CollisionDetection,
                gameControl.EnemyManager.MeleeEnemyAnimations,
                ref gameControl.Statistics,
                ref gameControl.Achievements,
                gameControl.MapManager,
                gameControl.PlayerDefensive,
                gameControl.PlayerOffensive,
                gameControl.GraphicsDevice,
                gameControl.AssetManager
            )
            {
                Position = new Vector2(data.PositionX, data.PositionY),
                IsAlive = data.IsAlive,
                Speed = data.Speed,
                MoveTimeX = data.MoveTimeX,
                MoveTimeY = data.MoveTimeY,
                MaxMoveTime = data.MaxMoveTime,
                MoveDirectionX = data.MoveDirectionX,
                MoveDirectionY = data.MoveDirectionY,
                IsMovingInXDirection = data.IsMovingInXDirection,
                Flipped = data.Flipped,
                IsExploding = data.IsExploding,
                ExplosionAnimationTimer = data.ExplosionAnimationTimer,
                AnimationWasFlippedBecauseOfPlayer = data.AnimationWasFlippedBecauseOfPlayer,
                ProjectileSpawned = data.ProjectileSpawned,
                ExplosionDelay = data.ExplosionDelay,
                ExplosionShortFactor = data.ExplosionShortFactor,
                MapIndex = data.MapIndex,
                Damage = data.Damage
            };

            gameControl.EnemyManager.MeleeEnemies.Add(newMeleeEnemy);
        }
    }
    
    private void SaveRangedEnemies(string fileName)
    {
        var rangedEnemies = gameControl.EnemyManager.RangedEnemies;
        var dataList = new List<RangedEnemyData>();
    
        foreach (var r in rangedEnemies)
        {
            dataList.Add(new RangedEnemyData
            {
                PositionX = r.Position.X,
                PositionY = r.Position.Y,
                IsAlive = r.IsAlive,
                Speed = r.Speed,
                MoveTimeX = r.MoveTimeX,
                MoveTimeY = r.MoveTimeY,
                MaxMoveTime = r.MaxMoveTime,
                TimeSinceLastShot = r.TimeSinceLastShot,
                ShotCooldown = r.ShotCooldown,
                MoveDirectionX = r.MoveDirectionX,
                MoveDirectionY = r.MoveDirectionY,
                IsMovingInXDirection = r.IsMovingInXDirection,
                Health = r.HealthPoints,
                HasAttackSystem = r.HasAttackSystem,
                TechDemo = r.TechDemo,
                PrevPositionX = r.PrevPosition.X,
                PrevPositionY = r.PrevPosition.Y,
                StartPositionX = r.StartPosition.X,
                StartPositionY = r.StartPosition.Y,
                TimeSinceLastUpdate = r.TimeSinceLastUpdate,
                MapIndex = r.MapIndex
            });
        }
        SaveToFile(dataList, fileName);
    }
    
            private void SaveEndBoss(string fileName)
        {
            ClearFile(fileName);
            var menuCtrl = gameControl.MenuControl;
            if (menuCtrl == null) return; 
            
            var endBossScreen = gameControl.MenuControl.ScreenManager.ScreenStack.ElementAt(gameControl.MenuControl.ScreenManager.ScreenStack.Count - 2) as EndBossScreen;
            if (endBossScreen == null) return;
            
            var endBossHandle = endBossScreen.EndBossHandle;
            if (endBossHandle == null) return;
            
            var endBoss = endBossHandle.EndBoss;
            if (endBoss == null) return;

            
            var dataList = new List<EndBossData>();
            var d = new EndBossData
            {
                PositionX = endBoss.Position.X,
                PositionY = endBoss.Position.Y,
                TimeSinceLastAttack = endBoss.TimeSinceLastAttack,
                TimeSinceLastImmune = endBoss.TimeSinceLastImmune,
                TimeSinceLastHealing = endBoss.TimeSinceLastHealing,
                TimeSinceLastHit = endBoss.TimeSinceLastHit,
                EbHealingDuration = endBoss.EbHealingDuration,
                EbImmuneDuration = endBoss.EbImmuneDuration,
                TimeSinceLastGlobalSkill = endBoss.TimeSinceLastGlobalSkill,
                TimeSinceDamageBuffActivation = endBoss.TimeSinceDamageBuffActivation,
                TimeSinceDeath = endBoss.TimeSinceDeath,
                HealingValue = endBoss.HealingValue,
                IsHealing = endBoss.IsHealing,
                IsImmune = endBoss.IsImmune,
                GlobalSkillIsActive = endBoss.GlobalSkillIsActive,
                DamageBuffIsActive = endBoss.DamageBuffIsActive,
                DamageBuffAnimationRunning = endBoss.DamageBuffAnimationRunning,
                RangeAttackActive = endBoss.RangeAttackActive,
                MeleeAttackActive = endBoss.MeleeAttackActive,
                FlipAnimation = endBoss.FlipAnimation,
                EbIsAlive = endBoss.EbIsAlive,
                DeathAnimationRunning = endBoss.DeathAnimationRunning,
                GameObjectHealthPoints = endBoss.GameObjectHealthPoints,
                TimeSinceLastUpdate = endBoss.TimeSinceLastUpdate,
            };
            dataList.Add(d);

            
            SaveToFile(dataList, fileName);
        }

        private void LoadEndBoss(string fileName)
        {
            
            var dataList = LoadFromFile<EndBossData>(fileName);
            if (dataList == null || dataList.Count == 0)
                return;

            gameControl.MenuControl.Potion.ResetPotion();
            gameControl.MenuControl.StartEndBossMap(false);
            
            
            var menuCtrl = gameControl.MenuControl;
            if (menuCtrl == null) return; 
            
            var endBossScreen = gameControl.MenuControl.ScreenManager.ScreenStack.ElementAt(gameControl.MenuControl.ScreenManager.ScreenStack.Count - 2) as EndBossScreen;
            if (endBossScreen == null) return;
            
            var endBossHandle = endBossScreen.EndBossHandle;
            if (endBossHandle == null) return;
            
            var endBoss = endBossHandle.EndBoss;
            if (endBoss == null) return;

            
            

            
            var d = dataList[0];
            endBoss.Position = new Vector2(d.PositionX, d.PositionY);
            endBoss.TimeSinceLastAttack = d.TimeSinceLastAttack;
            endBoss.TimeSinceLastImmune = d.TimeSinceLastImmune;
            endBoss.TimeSinceLastHealing = d.TimeSinceLastHealing;
            endBoss.TimeSinceLastHit = d.TimeSinceLastHit;
            endBoss.EbHealingDuration = d.EbHealingDuration;
            endBoss.EbImmuneDuration = d.EbImmuneDuration;
            endBoss.TimeSinceLastGlobalSkill = d.TimeSinceLastGlobalSkill;
            endBoss.TimeSinceDamageBuffActivation = d.TimeSinceDamageBuffActivation;
            endBoss.TimeSinceDeath = d.TimeSinceDeath;
            endBoss.HealingValue = d.HealingValue;
            endBoss.IsHealing = d.IsHealing;
            endBoss.IsImmune = d.IsImmune;
            endBoss.GlobalSkillIsActive = d.GlobalSkillIsActive;
            endBoss.DamageBuffIsActive = d.DamageBuffIsActive;
            endBoss.DamageBuffAnimationRunning = d.DamageBuffAnimationRunning;
            endBoss.RangeAttackActive = d.RangeAttackActive;
            endBoss.MeleeAttackActive = d.MeleeAttackActive;
            endBoss.FlipAnimation = d.FlipAnimation;
            endBoss.EbIsAlive = d.EbIsAlive;
            endBoss.DeathAnimationRunning = d.DeathAnimationRunning;
            endBoss.GameObjectHealthPoints = d.GameObjectHealthPoints;
            endBoss.TimeSinceLastUpdate = d.TimeSinceLastUpdate;
        }



    private void LoadRangedEnemies(string fileName)
    {
        var dataList = LoadFromFile<RangedEnemyData>(fileName);
        if (dataList == null) return;

        gameControl.EnemyManager.ClearRangedEnemies();

        foreach (var data in dataList)
        {
          
            var newRangedEnemy = new RangedEnemy(
                gameControl.GraphicsDevice,
                new Vector2(data.PositionX, data.PositionY),
                data.Speed,
                gameControl.AssetManager,
                new Animation.AnimationPlayer(),
                true, // hasAttackSystem
                gameControl.CollisionDetection,
                gameControl.EnemyManager.RangedEnemyAnimations,
                ref gameControl.Statistics,
                ref gameControl.Achievements,
                gameControl.PlayerOffensive,
                gameControl.PlayerDefensive,
                gameControl.EnemyManager.Grid,
                gameControl.MapManager,
                new Vector2(15, 26)
            )
            {
                MoveTimeX = data.MoveTimeX,
                MoveTimeY = data.MoveTimeY,
                MaxMoveTime = data.MaxMoveTime,
                TimeSinceLastShot = data.TimeSinceLastShot,
                ShotCooldown = data.ShotCooldown,
                MoveDirectionX = data.MoveDirectionX,
                MoveDirectionY = data.MoveDirectionY,
                IsMovingInXDirection = data.IsMovingInXDirection,
                IsAlive = data.IsAlive,
                HealthPoints = data.Health,
                HasAttackSystem = data.HasAttackSystem,
                TechDemo = data.TechDemo,
                PrevPosition = new Vector2(data.PrevPositionX, data.PrevPositionY),
                StartPosition = new Vector2(data.StartPositionX, data.StartPositionY),
                TimeSinceLastUpdate = data.TimeSinceLastUpdate,
                MapIndex = data.MapIndex
            };

            gameControl.EnemyManager.RangedEnemies.Add(newRangedEnemy);
            foreach (var rangedEnemy in gameControl.EnemyManager.RangedEnemies)
            {
                if (true)
                {
                    rangedEnemy.SearchPath();
                }
            }
        }
    }
}
