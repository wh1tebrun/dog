#region File description
// region File Description
// AttackSystem.cs
// Handles the attack system.
#endregion

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Entities;

/// <summary>
/// A class for the attack system. It manages all the projectiles shot by a player/enemy.
/// </summary>
public class AttackSystem
{
    public List<Projectile> Projectiles { get; } = [];

    /// <summary>
    /// Adds a new projectile to the Projectiles list.
    /// </summary>
    /// <param name="projectile"></param>
    public void AddProjectile(Projectile projectile)
    {
        Projectiles.Add(projectile);
    }

    /// <summary>
    /// Updates all projectiles.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        foreach (var projectile in Projectiles)
        {
            projectile.Update(gameTime);
        }
        Projectiles.RemoveAll(p => p.Lifespan <= 0);
    }

    /// <summary>
    /// Draws all projectiles.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var projectile in Projectiles)
        {
            projectile.Draw(spriteBatch);
        }
    }

    public void DeleteAllProjectiles()
    {
        foreach (var projectile in Projectiles)
        {
            projectile.DeleteProjectile();
        }
    }
}
