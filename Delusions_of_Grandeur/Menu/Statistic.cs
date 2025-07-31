namespace Delusions_of_Grandeur.Menu;

public class Statistic
{
    public string Name { get; set; }

    public int Value { get; set; }
    
    public Statistic(string name)
    {
        Name = name;
        Value = 0;
    }
}