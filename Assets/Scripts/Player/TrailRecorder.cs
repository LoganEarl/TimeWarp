using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRecorder : MonoBehaviour
{
    private TrailRenderer trail;
    private PlayerController controller;
    private IGameMode gameMode;
    private bool setup = false;

    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
    }

    public void Setup(PlayerController controller, IGameMode gameMode)
    {
        this.controller = controller;
        this.gameMode = gameMode;
        setup = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (setup)
        {
            trail.emitting = 
                !gameMode.GameState.GetPlayerPositionsLocked(controller.PlayerNumber, controller.RoundNum) &&
                gameMode.GameState.GetPlayerVisible(controller.PlayerNumber, controller.RoundNum);
        }
    }
}
