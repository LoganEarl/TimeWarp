using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelExchange : MonoBehaviour {
    private static readonly int PLAYER_0 = 0;
    private static readonly int PLAYER_1 = 1;
    private static readonly int PISTOL = 0;
    private static readonly int SHOTGUN = 1;
    private static readonly int SNIPER = 2;
    private static readonly string PISTOL_NAME_P1 = "ChangeToPistol1";
    private static readonly string SNIPER_SHOTGUN_NAME_P1 = "ChangeToSniperOrShotgun1";
    private static readonly string PISTOL_NAME_P0 = "ChangeToPistol0";
    private static readonly string SNIPER_SHOTGUN_NAME_P0 = "ChangeToSniperOrShotgun0";

    [SerializeField]
    private int playerNum;
    [SerializeField]
    private GameObject[] weapons;

    private GameObject p0_CurrentWeapon;
    private GameObject p1_CurrentWeapon;

    void Start()
    {
        ResetWeapons();
    }

    void Update()
    {
        if(!PauseOverlay.GameIsPaused)
            SwapWeapon();
    }
    
    private void SwapWeapon()
    {
        if (playerNum == 1)
        {
            if (Input.GetAxisRaw(PISTOL_NAME_P1) > 0)
                SetWeaponObject(weapons[PISTOL], PLAYER_1);
            if (Input.GetAxisRaw(SNIPER_SHOTGUN_NAME_P1) < 0)
                SetWeaponObject(weapons[SNIPER], PLAYER_1);
            if (Input.GetAxisRaw(SNIPER_SHOTGUN_NAME_P1) > 0)
                SetWeaponObject(weapons[SHOTGUN], PLAYER_1);
        }

        else
        {
            if (Input.GetAxisRaw(PISTOL_NAME_P0) > 0)
                SetWeaponObject(weapons[PISTOL], PLAYER_0);
            if (Input.GetAxisRaw(SNIPER_SHOTGUN_NAME_P0) < 0)
                SetWeaponObject(weapons[SNIPER], PLAYER_0);
            if (Input.GetAxisRaw(SNIPER_SHOTGUN_NAME_P0) > 0)
                SetWeaponObject(weapons[SHOTGUN], PLAYER_0);
        }
    }

    private void SetWeaponObject(GameObject weapon, int playerNum)
    {
        if (weapon != null)
        {
            if (playerNum == 0)
                p0_CurrentWeapon = weapon;
            else
                p1_CurrentWeapon = weapon;
        }

        SetAllWeaponsUnActive();

        if (playerNum == 0)
            p0_CurrentWeapon.SetActive(true);
        else
            p1_CurrentWeapon.SetActive(true);
    }

    private void SetAllWeaponsUnActive()
    {
        foreach(GameObject weapon in weapons)
                weapon.SetActive(false);
    }

    public void ResetWeapons()
    {
        SetWeaponObject(weapons[PISTOL], PLAYER_0);
        SetWeaponObject(weapons[PISTOL], PLAYER_1);
    }
}
