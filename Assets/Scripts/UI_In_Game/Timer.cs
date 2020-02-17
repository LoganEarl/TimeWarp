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
        timerText = GetComponent<TextMeshProUGUI>();
        timer = mainTimer;
        timeForIterations = new float[6];
    }

    public void Update()
    {
        /*if (timer >= 0.0f && !doOnce && canCount && !bothPlayersAlive)
        {
            timerText.text = timer.ToString("F");
            RecordTime();
            canCount = false;
            doOnce = true;
        }*/
        if (timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F");
        }
        else if(timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            timerText.text = "0.00";
            timer = 0.0f;
            RecordTime();
        }
    }

    private void RecordTime()
    {
        timeForIterations[currentIteration] = timer;
        currentIteration++;
    }

    public float GetTimer() { return timer; }

    public float GetRemainingTime()
    {
        return mainTimer - GetTimer();
    }
}
