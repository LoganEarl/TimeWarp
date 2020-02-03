using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
    public static T AddComponentWithInit<T>(this GameObject obj, System.Action<T> onInit) where T : Component
    {
        bool oldState = obj.activeSelf;
        obj.SetActive(false);
        T comp = obj.AddComponent<T>();
        onInit?.Invoke(comp);
        obj.SetActive(oldState);
        return comp;
    }
}
