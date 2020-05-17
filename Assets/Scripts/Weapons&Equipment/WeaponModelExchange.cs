using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelExchange : MonoBehaviour {
    private static readonly int PISTOL = 0;
    private static readonly int SHOTGUN = 1;
    private static readonly int SNIPER = 2;

    [SerializeField]
    private GameObject[] weapons;

    private GameObject currentWeapon;

    void Start()
    {
        SetWeaponObject(weapons[PISTOL]);
    }

    void Update()
    {
        SwapWeapon();
    }

    private void SwapWeapon()
    {
        if (Input.GetButton("ChangeToPistol0"))
            SetWeaponObject(weapons[PISTOL]);
        if (Input.GetButton("ChangeToSniperOrShotgun0"))
            SetWeaponObject(weapons[SNIPER]);
        if (Input.GetButton("ChangeToSniperOrShotgun1"))
            SetWeaponObject(weapons[SHOTGUN]);
    }

    private void SetWeaponObject(GameObject weapon)
    {
        if(weapon != null)
            currentWeapon = weapon;

        SetOtherWeaponsUnActive();
    }

    private void SetOtherWeaponsUnActive()
    {
        foreach(GameObject weapon in weapons)
            if (weapon != currentWeapon)
                weapon.SetActive(false);
    }
}
