using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomSound : MonoBehaviour
{
    [SerializeField] private float[] activationChance;
    [SerializeField] private string[] clipNames;


    public string GetClip()
    {
        int randomSound = Mathf.RoundToInt(Random.value * (clipNames.Length - 1));
        bool speaking = (Random.value * 100) <= activationChance[randomSound];

        if (speaking)
            return "Player" + clipNames[randomSound];
        else
            return null;
    }
}
