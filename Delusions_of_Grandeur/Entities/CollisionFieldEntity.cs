using Microsoft.Xna.Framework;

namespace Delusions_of_Grandeur.Entities;

public class CollisionFieldEntity
{
    private Rectangle _hitBox;
    private bool _isEdited;
    private Vector2 _positionLastEdit;

    /// <summary>
    /// Creates a collision field for an Entity and is used for the collision detection.
    /// </summary>
    /// <param name="position">Position of the collision field of the Entity</param>
    /// <param name="fieldSize">Size of the collision field</param>
    public CollisionFieldEntity(Vector2 position, Vector2 fieldSize)
    {
        _hitBox = new Rectangle((int)position.X, (int)position.Y, (int)fieldSize.X, (int)fieldSize.Y);
        _isEdited = true;
        _positionLastEdit = position;
    }

    /// <summary>
    /// Update the position of the hitbox
    /// </summary>
    /// <param name="position">Gives new position.</param>
    public void UpdatePosition(Vector2 position)
    {
        _hitBox.X = (int)position.X;
        _hitBox.Y = (int)position.Y;
        if (Vector2.Distance(new Vector2(_hitBox.X, _hitBox.Y), _positionLastEdit) >= 1f)
        {
            _isEdited = true;
        }
    }

    /// <summary>
    /// Returns the Hitbox of the game object
    /// </summary>
    /// <returns>hit box</returns>
    public Rectangle GetHitbox()
    {
        return _hitBox;
    }

    public bool GetIsEdited()
    {
        return _isEdited;
    }

    public void SetIsEditedToFalse()
    {
        _isEdited = false;
        _positionLastEdit = new Vector2(_hitBox.X, _hitBox.Y);
    }
}
