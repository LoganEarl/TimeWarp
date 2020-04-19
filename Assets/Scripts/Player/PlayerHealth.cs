using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int TimeAlive { get; private set; }
    public int MaxHealth { get; set; } = 3;

    private int health = 3;
    private bool dead = false;
    private AudioSource audioSource;
    private RandomHurt hurtSound;

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
            audioSource.PlayOneShot(hurtSound.GetClip(), 0.3f);
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
        audioSource = GetComponent<AudioSource>();
        hurtSound = GetComponent<RandomHurt>();
    }
}
