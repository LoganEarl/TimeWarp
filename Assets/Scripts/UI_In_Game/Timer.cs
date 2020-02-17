using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private float mainTimer;
    [SerializeField]
    private int numIterations;

    private float[] timeForIterations;

    private float timer;
    private int currentIteration = 0;
    private bool canCount = true;
    private bool doOnce = false;

    private bool bothPlayersAlive = true;

    public void Start()
    {
        timer = mainTimer;
        timeForIterations = new float[numIterations];
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bothPlayersAlive = false;
            ResetTimer();
        }
        if (timer >= 0.0f && !doOnce && canCount && !bothPlayersAlive)
        {
            timerText.text = timer.ToString("F0");
            RecordRemainingTime();
            canCount = false;
            doOnce = true;
        }
        /*
        if (timer >= 0.00f && timer <= 10.01f && canCount)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F2");
        }*/
        else if (timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F0");
        }
        else if(timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            timerText.text = "0.00";
            timer = 0.0f;
            RecordRemainingTime();
        }
    }

    private void RecordRemainingTime()
    {
        if(currentIteration < numIterations)
            timeForIterations[currentIteration] = timer;
        currentIteration++;
    }

    public float GetTimer() { return timer; }

    public float GetRemainingTime()
    {
        return mainTimer - GetTimer();
    }

    public void ResetTimer()
    {
        timer = mainTimer;
        canCount = true;
        doOnce = false;
        bothPlayersAlive = true;
    }
}
