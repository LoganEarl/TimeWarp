using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerHealth = 1;
    
    public void TakeDamage(int dmg) {
        playerHealth -= dmg;

        if (playerHealth <= 0)
            Death();
    }

    void Death() {
        Destroy(gameObject, 0f);
    }
}
