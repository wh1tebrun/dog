# region File Description
// CollisionDetection.cs
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Entities;

public class CollisionDetection
{
    // Utilities
    private readonly GraphicsDevice _graphics;
    private CollisionField _collisionFieldToCheck;
    private Vector2 _coordinatesUpperLeftCorner;

    // Data structures
    private readonly List<CollisionField> _collisionFields;
    private readonly List<GameObject> _gameObjects;
    private readonly Dictionary<int, CollisionField> _dictionaryGameObjectToCollisionField;

    // Integer
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    private int _hitBoxCounter;

    // Boolean
    private bool _isTransformed;


    /// <summary>
    /// Generates the collision fields and distributes them evenly on the screen.
    /// Then know the players in each field based on their current position.
    /// </summary>
    /// <param name="numberOfCollisionFieldsInOneRow">Specifies the number of collision fields to be generated in a row and column.</param>
    /// <param name="graphics"> </param>
    public CollisionDetection(int numberOfCollisionFieldsInOneRow, GraphicsDevice graphics)
    {
        _graphics = graphics;
        _hitBoxCounter = 1;
        _gameObjects = [];
        AddHitBoxId(_gameObjects);
        _isTransformed = false;
        // var numberOfCollisionFields = numberOfCollisionFieldsInOneRow * numberOfCollisionFieldsInOneRow;
        var collisionFieldSize = CalculateCollisionFieldSize(numberOfCollisionFieldsInOneRow);
        _collisionFields = [];
        _dictionaryGameObjectToCollisionField = new Dictionary<int, CollisionField>();
        //---------------------------------------------------------------------------------------------
        // Loop for second screen height above camera
        for (var i = 0; i < numberOfCollisionFieldsInOneRow; i++)
        {
            for (var j = 0; j < numberOfCollisionFieldsInOneRow; j++)
            {
                _collisionFields.Add(new CollisionField(new Vector2(0 + i * collisionFieldSize.X, (0 - (2 * ScreenHeight)) + j * collisionFieldSize.Y), collisionFieldSize,
                    [], [], []));
            }
        }
        // Loop for first screen height above camera
        for (var i = 0; i < numberOfCollisionFieldsInOneRow; i++)
        {
            for (var j = 0; j < numberOfCollisionFieldsInOneRow; j++)
            {
                _collisionFields.Add(new CollisionField(new Vector2(0 + i * collisionFieldSize.X, (0 - ScreenHeight) + j * collisionFieldSize.Y), collisionFieldSize,
                    [], [], []));
            }
        }
        // Loop for fields on camera position
        for (var i = 0; i < numberOfCollisionFieldsInOneRow; i++)
        {
            for (var j = 0; j < numberOfCollisionFieldsInOneRow; j++)
            {
                _collisionFields.Add(new CollisionField(new Vector2(0 + i * collisionFieldSize.X, 0 + j * collisionFieldSize.Y), collisionFieldSize,
                    [], [], []));
            }
        }
        // Loop for first screen height under the camera
        for (var i = 0; i < numberOfCollisionFieldsInOneRow; i++)
        {
            for (var j = 0; j < numberOfCollisionFieldsInOneRow; j++)
            {
                _collisionFields.Add(new CollisionField(new Vector2(0 + i * collisionFieldSize.X, (0 + ScreenHeight) + j * collisionFieldSize.Y), collisionFieldSize,
                    [], [], []));
            }
        }
        // Loop for second screen height under the camera
        for (var i = 0; i < numberOfCollisionFieldsInOneRow; i++)
        {
            for (var j = 0; j < numberOfCollisionFieldsInOneRow; j++)
            {
                _collisionFields.Add(new CollisionField(new Vector2(0 + i * collisionFieldSize.X, (0 + (2 * ScreenHeight)) + j * collisionFieldSize.Y), collisionFieldSize,
                    [], [], []));
            }
        }


        //---------------------------------------------------------------------------------------------
        // Set neighboring fields
        for (int i = 0; i < _collisionFields.Count; i++)
        {
            if (i == 0) // top left corner
            {
                _collisionFields[0].SetNeighboringCollisionFields([
                    _collisionFields[1], _collisionFields[10], _collisionFields[11]
                ]);
            }
            else if (0 < i && i < 9) // upper row without the corners
            {
                _collisionFields[i].SetNeighboringCollisionFields([
                    _collisionFields[i - 1], _collisionFields[i + 1],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow + 1]
                ]);
            }
            else if (i == 9) // top right corner
            {
                _collisionFields[9].SetNeighboringCollisionFields([
                    _collisionFields[8], _collisionFields[18], _collisionFields[19]
                ]);
            }
            else if (i == 10 || i == 20 || i == 30 || i == 40 || i == 50 || i == 60 || i == 70 || i == 80 || i == 90 || i == 100 || i == 110 || i == 120 || i == 130 || i == 140 || i == 150 || i == 160 || i == 170 || i == 180 || i == 190 || i == 200 || i == 210 || i == 220 || i == 230 || i == 240 || i == 250 || i == 260 || i == 270 || i == 280 || i == 290 || i == 300 || i == 310 || i == 320 || i == 330 || i == 340 || i == 350 || i == 360 || i == 370 || i == 380 || i == 390 || i == 400 || i == 410 || i == 420 || i == 430 || i == 440 || i == 450 || i == 460 || i == 470 || i == 480) // left side without the corners
            {
                _collisionFields[i].SetNeighboringCollisionFields([
                    _collisionFields[i - numberOfCollisionFieldsInOneRow],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow + 1], _collisionFields[i + 1],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow + 1]
                ]);
            }
            else if (i == 490) // bottom left corner
            {
                _collisionFields[490].SetNeighboringCollisionFields([
                    _collisionFields[i - numberOfCollisionFieldsInOneRow],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow + 1], _collisionFields[i + 1]
                ]);
            }
            else if (490 < i && i < 499) // bottom row without the corners
            {
                _collisionFields[i].SetNeighboringCollisionFields([
                    _collisionFields[i - numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow + 1], _collisionFields[i - 1],
                    _collisionFields[i + 1]
                ]);
            }
            else if (i == 499) // bottom right corner
            {
                _collisionFields[499].SetNeighboringCollisionFields([
                    _collisionFields[i - numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow], _collisionFields[i - 1]
                ]);
            }
            else if (i == 19 || i == 29 || i == 39 || i == 49 || i == 59 || i == 69 || i == 79 || i == 89 || i == 99 || i == 109 || i == 119 || i == 129 || i == 139 || i == 149 || i == 159 || i == 169 || i == 179 || i == 189 || i == 199 || i == 209 || i == 219 || i == 229 || i == 239 || i == 249 || i == 259 || i == 269 || i == 279 || i == 289 || i == 299 || i == 309 || i == 319 || i == 329 || i == 339 || i == 349 || i == 359 || i == 369 || i == 379 || i == 389 || i == 399 || i == 409 || i == 419 || i == 429 || i == 439 || i == 449 || i == 459 || i == 469 || i == 479 || i == 489) // right side without the corners
            {
                _collisionFields[i].SetNeighboringCollisionFields([
                    _collisionFields[i - numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow], _collisionFields[i - 1],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow]
                ]);
            }
            else // fields in the middle
            {
                _collisionFields[i].SetNeighboringCollisionFields([
                    _collisionFields[i - numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow],
                    _collisionFields[i - numberOfCollisionFieldsInOneRow + 1], _collisionFields[i - 1],
                    _collisionFields[i + 1], _collisionFields[i + numberOfCollisionFieldsInOneRow - 1],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow],
                    _collisionFields[i + numberOfCollisionFieldsInOneRow + 1]
                ]);
            }
        }
        UpdateAllGameObjectsToCollisionFields();
    }

    /// <summary>
    /// Calculates the size of each field based on the desired number.
    /// </summary>
    /// <param name="numberOfCollisionFieldsInOneRow">Specifies the number of collision fields to be generated in a row and column.</param>
    /// <returns>Returns a Vector2 with the axes indicating the size of the fields.</returns>
    private Vector2 CalculateCollisionFieldSize(int numberOfCollisionFieldsInOneRow)
    {
        return new Vector2((int)Math.Ceiling((double)ScreenWidth / numberOfCollisionFieldsInOneRow), (int)Math.Ceiling((double)ScreenHeight / numberOfCollisionFieldsInOneRow));
    }

    /// <summary>
    /// Starts collision detection for a specific gameObject.
    /// </summary>
    /// <param name="gameObject">Object that should be controlled</param>
    /// <returns>Returns the gameObject that collides with.</returns>
    public GameObject RunCollisionDetectionForSpecificObject(GameObject gameObject)
    {
        if (gameObject.HitBoxIsActive && _dictionaryGameObjectToCollisionField.TryGetValue(gameObject.HitBoxId, out var value))
        {
            _collisionFieldToCheck = value;
            if (gameObject is Projectile && gameObject.OwnerId != "Player1" && gameObject.OwnerId != "Player2")
            {
                // These are enemy projectiles, so you only have to check for the players.
                foreach (GameObject gameObjectToCheck in _collisionFieldToCheck.GetPlayers())
                {
                    if (gameObjectToCheck.HitBoxId != gameObject.HitBoxId)
                    {
                        if (gameObject.HitBox.GetHitbox().Intersects(gameObjectToCheck.HitBox.GetHitbox()))
                        {
                            return gameObjectToCheck;
                        }

                        if (gameObject.HitBox.GetHitbox().Contains(gameObjectToCheck.HitBox.GetHitbox()))
                        {
                            return gameObjectToCheck;
                        }
                    }
                }
                foreach (CollisionField collisionField in _collisionFieldToCheck.GetNeighboringCollisionFields())
                {
                    foreach (GameObject gameObjectToCheck in collisionField.GetPlayers())
                    {
                        if (gameObjectToCheck.HitBoxId != gameObject.HitBoxId)
                        {
                            if (gameObject.HitBox.GetHitbox().Intersects(gameObjectToCheck.HitBox.GetHitbox()))
                            {
                                return gameObjectToCheck;
                            }

                            if (gameObject.HitBox.GetHitbox().Contains(gameObjectToCheck.HitBox.GetHitbox()))
                            {
                                return gameObjectToCheck;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (GameObject gameObjectToCheck in _collisionFieldToCheck.GetObjects())
                {
                    if (gameObjectToCheck.HitBoxId != gameObject.HitBoxId)
                    {
                        if (gameObject.HitBox.GetHitbox().Intersects(gameObjectToCheck.HitBox.GetHitbox()))
                        {
                            return gameObjectToCheck;
                        }

                        if (gameObject.HitBox.GetHitbox().Contains(gameObjectToCheck.HitBox.GetHitbox()))
                        {
                            return gameObjectToCheck;
                        }
                    }
                }

                foreach (GameObject gameObjectToCheck in _collisionFieldToCheck.GetPlayers())
                {
                    if (gameObjectToCheck.HitBoxId != gameObject.HitBoxId)
                    {
                        if (gameObject.HitBox.GetHitbox().Intersects(gameObjectToCheck.HitBox.GetHitbox()))
                        {
                            return gameObjectToCheck;
                        }

                        if (gameObject.HitBox.GetHitbox().Contains(gameObjectToCheck.HitBox.GetHitbox()))
                        {
                            return gameObjectToCheck;
                        }
                    }
                }

                foreach (CollisionField collisionField in _collisionFieldToCheck.GetNeighboringCollisionFields())
                {
                    foreach (GameObject gameObjectToCheck in collisionField.GetObjects())
                    {
                        if (gameObjectToCheck.HitBoxId != gameObject.HitBoxId)
                        {
                            if (gameObject.HitBox.GetHitbox().Intersects(gameObjectToCheck.HitBox.GetHitbox()))
                            {
                                return gameObjectToCheck;
                            }

                            if (gameObject.HitBox.GetHitbox().Contains(gameObjectToCheck.HitBox.GetHitbox()))
                            {
                                return gameObjectToCheck;
                            }
                        }
                    }

                    foreach (GameObject gameObjectToCheck in collisionField.GetPlayers())
                    {
                        if (gameObjectToCheck.HitBoxId != gameObject.HitBoxId)
                        {
                            if (gameObject.HitBox.GetHitbox().Intersects(gameObjectToCheck.HitBox.GetHitbox()))
                            {
                                return gameObjectToCheck;
                            }

                            if (gameObject.HitBox.GetHitbox().Contains(gameObjectToCheck.HitBox.GetHitbox()))
                            {
                                return gameObjectToCheck;
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Add a new gameObject to the collision detection. For Example if a new enemy is spawn.
    /// </summary>
    /// <param name="gameObject">New gameObject that should be added.</param>
    public void AddGameObjectToCollisionDetection(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        AddHitBoxId([gameObject]);
        UpdateOneGameObjectToCollisionFields(gameObject);
    }

    /// <summary>
    /// Deletes the given gameObject from the collision detection.
    /// </summary>
    /// <param name="gameObject"></param>
    public void DeleteGameObjectFromCollisionDetection(GameObject gameObject)
    {
        if (_dictionaryGameObjectToCollisionField.ContainsKey(gameObject.HitBoxId))
        {
            _dictionaryGameObjectToCollisionField[gameObject.HitBoxId].RemoveObject(gameObject);
            _dictionaryGameObjectToCollisionField.Remove(gameObject.HitBoxId);
            _gameObjects.Remove(gameObject);
        }
    }

    /// <summary>
    /// Update on game object to the collision fields.
    /// </summary>
    /// <param name="gameObject">Game object that should be updated.</param>
    private void UpdateOneGameObjectToCollisionFields(GameObject gameObject)
    {
        bool foundCollisionField = false;
        if (!gameObject.HitBox.GetIsEdited() && !_isTransformed)
        {
            // if the hit box is not edited over a specific value there is no need to update the field.
            // Also, if the fields are not moved is no need to update
            return;
        }

        // ---------------------------------------------------------------
        if (_dictionaryGameObjectToCollisionField.ContainsKey(gameObject.HitBoxId))
        {
            var field = _dictionaryGameObjectToCollisionField[gameObject.HitBoxId];
            if (field.GetSetPosition.X <= gameObject.HitBox.GetHitbox().X && gameObject.HitBox.GetHitbox().X <
                field.GetSetPosition.X + field.GetSize.X)
            {
                if (field.GetSetPosition.Y <= gameObject.HitBox.GetHitbox().Y && gameObject.HitBox.GetHitbox().Y < (field.GetSetPosition.Y + field.GetSize.Y))
                {
                    // Hit box is already in the same field, no changes needed
                    gameObject.HitBox.SetIsEditedToFalse();
                    return;

                }
            }

            // The hit box is not in the previous field now search in the neighboring fields of the previous field
            foreach (var neighbouringField in field.GetNeighboringCollisionFields())
            {
                if (neighbouringField.GetSetPosition.X <= gameObject.HitBox.GetHitbox().X && gameObject.HitBox.GetHitbox().X <
                    neighbouringField.GetSetPosition.X + neighbouringField.GetSize.X)
                {
                    if (neighbouringField.GetSetPosition.Y <= gameObject.HitBox.GetHitbox().Y && gameObject.HitBox.GetHitbox().Y < (neighbouringField.GetSetPosition.Y + neighbouringField.GetSize.Y))
                    {
                        // Hit box is in a neighbouring field
                        // Deletes the gameObject in the list of the previous collision field
                        _dictionaryGameObjectToCollisionField[gameObject.HitBoxId].RemoveObject(gameObject);
                        // Deletes the allocation in the dictionary between the gameObject and the collision field
                        _dictionaryGameObjectToCollisionField.Remove(gameObject.HitBoxId);
                        _dictionaryGameObjectToCollisionField.Add(gameObject.HitBoxId, neighbouringField);
                        neighbouringField.AddObject(gameObject);
                        gameObject.HitBox.SetIsEditedToFalse();
                        return;

                    }
                }
            }

        }
        // ---------------------------------------------------------------

        foreach (CollisionField collisionField in _collisionFields)
        {
            // Check the x-Coordinate
            if (collisionField.GetSetPosition.X <= gameObject.HitBox.GetHitbox().X && gameObject.HitBox.GetHitbox().X <
                collisionField.GetSetPosition.X + collisionField.GetSize.X)
            {
                if (collisionField.GetSetPosition.Y <= gameObject.HitBox.GetHitbox().Y && gameObject.HitBox.GetHitbox().Y < (collisionField.GetSetPosition.Y + collisionField.GetSize.Y))
                {
                    // Found the collision field that contains the coordinates that the gameObject has.
                    foundCollisionField = true;
                    if (_dictionaryGameObjectToCollisionField.ContainsKey(gameObject.HitBoxId)) //The dictionary contains the gameObject
                    {
                        // Deletes the gameObject in the List of the collision field
                        _dictionaryGameObjectToCollisionField[gameObject.HitBoxId].RemoveObject(gameObject);
                        // Deletes the allocation in the dictionary between the gameObject and the collision field
                        _dictionaryGameObjectToCollisionField.Remove(gameObject.HitBoxId);
                    }
                    _dictionaryGameObjectToCollisionField.Add(gameObject.HitBoxId, collisionField);
                    collisionField.AddObject(gameObject);
                }
            }
        }

        if (!foundCollisionField)
        {
            if (_dictionaryGameObjectToCollisionField.ContainsKey(gameObject.HitBoxId)) //The dictionary contains the gameObject
            {
                // Deletes the gameObject in the List of the collision field
                _dictionaryGameObjectToCollisionField[gameObject.HitBoxId].RemoveObject(gameObject);
                // Deletes the allocation in the dictionary between the gameObject and the collision field
                _dictionaryGameObjectToCollisionField.Remove(gameObject.HitBoxId);
            }
            _dictionaryGameObjectToCollisionField.Add(gameObject.HitBoxId, _collisionFields[0]);
            _collisionFields[0].AddObject(gameObject);
        }
        gameObject.HitBox.SetIsEditedToFalse();
    }

    /// <summary>
    /// Updates the assignment of all game objects to fields based on their position.
    /// </summary>
    public void UpdateAllGameObjectsToCollisionFields()
    {
        foreach (GameObject gameObject in _gameObjects)
        {
            UpdateOneGameObjectToCollisionFields(gameObject);
        }
    }

    /// <summary>
    /// Updates the position of the collision fields to the camera
    /// </summary>
    /// <param name="newPosition"></param>
    public void TransformCollisionFields(Vector2 newPosition)
    {
        var newPositionRounded = Math.Round(newPosition.Y - 1080 / 2f, 0, MidpointRounding.AwayFromZero);
        if (!(Math.Abs(newPositionRounded - _coordinatesUpperLeftCorner.Y) > 1e-1))
        {
            _isTransformed = false;
            return;
        }

        _isTransformed = true;
        var offsetY = _coordinatesUpperLeftCorner.Y - newPositionRounded;
        _coordinatesUpperLeftCorner.Y = (float)newPositionRounded;
        foreach (var collisionField in _collisionFields)
        {
            collisionField.GetSetPosition = new Vector2(collisionField.GetSetPosition.X, collisionField.GetSetPosition.Y - (float)offsetY);
        }
    }

    /// <summary>
    /// Gives every hit box a unique ID
    /// </summary>
    /// <param name="gameObjects">List of objects that have at the moment only the default ID</param>
    private void AddHitBoxId(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.HitBoxId = _hitBoxCounter;
            _hitBoxCounter++;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D hitBoxTexture = new Texture2D(_graphics, 1, 1);
        hitBoxTexture.SetData(new[] { Color.White });
        foreach (var gameObject in _gameObjects)
        {
            spriteBatch.Draw(
                hitBoxTexture,
                gameObject.HitBox.GetHitbox(),
                Color.White * 0.5f
            );
        }

        foreach (var collisionField in _collisionFields)
        {
            Rectangle rectangleToDraw = new Rectangle((int)collisionField.GetSetPosition.X, (int)collisionField.GetSetPosition.Y,
                (int)collisionField.GetSize.X, (int)collisionField.GetSize.Y);
            spriteBatch.Draw(hitBoxTexture,
                new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, 1),
                Color.White * 0.5f);
            spriteBatch.Draw(hitBoxTexture,
                new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, 1, rectangleToDraw.Height),
                Color.White * 0.5f);
            spriteBatch.Draw(hitBoxTexture,
                new Rectangle(rectangleToDraw.X, rectangleToDraw.Y + rectangleToDraw.Height, rectangleToDraw.Width, 1),
                Color.White * 0.5f);
            spriteBatch.Draw(hitBoxTexture,
                new Rectangle(rectangleToDraw.X, rectangleToDraw.Y + rectangleToDraw.Height, 1, rectangleToDraw.Height),
                Color.White * 0.5f);
        }

    }
}
