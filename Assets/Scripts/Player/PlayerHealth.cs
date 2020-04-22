using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int TimeAlive { get; private set; }
    public int MaxHealth { get; set; } = 3;
    public delegate void OnHealthChange(int newHealth, int maxHealth, GameObject player);

    private List<OnHealthChange> healthListeners = new List<OnHealthChange>();

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
            foreach (OnHealthChange listener in healthListeners)
                listener.Invoke(health, MaxHealth, gameObject);
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
            FindObjectOfType<AudioManager>().PlayVoice(hurtSound.GetClip());
            
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

    public void AddHealthChangeListener(OnHealthChange listener)
    {
        if (!healthListeners.Contains(listener))
            healthListeners.Add(listener);
    }

    public void RemoveHealthChangeListener(OnHealthChange toRemove)
    {
        healthListeners.Remove(toRemove);
    }
}
