# region File Description
// Debugging.cs
#endregion

using System;
using System.Collections.Generic;
using Delusions_of_Grandeur.Entities;
using Delusions_of_Grandeur.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Utilities;

public class Debugging
{
    private readonly Pathfinding.Grid _grid;
    private readonly SpriteFont _font;
    private readonly CollisionDetection _collisionDetection;

    private readonly List<Player> _players;

    private readonly Texture2D _texture;

    public Debugging(GraphicsDevice graphics, AssetManager assetManager, List<Player> players, CollisionDetection collisionDetection, Pathfinding.Grid grid)
    {
        _texture = new Texture2D(graphics, 1, 1);
        _texture.SetData([Color.DarkSlateGray]);

        var random = new Random();
        _grid = grid;

        var movingObjectTexture = assetManager.GetTexture("metaparticle");

        var movingObjects = new List<MovingObject>();
        if (movingObjects == null) throw new ArgumentNullException(nameof(movingObjects));
        
        for (int i = 0; i < 100; i++)
        {
            Point position = new Point(random.Next(0, graphics.Viewport.Width), random.Next(0, graphics.Viewport.Height));
            Vector2 velocity = new Vector2(random.Next(-3, 4), random.Next(-3, 4));
            movingObjects.Add(new MovingObject(graphics, movingObjectTexture, position, velocity));
        }

        _players = players;

        _font = assetManager.GetFont("MainMenu");
        _collisionDetection = collisionDetection;
    }

    public void Update()
    {

    }
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        _collisionDetection.Draw(spriteBatch);

        foreach (var player in _players)
        {
            spriteBatch.DrawString(_font, $"Player-Position: {player.Position}", new Vector2(player.Position.X, player.Position.Y - 30), Color.Purple);
            spriteBatch.DrawString(_font, $"Player-Position: {player.Position}", new Vector2(player.Position.X, player.Position.Y - 30), Color.Purple);
        }

        if (_players[0].GodMode)
        {
            spriteBatch.DrawString(_font, "God Mode ", new Vector2(20, 10), Color.Yellow);

        }
        spriteBatch.DrawString(_font, "FPS: " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 1, MidpointRounding.AwayFromZero), new Vector2(20, 50), Color.Yellow);
    }
}
