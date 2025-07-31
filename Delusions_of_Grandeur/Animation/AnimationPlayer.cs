// region File Description
// AnimationPlayer.cs
// Controls and plays a given animation.
//#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Animation;

/// <summary>
/// Controls playback of a given animation.
/// </summary>
public class AnimationPlayer
{
    private Animation _currentAnimation;
    private float _timeElapsed;
    private int _currentFrameX;
    private int _currentFrameY;
    private SpriteEffects _flip;
    private float _alpha = 1f;
    public bool IsPaused { get; set; }
    public bool IsDraw = true;

    private int FrameWidth => _currentAnimation.Texture.Width / _currentAnimation.FrameCountX;
    private int FrameHeight => _currentAnimation.Texture.Height / _currentAnimation.FrameCountY;

    /// <summary>
    /// Starts or restarts playback of an animation.
    /// </summary>
    /// <param name="animation">The animation to play.</param>
    public void PlayAnimation(Animation animation)
    {
        if (_currentAnimation != animation)
        {
            _currentAnimation = animation;
            _timeElapsed = 0;

	    int absoluteFrameIndex = _currentAnimation.StartFrame;
	    _currentFrameX = absoluteFrameIndex % _currentAnimation.FrameCountX;
	    _currentFrameY = absoluteFrameIndex / _currentAnimation.FrameCountX;

            _flip = SpriteEffects.None;
            IsPaused = false;
        }
    }

    /// <summary>
    /// Draws the current frame of the animation.
    /// </summary>

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float alpha, SpriteEffects spriteEffects = SpriteEffects.None)
    {
        if (_currentAnimation == null)
            return;

        _flip = _currentAnimation.IsFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        _alpha = alpha;

        if (!IsPaused)
        {

            _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeElapsed >= _currentAnimation.FrameTime)
            {
                _timeElapsed = 0;

                int totalFrames = _currentAnimation.EndFrame - _currentAnimation.StartFrame + 1;
                int currentIndex = (_currentFrameY * _currentAnimation.FrameCountX + _currentFrameX) % totalFrames;

                int nextFrameIndex = (currentIndex + 1) % totalFrames;
                int absoluteFrameIndex = _currentAnimation.StartFrame + nextFrameIndex;

                _currentFrameX = absoluteFrameIndex % _currentAnimation.FrameCountX;
                _currentFrameY = absoluteFrameIndex / _currentAnimation.FrameCountX;
            }
        }


        Rectangle sourceRectangle = new Rectangle(
            FrameWidth * _currentFrameX,
            FrameHeight * _currentFrameY,
            FrameWidth,
            FrameHeight
        );

        if (IsDraw)
        {
            spriteBatch.Draw(_currentAnimation.Texture, position, sourceRectangle, Color.White * _alpha, 0f,
                Vector2.Zero, _currentAnimation.Scale, _flip, 0f);
        }
    }
}
