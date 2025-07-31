#region File description
// region File Description
// Animation.cs
// Handles the animation system.
#endregion

using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Animation;

/// <summary>
/// A factory for creating animations.
/// </summary>
public static class AnimationFactory
{
    /// <summary>
    /// Creates an animation with the specified parameters.
    /// </summary>
    public static Animation CreateAnimation(Texture2D texture, int frameCountX, int frameCountY, float frameTime, bool isLooping, bool isFlipped, int startFrame, int endFrame, float scale)
    {
        return new Animation
        {
            Texture = texture,
            FrameCountX = frameCountX,
            FrameCountY = frameCountY,
            FrameTime = frameTime,
            IsFlipped = isFlipped,
            StartFrame = startFrame,
            EndFrame = endFrame,
            Scale = scale
        };
    }
}

/// <summary>
/// Represents an animated texture.
/// </summary>
public class Animation
{
    /// <summary>
    /// The texture for the animation.
    /// </summary>
    public Texture2D Texture { get; init; }

    /// <summary>
    /// Amount of horizontal frames.
    /// </summary>
    public int FrameCountX { get; init; }

    /// <summary>
    /// Amount of vertical frames.
    /// </summary>
    public int FrameCountY { get; init; }

    /// <summary>
    /// The index that marks the start of the animation.
    /// </summary>
    public int StartFrame { get; init;  }
    
    /// <summary>
    /// The index that marks the end of the animation.
    /// </summary>
    public int EndFrame { get; init; }

    /// <summary>
    /// Duration of time to show each frame.
    /// </summary>
    public float FrameTime { get; init; }

    /// <summary>
    /// If true, the sprite is flipped along the y-axis.
    /// </summary>
    public bool IsFlipped { get; set; }
    
    /// <summary>
    /// Scaling factor used to scale the texture.
    /// </summary>
    public float Scale { get; init; }
    public float TotalDuration => (EndFrame - StartFrame + 1) * FrameTime;

}