using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Delusions_of_Grandeur;

public static class GlobalSettings
{
    public static float MediaPlayerVolume { get; set; } = 0.5f;
    
    public static Dictionary<string, Keys> KeyBindings = new()
    {
        // General
        { "EndBossActivate", Keys.M },
        { "DebugMode", Keys.I },
        {"GodMode", Keys.G},
        
        // Offensive player
        { "UpOffensive", Keys.Up },
        { "LeftOffensive", Keys.Left },
        { "RightOffensive", Keys.Right },
        { "ChangeWeapon", Keys.RightShift},
        
        
        // Defensive player
        { "UpDefensive", Keys.W },
        { "LeftDefensive", Keys.A },
        { "RightDefensive", Keys.D },
        { "Invisibility", Keys.R },
        { "PickUpPotion", Keys.E },
        { "ConsumePotion", Keys.F },
        { "ActivateShield", Keys.Q },
        {"ChangeShield", Keys.LeftShift}
        
    };
}