using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreOverlay : MonoBehaviour
{
    [SerializeField]
    private GameObject textTie;
    [SerializeField]
    private GameObject[] textWinner;
    [SerializeField]
    private TextMeshProUGUI[] textPlayerScoreHeader;
    [SerializeField]
    private TextMeshProUGUI[] textPlayerScoreLabels;
    [SerializeField]
    private TextMeshProUGUI[] textPlayerScoreValues;

    private IGameMode sourceGameMode;
    private bool setup = false;

    public void Setup(ScoreList scores, IGameMode gameMode)
    {
        this.sourceGameMode = gameMode;

        int max = 0, second = 0, maxIndex = 0;
        for(int i = 0; i < scores.PlayerScores.Length; i++)
        {
            if(scores.PlayerScores[i] >= max)
            {
                second = max;
                max = scores.PlayerScores[i];
                maxIndex = i;
            }
        }

        textTie.SetActive(false);
        for (int i = 0; i < textWinner.Length; i++)
            textWinner[i].SetActive(false);

        if (max != second)
            textWinner[maxIndex].SetActive(true);
        else
            textTie.SetActive(true);

        for (int i = 0; i < scores.ScoreListings.Length && i < textPlayerScoreValues.Length; i++)
        {
            textPlayerScoreHeader[i].text = "Player " + (i + 1) + " Score";

            textPlayerScoreLabels[i].text = "";
            textPlayerScoreValues[i].text = "";
            
            foreach (string item in scores.ScoreKeyOrder)
                if(scores.ScoreListings[i].ContainsKey(item)) {
                    textPlayerScoreLabels[i].text += item + ":\n";
                    textPlayerScoreValues[i].text += scores.ScoreListings[i][item] + "\n";
                }

            textPlayerScoreLabels[i].text += "Total:";
            textPlayerScoreValues[i].text += scores.PlayerScores[i];
        }

        Canvas drawCanvas = gameObject.GetComponentInChildren<Canvas>();
        drawCanvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        drawCanvas.planeDistance = 1;

        setup = true;
    }

    public void OnPressRestart()
    {
        if (setup)
        {
            Destroy(gameObject);
            sourceGameMode.Reset();
            sourceGameMode.Begin(); 
        }
    }

    public void OnPressQuit()
    {
        if (setup)
        {
            Destroy(sourceGameMode.GameObject);
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }

    public class ScoreList
    {
        public int[] PlayerScores { get; private set; }
        public Dictionary<string,int>[] ScoreListings { get; private set; }
        public string[] ScoreKeyOrder { get; private set; }

        public ScoreList(int[] playerScores, Dictionary<string, int>[] scoreListings, string[] scoreKeyOrder) =>
            (PlayerScores, ScoreListings, ScoreKeyOrder) = (playerScores, scoreListings, scoreKeyOrder);
    }
}
