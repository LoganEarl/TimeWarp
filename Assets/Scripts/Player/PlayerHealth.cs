using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth { get; set; } = 3;
    private int health = 3;
    private bool dead = false;

    public int Health {
        get {
            return health;
        }
        set {
            if (value > MaxHealth) value = MaxHealth;
            if (value < 0) value = 0;
            health = value;
            Dead = Health == 0;
        }
    }
    public bool Dead {
        get {
            return dead;
        }
        private set {
            dead = value;
            gameObject.SetActive(!dead);
        }
    }

    public void DoDamage(int damage)
    {
        if(damage > 0)
            Health -= damage;
    }

    public void FullHeal()
    {
        Health = MaxHealth;
    }

    public void Awake()
    {
        FullHeal();
    }
}
