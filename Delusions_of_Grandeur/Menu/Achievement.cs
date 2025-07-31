#region File Description
// Achievement.cs
// Handle the achievement for the game.
#endregion

namespace Delusions_of_Grandeur.Menu;

/// <summary>
/// a class for one achievement.
/// </summary>
public class Achievement
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool Completed { get; set; }

    public int ValueToComplete { get; set; }
    public int CurrentValue { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="valueToComplete"></param>
    public Achievement(string name, string description, int valueToComplete)
    {
        Name = name;
        Description = description;
        Completed = false;
        ValueToComplete = valueToComplete;
    }


    /// <summary>
    /// Increases the value if the achievement is not already completed. After increasing the
    /// value it checks if the Achievement is completed.
    /// Special case:  For the achievements with the playing time and the revivals, the final game time and the other revivals are then handed over.
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseValue(int amount)
    {
        if (Completed) return;
        
        CurrentValue += amount;
        if (ValueToComplete <= CurrentValue)
        {
            Completed = true;
        }
    }
}