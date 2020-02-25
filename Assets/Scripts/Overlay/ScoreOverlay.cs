using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreOverlay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textWinner;
    [SerializeField]
    private TextMeshProUGUI[] textPlayerScores;

    private IGameMode sourceGameMode;
    private bool setup = false;

    public void Setup(ScoreList scores, IGameMode gameMode)
    {
        this.sourceGameMode = gameMode;

        int max = 0, maxIndex = 0;
        for(int i = 0; i < scores.PlayerScores.Length; i++)
        {
            if(scores.PlayerScores[i] > max)
            {
                max = scores.PlayerScores[i];
                maxIndex = i;
            }
        }

        textWinner.text = "Player " + (maxIndex + 1) + " Wins!";

        for(int i = 0; i < scores.ScoreListings.Length && i < textPlayerScores.Length; i++)
        {
            string text = "Player " + (i + 1) + " Score\n";
            foreach (string item in scores.ScoreKeyOrder)
                if(scores.ScoreListings[i].ContainsKey(item))
                    text += "\t" + item + ":" + scores.ScoreListings[i][item] + "\n";
            text += "Total: " + scores.PlayerScores[i];
            textPlayerScores[i].text = text;
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
