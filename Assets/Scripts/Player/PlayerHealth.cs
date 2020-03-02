﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private AudioClip hurtSound;

    public int TimeAlive { get; private set; }
    public int MaxHealth { get; set; } = 3;

    private int health = 3;
    private bool dead = false;
    private AudioSource audio;

    public int Health {
        get {
            return health;
        }
        set {
            if (value > MaxHealth) value = MaxHealth;
            if (value < 0) value = 0;
            health = value;
            Dead = Health <= 0;
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

    private void FixedUpdate()
    {
        TimeAlive++;
    }

    public void DoDamage(int damage)
    {
        if (damage > 0)
        {
            Health -= damage;
            float temp = audio.volume;
            audio.PlayOneShot(hurtSound, 0.3f);
            //audio.volume = temp;
        }
    }

    public void FullHeal()
    {
        Health = MaxHealth;
    }

    public void ResetStatistics()
    {
        TimeAlive = 0;
    }

    public void Awake()
    {
        FullHeal();
        audio = GetComponent<AudioSource>();
    }
}
