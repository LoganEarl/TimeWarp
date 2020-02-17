using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * This class operates the timer for the UI_In_Game. This will count down from
 * 90 seconds. This will also control the round number for the game.
 */

public class Timer : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private float mainTimer;
    [SerializeField]
    private int numIterations;

    public RoundNumber round;

    private float[] timeForIterations;

    private float timer;
    private int currentIteration = 0;
    private bool canCount = true;
    private bool doOnce = false;

    private bool bothPlayersAlive = true;

    public void Start()
    {
        timer = mainTimer;
        timeForIterations = new float[numIterations]; // Stores time for each iteration
    }

    public void Update()
    {
        // Testing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bothPlayersAlive = false;
            RecordRemainingTime();
            //PrintTimeArray();
            ResetTimer();
        }
        if (timer >= 0.0f && !doOnce && canCount && !bothPlayersAlive)
        {
            timerText.text = timer.ToString("F0");
            RecordRemainingTime();
            canCount = false;
            doOnce = true;
        }
        else if (timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F0");
        }
        else if(timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            timerText.text = "0";
            timer = 0.0f;
            RecordRemainingTime();
            //PrintTimeArray();
        }
    }

    private void RecordRemainingTime()
    {
        if(currentIteration < numIterations)
            timeForIterations[currentIteration] = GetRemainingTime();
        currentIteration++;
        round.incrementRoundNumber();
    }

    private void ResetTimer()
    {
        timer = mainTimer;
        canCount = true;
        doOnce = false;
        bothPlayersAlive = true;
    }

    public float GetRemainingTime()
    {
        return timer;
    }

    public float GetActualTime()
    {
        return mainTimer - GetRemainingTime();
    }

    public float[] GetTimeArray()
    {
        return timeForIterations;
    }

    public void PrintTimeArray()
    {
        foreach (float time in timeForIterations)
            Debug.Log(time.ToString("F2"));
    }
}
