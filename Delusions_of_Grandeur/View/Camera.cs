using System;
using Microsoft.Xna.Framework;

namespace Delusions_of_Grandeur.View;

public class Camera
{
    public Matrix Transform {get; set;}
    public Vector2 Centre {get; set;}
    public Vector2 CurrentPosition = Vector2.Zero;
    private const float LowerLimit = 1080;
    private const float Offset = 250;
    private const float MinZoom = 0.5f;
    private const float MaxZoom = 2.0f;
    public float Zoom { get; set; } = 1.0f;
    public float LowerBoundary {get; set;}
    public int CounterInBeginning {get; set;}
    public float ShakeDuration {get; set;}
    public float ShakeIntensity {get; set;}
    private readonly Random _random = new Random();

    private const float LerpFactor = 0.1f;

    /// <summary>
    /// Center the camera to the average height of the two players.
    /// The offset sets the camera position to fit to the lower border of the map.
    /// If one player falls from the map, the camera keeps its initial position.
    /// </summary>
    /// <param name="targetPosition"></param>
    public void Update(Vector2 targetPosition)
    { 
	    var targetY = CurrentPosition.Y;
		
		if (targetPosition.Y > LowerLimit - Offset)
		{
		    targetY = LowerLimit - Offset;
		}
		else if (targetPosition.Y < CurrentPosition.Y - 540)
		{
			targetY = MathHelper.Lerp(CurrentPosition.Y, targetPosition.Y, LerpFactor * 0.3f);
		}
		else if (CounterInBeginning < 50)
		{
			targetY = targetPosition.Y;
		}
		CurrentPosition.Y = MathHelper.Lerp(CurrentPosition.Y, targetY, LerpFactor);
		
		var shakeOffset = Vector2.Zero;
		if (ShakeDuration > 0)
		{
		    shakeOffset = new Vector2(
			(float)(_random.NextDouble() * 2 - 1) * ShakeIntensity,
			(float)(_random.NextDouble() * 2 - 1) * ShakeIntensity
		    );
		    ShakeDuration -= 1; // Decrease duration
		}

		// Calculate center with the shake offset
		Centre = new Vector2(0, CurrentPosition.Y - Offset) + shakeOffset;

		LowerBoundary = CurrentPosition.Y + 230;

		Transform = Matrix.CreateTranslation(new Vector3(0, -Centre.Y, 0)) *
		    Matrix.CreateTranslation(0, LowerLimit / 1.9f, 0);

		CounterInBeginning += 1;
    }

    public void Shake(float intensity, float duration)
    {
        ShakeIntensity = intensity;
        ShakeDuration = duration;
    }

    public float GetLowerBoundary()
    {
	    return CounterInBeginning < 20 ? LowerLimit : LowerBoundary;
    }
    public Vector2 CurrentCenter => Centre;
}
