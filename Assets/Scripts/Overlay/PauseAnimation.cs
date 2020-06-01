using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseAnimation : MonoBehaviour {
    ArrayList states = new ArrayList();
    float lastRealTime = 0.0f;

    public void PlayOnce(AnimationState state) {
        states.Add(state);
    }

    void Update() {
        for (int i = 0; i < states.Count; ++i) {
            AnimationState state = (AnimationState)states[i];
            state.weight = 1;
            state.enabled = true;
            state.time += (Time.realtimeSinceStartup - lastRealTime);

            if (state.time >= state.length)
                states.Remove(state);
        }
        lastRealTime = Time.realtimeSinceStartup;
    }
}