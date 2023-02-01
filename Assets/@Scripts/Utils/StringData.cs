using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringData 
{
    private const string _player = "Player";
    public static string Player => _player;
    private const string _healthBar = "Healthbar";
    public static string HealthBar => _healthBar;

    private const string _sound = "SoundData";
    public static string Sound=> _sound;

    #region BackEnd
    
    private const string _energy = "EN";
    public static string Energy => _energy;

    private const string _coin = "CO";
    public static string Coin => _coin;


    private const string _dia = "DI";
    public static string Diamond => _dia;

    #endregion

    public class AdMob
    {
        private const string _respwan = "ca-app-pub-8904224703245079/8096563976";
        public static string Respawn => _respwan; 
    }
}
