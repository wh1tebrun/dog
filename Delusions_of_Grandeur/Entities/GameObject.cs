#region File Description
// GameObject.cs
#endregion

using System.Collections.Generic;
using Delusions_of_Grandeur.Menu;
using Microsoft.Xna.Framework;

namespace Delusions_of_Grandeur.Entities;

public class GameObject
{
    protected CollisionFieldEntity GameObjectHitBox;
    protected CollisionDetection CollisionDetection;
    
    private Vector2 _hitBoxSize;
    public Vector2 HitBoxPositionOffset;
    public int GameObjectHealthPoints { get; set; }
    private int _hitBoxId;
    
    public bool HitBoxIsActive;
    
    public string OwnerId { get; set; }
    
    protected List<Statistic> Statistics;
    protected List<Achievement> Achievements;

    /// <summary>
    /// Get or set the position of the game object
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Get or set the hitbox of the game object
    /// </summary>
    public CollisionFieldEntity HitBox
    {
        get => GameObjectHitBox;
        set => GameObjectHitBox = value;
    }


    /// <summary>
    /// Get or Set the hitbox ID
    /// </summary>
    public int HitBoxId
    {
        get { return _hitBoxId; }
        set { _hitBoxId = value; }
    }
    

    /// <summary>
    /// Creates a hitbox with the given size and positions them at the game object position.
    /// </summary>
    /// <param name="size">Size of the new hitbox</param>
    protected void CreateHitBox(Vector2 size)
    {
        _hitBoxSize = size;
        _hitBoxId = 0;
        GameObjectHitBox = new CollisionFieldEntity(new Vector2(Position.X + HitBoxPositionOffset.X, Position.Y + HitBoxPositionOffset.Y), _hitBoxSize);
        HitBoxIsActive = true;
    }

    /// <summary>
    /// Get and Set the health points from this gameObject.
    /// </summary>
    public int HealthPoints
    {
        get => GameObjectHealthPoints;
        set => GameObjectHealthPoints = value;
    }

    /// <summary>
    /// Deactivate the Hitbox from this gameObject.
    /// </summary>
    public void DeactivateHitBox()
    {
        if (HitBoxIsActive)
        {
            CollisionDetection.DeleteGameObjectFromCollisionDetection(this);
            HitBoxIsActive = false;
        }
    }
    
    public void HitboxPositionUpdate()
    {
        GameObjectHitBox.UpdatePosition(new Vector2(Position.X + HitBoxPositionOffset.X, Position.Y + HitBoxPositionOffset.Y));
    }
    
    public void HitboxPositionUpdate(Vector2 positionOffset)
    {
        GameObjectHitBox.UpdatePosition(new Vector2(Position.X + HitBoxPositionOffset.X + positionOffset.X, Position.Y + HitBoxPositionOffset.Y + positionOffset.Y));
    }

    public virtual void DealDamage(int damage)
    {
        GameObjectHealthPoints -= damage;
    }
}
