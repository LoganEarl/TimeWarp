using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class maintains the health bar of a player. Taking damage will cause the
 * slider to change in value in accordance to how much health that player has left.
 * Other classes may reference this to control the UI of the player health bar.
 */

public class HealthBar : MonoBehaviour {
    [SerializeField]
    private Slider slider;

    public PlayerHealth playerHealth;
    
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = 0;
    }

    public void SetHealth(int health)
    {
        slider.value = (int)slider.maxValue - health;
    }
}
