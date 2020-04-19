using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomSound : MonoBehaviour
{
    [SerializeField] private float[] activationChance;
    [SerializeField] private AudioClip[] clips;

    public AudioClip GetClip()
    {
        int randomSound = Mathf.RoundToInt(Random.value * (clips.Length - 1));
        bool speaking = (Random.value * 100) <= activationChance[randomSound];

        if (speaking)
            return clips[randomSound];
        else
            return null;
    }
}
