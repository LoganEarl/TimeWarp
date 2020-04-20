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
        myOptionsMenu = AudioManager.GetInstance().GetOptionsMenu();
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
