using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Delusions_of_Grandeur.Entities;

public class CollisionField
{
    private Vector2 _position;
    private readonly List<GameObject> _gameObjects;
    private readonly List<GameObject> _players; 
    private readonly List<GameObject> _projectiles;
    private List<CollisionField> _neighboringCollisionFields;
    
    /// <summary>
    /// Creates a collision field with the size and position.
    /// </summary>
    /// <param name="position">Vector2 indicating the position on the screen.</param>
    /// <param name="size">Vector2, which specifies the size of the field.</param>
    /// <param name="gameObjects">List of gameObjects that is set to this collision field.</param>
    /// <param name="projectiles">List of projectiles that is set to this collision field.</param>
    /// <param name="players">List of players that is set to this collision field.</param>
    public CollisionField(Vector2 position, Vector2 size, List<GameObject> gameObjects, List<GameObject> projectiles, List<GameObject> players)
    {
        _position = position;
        GetSize = size;
        _gameObjects = gameObjects;
        _projectiles = projectiles;
        _players = players;
    }

    public void SetNeighboringCollisionFields(List<CollisionField> collisionFields)
    {
        _neighboringCollisionFields = collisionFields;
    }

    public List<CollisionField> GetNeighboringCollisionFields()
    {
        return _neighboringCollisionFields;
    }

    /// <summary>
    /// Returns the position of the field.
    /// </summary>
    /// <returns>Vector2 which indicates the position.</returns>
    public Vector2 GetSetPosition
    {
        get => _position;
        set => _position = value;

    }

    /// <summary>
    /// Returns the size of the field.
    /// </summary>
    /// <returns>Vector2 which indicates the size.</returns>
    public Vector2 GetSize { get; }

    /// <summary>
    /// Returns a list of all players assigned to the field.
    /// </summary>
    /// <returns>List with all players assigned to this field</returns>
    public List<GameObject> GetObjects()
    {
        return _gameObjects;
    }

    public List<GameObject> GetPlayers()
    {
        return _players;
    }

    

    /// <summary>
    /// Adds a player to the field.
    /// </summary>
    /// <param name="gameObject">Adds this gameObject to the field</param>
    public void AddObject(GameObject gameObject)
    {
        if (gameObject is Player)
        {
            _players.Add(gameObject);
        }
        else if (gameObject is not Projectile)
        {
            _gameObjects.Add(gameObject);
        }
        else
        {
            _projectiles.Add(gameObject);
        }
    }

    /// <summary>
    /// Deletes the assignment of a player to this field.
    /// </summary>
    /// <param name="gameObject">This gameObject's assignment to this field is deleted.</param>
    public void RemoveObject(GameObject gameObject)
    {
        if (gameObject is Player)
        {
            _players.Remove(gameObject);
        }
        else if (gameObject is not Projectile)
        {
            _gameObjects.Remove(gameObject);
        }
        else
        {
            _projectiles.Remove(gameObject);
        }
    }
}