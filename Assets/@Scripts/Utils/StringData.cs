using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringData 
{
    private const string _player = "Player";
    public static string Player => _player;
    private const string _healthBar = "Healthbar";
    public static string HealthBar => _healthBar;

    #region BackEnd
    
    private const string _energy = "EN";
    public static string Energy => _energy;
    
    #endregion
}
