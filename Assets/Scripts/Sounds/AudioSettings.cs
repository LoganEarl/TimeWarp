using UnityEngine;

/*
 * Attach this to the GameObject to bring up Options Prefab for Options Menu
 */

public class AudioSettings : MonoBehaviour
{
    GameObject myGameObject;
    GameObject myOptionsMenu;
    
    void Awake()
    {
        myGameObject = GameObject.Find("AudioManager");
        AudioManager audioManager = AudioManager.GetInstance();
        myOptionsMenu = audioManager.GetOptionsMenu();
    }

    public void EnableOptionsMenu()
    {
        myOptionsMenu.SetActive(true);

    }

    public void DisableOptionsMenu()
    {
        myOptionsMenu.SetActive(false);
    }
}
