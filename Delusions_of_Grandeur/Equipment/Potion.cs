#region File Description
// Potion.cs
// Manage the number of potions and draw the texture for the HUD.
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Equipment;

public class Potion
{
    private readonly Texture2D _potionTexture;
    public List<Vector2> PotionPositions { get; set; }

    public Player PlayerOffensive { get; set; }

    public SpriteFont Font { get; set; }
    public int NumberOfPotions { get; set; }

    public Potion(AssetManager assetManager)
    {
        _potionTexture = assetManager.GetTexture("potion_full");
        Font = assetManager.GetFont("MainMenu");
        PotionPositions = new List<Vector2>();
        // _potionRectangles = new List<Rectangle>();
        InitializePotionPositions();
    }

    public void InitializePotionPositions()
    {
        foreach (var rect in MapManager.PotionRectangles)
        {
            PotionPositions.Add(new Vector2(rect.X, rect.Y));
        }
    }

    public void SetPlayers(Player defensivePlayer, Player offensivePlayer)
    {
        PlayerOffensive = offensivePlayer;
    }

    public void AddPotion()
    {
        NumberOfPotions += 1;
    }

    public void SubtractPotion()
    {
        if (NumberOfPotions > 0)
        {
            NumberOfPotions -= 1;
        }
    }

    public void ResetPotion()
    {
        NumberOfPotions = 0;
        PotionPositions.Clear();
        MapManager.PotionRectangles.Clear();
        InitializePotionPositions();
    }

    public void ResetForEndBossMap()
    {
        PotionPositions.Clear();
        MapManager.PotionRectangles.Clear();
        InitializePotionPositions();
    }

    public void RemovePotionAt(int index)
    {
        if (index >= 0 && index < PotionPositions.Count)
            PotionPositions.RemoveAt(index);
    }

    /// <summary>
    /// Draw the Potion texture so that it can be seen in the HUD.
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var position in PotionPositions)
        {
            spriteBatch.Draw(_potionTexture, position, null, Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None,
                0f);
        }
    }
}

