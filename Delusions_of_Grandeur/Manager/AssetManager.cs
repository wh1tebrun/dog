# region File Description
// AssetManager.cs
// Manage the Assets for the game.
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Delusions_of_Grandeur.Manager;

public class AssetManager
{
    private readonly Dictionary<string, Texture2D> _textures = new();
    private readonly Dictionary<string, SpriteFont> _fonts = new();
    private readonly Dictionary<string, Effect> _shaders = new();
    private readonly Dictionary<string, SoundEffect> _soundEffects = new();
    private readonly Dictionary<string, SoundEffectInstance> _soundInstances = new();
    private readonly Dictionary<string, Song> _backgroundMusic = new();
    private float _soundVolume = 0.5f;
    public static bool IsSoundActive = true;

    /// <summary>
    /// Retrieve a sound effect based on a given key.
    /// </summary>
    /// <param name="key"> The key that specifies the sound effect. </param>
    /// <returns></returns>
    public SoundEffect GetSound(string key)
    {
        return _soundEffects.GetValueOrDefault(key);
    }

    public SoundEffectInstance GetSoundEffectInstance(string key)
    {
        return _soundInstances.GetValueOrDefault(key);
    }

    public float SoundVolume
    {
        get => _soundVolume;
        set => _soundVolume = MathHelper.Clamp(value, 0, 1);
    }
    
        
    private Random _random = new Random();
    /// <summary>
    /// Play the sound of a given key with a given volume.
    /// </summary>
    /// <param name="key"> The key represents the name of the sound. </param>
    /// <param name="minPitch"> The upper bound for the pitch. </param>
    /// <param name="maxPitch"> The lower bound for the pitch </param>
    public void PlaySound(string key, float minPitch = -0f, float maxPitch = 0f)
    {
	if (!IsSoundActive) return;

	float randomPitch = (float)(_random.NextDouble() * (maxPitch - minPitch) + minPitch);
	_soundInstances.TryGetValue(key, out var soundInstance);

	if (soundInstance == null)
	{
	    soundInstance = _soundEffects[key].CreateInstance();
	    _soundInstances[key] = soundInstance;
	}

	// Ensure the sound isn't already playing and stop it only if necessary
	if (soundInstance.State == SoundState.Playing)
	{
	    soundInstance.Stop();
	}

	soundInstance.Pitch = randomPitch;
	soundInstance.Play();
    }

    // Declare a single SoundEffectInstance for the ranged enemies
    private SoundEffectInstance _rangedEnemySoundInstance;

    public void PlaySoundRangedEnemy(string key, float minPitch = -0f, float maxPitch = 0f)
    {
        if (!IsSoundActive) return;

	if (_soundEffects.ContainsKey(key))
	{
	    var soundEffect = _soundEffects[key];

	    if (_rangedEnemySoundInstance == null)
	    {
		_rangedEnemySoundInstance = soundEffect.CreateInstance();
	    }

	    _rangedEnemySoundInstance.Pitch = (float)(_random.NextDouble() * (maxPitch - minPitch) + minPitch);

	    if (_rangedEnemySoundInstance.State != SoundState.Playing)
	    {
		_rangedEnemySoundInstance.Play();
	    }
	}
    }
    
    // Declare a single SoundEffectInstance for the ranged enemies
    private SoundEffectInstance _rangedEnemyDeathSoundInstance;

    public void PlayDeathSoundRangedEnemy(string key, float minPitch = -0f, float maxPitch = 0f)
    {
        if (!IsSoundActive) return;

        if (_soundEffects.ContainsKey(key))
        {
            var soundEffect = _soundEffects[key];

            if (_rangedEnemyDeathSoundInstance == null)
            {
                _rangedEnemyDeathSoundInstance = soundEffect.CreateInstance();
            }

            _rangedEnemyDeathSoundInstance.Pitch = (float)(_random.NextDouble() * (maxPitch - minPitch) + minPitch);

            if (_rangedEnemyDeathSoundInstance.State != SoundState.Playing)
            {
                _rangedEnemyDeathSoundInstance.Play();
            }
        }
    }


    /// <summary>
    /// Change volume by a given value.
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeSoundVolumeArrows(float volume)
    {
        _soundVolume += volume;
        _soundVolume = MathHelper.Clamp(_soundVolume, 0, 1);
        SoundEffect.MasterVolume = _soundVolume;
    }
    public void ChangeSoundVolumeMouse(float volume)
    {
        SoundEffect.MasterVolume = MathHelper.Clamp(volume, 0, 1);;
        // _soundVolume = MathHelper.Clamp(volume, 0, 1);
    }

    /// <summary>
    /// Retrieve a texture based on given key.
    /// </summary>
    /// <param name="key"> The key that specifies the texture. </param>
    /// <returns></returns>
    public Texture2D GetTexture(string key)
    {
        return _textures.GetValueOrDefault(key);
    }

    /// <summary>
    /// Retrieve a font based on given key.
    /// </summary>
    /// <param name="key"> Specifies a font. </param>
    /// <returns></returns>
    public SpriteFont GetFont(string key)
    {
        return _fonts.GetValueOrDefault(key);
    }

    /// <summary>
    /// Retrieve a music based on a given key.
    /// </summary>
    /// <param name="key"> The key that specifies the music. </param>
    /// <returns></returns>
    public Song GetMusic(string key)
    {
        return _backgroundMusic.GetValueOrDefault(key);
    }

    public Effect GetEffect(string key)
    {
        return _shaders.GetValueOrDefault(key);
    }

    /// <summary>
    /// Load assets (textures or sounds) from a specified directory.
    /// </summary>
    /// <param name="subDirectory"> The subdirectory to load assets from (e.g., "Textures" or "Sounds"). </param>
    /// <param name="loadAction"> The action defining how each asset is loaded and processed. </param>
    private void LoadAssets(string subDirectory, Action<string> loadAction)
    {
        var directoryPath = Path.Combine(AppContext.BaseDirectory, "Content", subDirectory);
        var files = Directory.GetFiles(directoryPath, "*.xnb");
        foreach (var file in files)
        {
            loadAction(file);
        }
    }
    /// <summary>
    /// Load all textures for the game.
    /// </summary>
    public void LoadTextures(ContentManager content, GraphicsDevice graphicsDevice)
    {
        LoadAssets("Textures", file =>
        {
            string textureName = Path.GetFileNameWithoutExtension(file);
            _textures[textureName] = content.Load<Texture2D>(Path.Combine("Textures", textureName));
        });

        Texture2D red = new Texture2D(graphicsDevice, 1, 1);
        red.SetData([Color.Red]);
        Texture2D white = new Texture2D(graphicsDevice, 1, 1);
        white.SetData([Color.White]);
        _textures["red"] = red;
        _textures["white"] = white;
    }

    /// <summary>
    /// Load all fonts for the game.
    /// </summary>
    /// <param name="content"></param>
    public void LoadFonts(ContentManager content)
    {
        LoadAssets("Fonts", file =>
        {
            string fontName = Path.GetFileNameWithoutExtension(file);
            _fonts[fontName] = content.Load<SpriteFont>(Path.Combine("Fonts", fontName));
        });
    }

    /// <summary>
    /// Load all sound effects for the game.
    /// </summary>
    public void LoadSounds(ContentManager content)
    {
        LoadAssets("Sounds", file =>
        {
            string soundName = Path.GetFileNameWithoutExtension(file);
            SoundEffect soundEffect = content.Load<SoundEffect>(Path.Combine("Sounds", soundName));

            _soundEffects[soundName] = soundEffect;
            _soundInstances[soundName] = soundEffect.CreateInstance();
        });
    }

    /// <summary>
    /// Load all background music for the game.
    /// </summary>
    public void LoadMusic(ContentManager content)
    {
        LoadAssets("BackgroundMusic", file =>
        {
            string musicName = Path.GetFileNameWithoutExtension(file);
            _backgroundMusic[musicName] = content.Load<Song>(Path.Combine("BackgroundMusic", musicName));
        });
    }

    public void LoadShaders(ContentManager content)
    {
        LoadAssets("Shaders", file =>
        {
            string shaderName = Path.GetFileNameWithoutExtension(file);
            _shaders[shaderName] = content.Load<Effect>(Path.Combine("Shaders", shaderName));
        });
    }
    /// <summary>
    /// Unload music for the game.
    /// </summary>
    public void UnloadMusic()
    {
        foreach (var music in _backgroundMusic.Values)
        {
            music.Dispose();
        }
    }
    /// <summary>
    /// Unload textures for the game.
    /// </summary>
    public void UnloadTextures()
    {
        foreach (var texture in _textures.Values)
        {
            texture.Dispose();
        }
    }

    /// <summary>
    /// Unload sounds for the game.
    /// </summary>
    public void UnloadSounds()
    {
        foreach (var sound in _soundEffects.Values)
        {
            sound.Dispose();
        }
    }
}
