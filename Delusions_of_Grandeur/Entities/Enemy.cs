using System.Collections.Generic;
using System.Linq;
using Delusions_of_Grandeur.Pathfinding;
using Microsoft.Xna.Framework.Graphics;

namespace Delusions_of_Grandeur.Entities;

public abstract class Enemy : GameObject
{
    // Float
    public float MoveTimeX { get; init; }
    public float MoveTimeY { get; init; }
    public float MaxMoveTime { get; init; } = 2f;
    public float Speed { get; set; }

    // Boolean
    public bool IsAlive { get; set; } = true;
    public bool IsMovingInXDirection { get; init; } = true;

    // Integer
    public int MoveDirectionX { get; set; } = 1;
    public int MoveDirectionY { get; init; } = -1;
    
    public int MapIndex { get; set; }

    // Utilities
    public Animation.AnimationPlayer AnimationPlayer { get; protected init; }
    protected Dictionary<string, Animation.Animation> Animations { get; init; }

    // Player
    protected Player PlayerDefensive;
    protected Player PlayerOffensive;

    // Pathfinding
    protected readonly AStar AStar;

    protected Grid Grid { get; }

    protected Enemy(GraphicsDevice graphicsDevice, Grid grid, Player playerOffensive, Player playerDefensive)
    {
        Grid = grid;
        AStar = new AStar(graphicsDevice, Grid);
        PlayerOffensive = playerOffensive;
        PlayerDefensive = playerDefensive;
    }
    /// <summary>
    /// Increases the value of statistic "Defeated Enemies" by one point.
    /// </summary>
    protected void IncreaseStatisticDefeatedEnemies()
    {
        foreach (var statistic in Statistics.Where(statistic => statistic.Name == "Total defeated enemies"))
        {
            statistic.Value += 1;
        }
    }

    /// <summary>
    /// Increases the value of achievement "Attack expert" by one point.
    /// </summary>
    protected void IncreaseAchievementAttackExpert()
    {
        foreach (var achievement in Achievements.Where(achievement => achievement.Name == "Attack expert"))
        {
            achievement.IncreaseValue(1);
        }
    }
}
