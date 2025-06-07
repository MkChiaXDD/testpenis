using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagment : MonoBehaviour
{
    int Health;
    int Money;
    int lives;

    public static PlayerManagment Instance 
    {
        get
        {
            return instance;
        }
    
    
    }

    private static PlayerManagment instance = null;

    void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }


    public int _PlayerMoney;
    public bool _Ispause;
    public int _cash;
    public int _lives;

    public void SpendCash(int cash)
    {
        this._cash = this._cash - cash;
    }

    public void LossLive(int lives)
    {
        this._lives = this._lives - lives;
    }

    public void SpendPlayerCurrency(int PlayerCurrency)
    {
        this._PlayerMoney = this._PlayerMoney - PlayerCurrency;
    }
}
