# region File Description
// DebugMovingObject.cs
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Utilities;

public class MovingObject
{
    private Point _position;
    private Vector2 _velocity;
    private readonly Texture2D _texture;

    private readonly GraphicsDevice _graphics;

    public MovingObject(GraphicsDevice graphics, Texture2D texture, Point position, Vector2 velocity)
    {
        _texture = texture;
        _position = position;
        _velocity = velocity;
        _graphics = graphics;
    }
}
