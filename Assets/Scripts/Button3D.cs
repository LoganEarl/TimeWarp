using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button3D : MonoBehaviour
{
    public LevelLoader LL;

    private void OnMouseDown() {
        LL.LoadNextLevel("Menu");
    }
}
