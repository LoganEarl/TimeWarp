﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] GameObject deadPlayer;

    public int TimeAlive { get; private set; }
    public int MaxHealth { get; set; } = 3;
    public delegate void OnHealthChange(int newHealth, int maxHealth, GameObject player);

    private List<OnHealthChange> healthListeners = new List<OnHealthChange>();
    private PlayerController playerController;

    private int health = 3;
    private bool dead = false;
    private RandomHurt hurtSound;
    private Color playerColor;

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
        }
    }

    private void LateUpdate()
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

        if (Health <= 0)
        {
            GameObject deadBody = Instantiate(deadPlayer, transform.position, transform.rotation);
            Material pMaterial = 
                gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()[1].material;

            deadBody.GetComponent<PlayerDeath>().PlayerMaterial = pMaterial;

            GetComponent<PlayerController>().GameMode.ClearOnRoundChange(deadBody);
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
